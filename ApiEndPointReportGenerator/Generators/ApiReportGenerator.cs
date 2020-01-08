using ApiEndPointReportGenerator.Helpers;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;

namespace ApiEndPointReportGenerator.Generators
{
  internal class ApiReportGenerator
  {
    private EndPointReportConfigResource _Config;
    public void Generate(FileInfo configResourceFileInfo)
    {
      _Config = ResourceHelper.GenerateResourceFromFile(configResourceFileInfo);
      BuildRedirectAssemblies();
      var controllers = GetControllersFromAssembly();
      var report = GenerateReport(controllers);
      SaveReportToOutputFile(report);
    }

    private void BuildRedirectAssemblies()
    {
      foreach (var assemblyNeeded in _Config.AssembliesNeeded)
      {
        var version = new Version(assemblyNeeded.Version);
        AssemblyRedirectHandler.RedirectAssembly(assemblyNeeded.Name, version, assemblyNeeded.PublicKey);
      }
    }

    private IEnumerable<Type> GetControllersFromAssembly()
    {
      var apiClasses = Assembly.LoadFrom(_Config.SourceFilename)
        .GetTypes()
        .Where(t => t.IsNotCompilerGenerated() && !t.IsEnum && t.IsClass);

      IEnumerable<Type> controllers = GetAllRelevantControllerClasses(apiClasses, _Config.ControllerNamespace, _Config.BaseControllerName);
      return controllers;
    }

    private IList<Tuple<string, string, bool>> GenerateReport(IEnumerable<Type> controllers)
    {
      var routes = new List<Tuple<string, string, bool>>();

      foreach (var controller in controllers)
      {
        var controllerMethods = controller.GetAllMethodsCreatedByHumans(_Config.ControllerNamespace)
          .Where(i => i.IsPublic && !i.DeclaringType.FullName.Contains(_Config.HelpPageControllerNamespaceToAvoid));

        foreach (var myMethod in controllerMethods)
        {
          var methodHasRouteeAttribute = myMethod
                                           .GetCustomAttributes(typeof(RouteAttribute), false).Length > 0;
          if (methodHasRouteeAttribute)
          {

            var routeAttributes = myMethod.GetCustomAttributes(typeof(RouteAttribute), false);
            bool isIgnoredByAutoDoc = false;
            var ignoreAttributes = myMethod.GetCustomAttributes(typeof(ApiExplorerSettingsAttribute), false);
            var hasIgnoreAttributes = ignoreAttributes.Length > 0;
            if (hasIgnoreAttributes)
            {
              isIgnoredByAutoDoc = ((ApiExplorerSettingsAttribute)ignoreAttributes[0]).IgnoreApi == true;
            }

            var methodSignature = MethodSignature(myMethod);
            foreach (var routeAttribute in routeAttributes)
            {
              var routeAttributeCast = (RouteAttribute)routeAttribute;
              var newEntry = new Tuple<string, string, bool>(routeAttributeCast.Template, methodSignature, isIgnoredByAutoDoc);
              routes.Add(newEntry);
            }
          }
        }
      }

      routes.Sort((x, y) =>
      {
        if (x == null) throw new ArgumentNullException(nameof(x));
        return String.Compare(x.Item1, y.Item1, StringComparison.Ordinal);
      });

      return routes;
    }

    private void SaveReportToOutputFile(IList<Tuple<string, string, bool>> routes)
    {
      using (StreamWriter writer = new StreamWriter(_Config.OutputFilename, false))
      {
        foreach (var route in routes)
        {
          writer.WriteLine(route.Item1 + "|" + route.Item2 + "|" + route.Item3);
        }
      }
    }

    private IEnumerable<Type> GetAllRelevantControllerClasses(IEnumerable<Type> retailWebApiClasses,
      string controllerNamespace, string baseControllerName)
    {
      return retailWebApiClasses
        .Where(x => x.FullName.Contains(controllerNamespace)
                    && x.Name.EndsWith("Controller") && !x.Name.Equals(baseControllerName))
        .ToList();
    }

    private static string MethodSignature(MethodInfo mi)
    {
      String[] param = mi.GetParameters()
        .Select(p => $"{p.ParameterType.Name} {p.Name}")
        .ToArray();

      string signature = $"{mi.Name}({String.Join(",", param)})";

      return signature;
    }
  }
}
