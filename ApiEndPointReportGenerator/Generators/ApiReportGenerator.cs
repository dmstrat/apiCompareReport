using ApiEndPointReportGenerator.Helpers;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;

namespace ApiEndPointReportGenerator.Generators
{
  public class ApiReportGenerator
  {
    public EndPointReportConfigResource Config;
    public const string XpathForWebOrAppConfigToRuntimeElement = "configuration/runtime";
    public const string AssemblyBindingNodeName = "assemblyBinding";
    public const string DependentAssemblyNodeName = "dependentAssembly";
    public const string AssemblyIdentityNodeName = "assemblyIdentity";
    public const string BindingRedirectNodeName = "bindingRedirect";
    public const string AssemblyIdentityNameAttribute = "name";
    public const string AssemblyIdentityPublicKeyAttribute = "publicKeyToken";
    public const string BindingRedirectVersionAttribute = "newVersion";

    public void Generate(FileInfo configResourceFileInfo)
    {
      Config = ResourceHelper.GenerateResourceFromFile(configResourceFileInfo);
      var webOrAppConfigFilename =
        GenerateWebOrAppConfigFromConfiguration(Config.SourceFilename, Config.ConfigFileRelativeToInstall);
      var redirectAssembliesNeeded = GenerateRedirectAssembliesList(webOrAppConfigFilename);
      ApplyRedirectAssemblies(redirectAssembliesNeeded);
      var controllers = GetControllersFromAssembly();
      Trace.WriteLine("Pre-Report: " + Config.SourceFilenameOnly + " :Controller Count:" + controllers.Count());
      var report = GenerateReport(controllers);
      SaveReportToOutputFile(report);
    }

    public string GenerateWebOrAppConfigFromConfiguration(string sourceFilename, string configFileRelativeToInstall)
    {
      var sourceFilenamePath = Path.GetDirectoryName(Config.SourceFilename);
      var webOrAppConfigFilename = Path.Combine(sourceFilenamePath, Config.ConfigFileRelativeToInstall);
      return webOrAppConfigFilename;
    }

    public IEnumerable<AssemblyNeeded> GenerateRedirectAssembliesList(string webOrAppConfigFilename)
    {
      var configFile = new XmlDocument();
      configFile.Load(webOrAppConfigFilename);
      var runtimeList = configFile.SelectNodes(XpathForWebOrAppConfigToRuntimeElement);

      var assemblyNeededList = new List<AssemblyNeeded>();

      for (int x = 0; x < runtimeList.Count; x++)
      {
        for (int z = 0; z < runtimeList.Item(x).ChildNodes.Count; z++)
        {
          var isAssemblyBindingNode = runtimeList.Item(x).ChildNodes[z].Name == AssemblyBindingNodeName;
          if (isAssemblyBindingNode)
          {
            var assemblyBindingItem = runtimeList.Item(x).ChildNodes[z];
            for (int y = 0; y < assemblyBindingItem.ChildNodes.Count; y++)
            {
              var isDependentAssemblyNode = assemblyBindingItem.ChildNodes[y].Name == DependentAssemblyNodeName;
              if (isDependentAssemblyNode)
              {
                var dependentAssemblyItem = assemblyBindingItem.ChildNodes[y];
                var assemblyIdentityPointer = -1;
                var bindingRedirectPointer = -1;
                for (int i = 0; i < dependentAssemblyItem.ChildNodes.Count; i++)
                {
                  var thisChildIsTheAssemblyIdentityNode =
                    dependentAssemblyItem.ChildNodes[i].Name == AssemblyIdentityNodeName;
                  if (thisChildIsTheAssemblyIdentityNode)
                  {
                    assemblyIdentityPointer = i;
                  }

                  var thisChildIsTheBindingRedirect = dependentAssemblyItem.ChildNodes[i].Name == BindingRedirectNodeName;
                  if (thisChildIsTheBindingRedirect)
                  {
                    bindingRedirectPointer = i;
                  }
                }

                var validEntryToAdd = bindingRedirectPointer > -1 && assemblyIdentityPointer > -1;
                if (validEntryToAdd)
                {
                  var xmlAssemblyIdentityAttributeCollection =
                    dependentAssemblyItem.ChildNodes[assemblyIdentityPointer].Attributes;
                  var xmlBindingRedirectAttributeCollection =
                    dependentAssemblyItem.ChildNodes[bindingRedirectPointer].Attributes;
                  var validEntryAvailable = xmlAssemblyIdentityAttributeCollection != null &&
                                            xmlBindingRedirectAttributeCollection != null;
                  if (validEntryAvailable)
                  {
                    var assemblyNeeded = new AssemblyNeeded()
                    {
                      Name = xmlAssemblyIdentityAttributeCollection[AssemblyIdentityNameAttribute].Value,
                      PublicKey = xmlAssemblyIdentityAttributeCollection[AssemblyIdentityPublicKeyAttribute]
                        .Value,
                      Version = xmlBindingRedirectAttributeCollection[BindingRedirectVersionAttribute].Value,
                    };
                    assemblyNeededList.Add(assemblyNeeded);
                  }
                }
              }
            }
          }
        }
      }
      return assemblyNeededList;
    }

    private void ApplyRedirectAssemblies(IEnumerable<AssemblyNeeded> assembliesNeeded)
    {

      foreach (var assemblyNeeded in assembliesNeeded)
      {
        var version = new Version(assemblyNeeded.Version);
        AssemblyRedirectHandler.RedirectAssembly(assemblyNeeded.Name, version, assemblyNeeded.PublicKey);
      }
    }

    private IEnumerable<Type> GetControllersFromAssembly()
    {
      var apiClasses = Assembly.LoadFrom(Config.SourceFilename)
        .GetTypes()
        .Where(t => t.IsNotCompilerGenerated() && !t.IsEnum && t.IsClass);

      IEnumerable<Type> controllers = GetAllRelevantControllerClasses(apiClasses, Config.ControllerNamespace, Config.BaseControllerName);
      return controllers;
    }

    private ApiEndPointReport GenerateReport(IEnumerable<Type> controllers)
    {
      var report = new ApiEndPointReport();

      foreach (var controller in controllers)
      {
        var controllerMethods = controller.GetAllMethodsCreatedByHumans(Config.ControllerNamespace)
          .Where(i => i.IsPublic && !i.DeclaringType.FullName.Contains(Config.HelpPageControllerNamespaceToAvoid));

        foreach (var myMethod in controllerMethods)
        {
          var newRoutes = GenerateRouteEntriesFromMethod(myMethod);
          report.Rows.AddRange(newRoutes);
        }
      }

      report.Rows.Sort((x, y) =>
      {
        if (x == null) throw new ArgumentNullException(nameof(x));
        return String.Compare(x.Route, y.Route, StringComparison.Ordinal);
      });

      return report;
    }

    private IList<ApiEndPointReportRow> GenerateRouteEntriesFromMethod(MethodInfo myMethod)
    {
      var newRoutes = new List<ApiEndPointReportRow>();

      var methodHasRouteeAttribute = myMethod.GetCustomAttributes(typeof(RouteAttribute), false).Length > 0;
      if (methodHasRouteeAttribute)
      {
        var isIgnoredByAutoDoc = GetIsIgnoredByAutoDoc(myMethod);

        var methodSignature = GenerateMethodSignature(myMethod);

        var routeAttributes = myMethod.GetCustomAttributes(typeof(RouteAttribute), false);
        foreach (var routeAttribute in routeAttributes)
        {
          var routeAttributeCast = (RouteAttribute)routeAttribute;
          var newEntry = new ApiEndPointReportRow
          {
            Route = routeAttributeCast.Template,
            MethodSignature = methodSignature,
            IgnoreFromAutoDocs = isIgnoredByAutoDoc
          };
          newRoutes.Add(newEntry);
        }
      }
      return newRoutes;
    }

    private static bool GetIsIgnoredByAutoDoc(MethodInfo myMethod)
    {
      bool isIgnoredByAutoDoc = false;
      var ignoreAttributes = myMethod.GetCustomAttributes(typeof(ApiExplorerSettingsAttribute), false);
      var hasIgnoreAttributes = ignoreAttributes.Length > 0;
      if (hasIgnoreAttributes)
      {
        isIgnoredByAutoDoc = ((ApiExplorerSettingsAttribute) ignoreAttributes[0]).IgnoreApi == true;
      }

      return isIgnoredByAutoDoc;
    }

    private void SaveReportToOutputFile(ApiEndPointReport report)
    {
      using (var writer = new StreamWriter(Config.OutputFilename, false))
      {
        foreach (var route in report.Rows)
        {
          writer.WriteLine(route.Route + "|" + route.MethodSignature + "|" + route.IgnoreFromAutoDocs);
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

    private static string GenerateMethodSignature(MethodInfo mi)
    {
      String[] param = mi.GetParameters()
        .Select(p => $"{p.ParameterType.Name} {p.Name}")
        .ToArray();

      string signature = $"{mi.Name}({String.Join(",", param)})";

      return signature;
    }
  }
}
