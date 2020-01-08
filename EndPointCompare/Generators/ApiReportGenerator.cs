using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using EndPointCompare.Helpers;

namespace EndPointCompare.Generators
{
  internal class ApiReportGenerator
  {

    public FileInfo Generate(FileInfo sourceFileInfo,
                                     DirectoryInfo sourceRootDirectoryInfo,
                                     FileInfo configFileInfo)
    {
      string controllerNamespace = "RP.RetailWebApi";
      string baseControllerName = "BaseWebRetailWebApiController";
      string helpPageControllerNamespace = "RP.RetailWebApi.Areas.HelpPage";
      string outputFilename = Path.GetTempFileName();// "RetailWebApi_Endpoints.rptx";

      /*
      name = "System.Web.Http" publicKeyToken = "31bf3856ad364e35" culture = "neutral" newVersion = "5.2.3.0" 
      name = "System.Net.Http.Formatting" publicKeyToken = "31bf3856ad364e35" culture = "neutral" newVersion = "5.2.3.0" />
      */
      var version = new Version("5.2.3.0");
      AssemblyRedirectHandler.RedirectAssembly("System.Web.Http", version, "31bf3856ad364e35");
      AssemblyRedirectHandler.RedirectAssembly("System.Net.Http.Formatting", version, "31bf3856ad364e35");

      //var domainInfo = new AppDomainSetup();
      //domainInfo.ConfigurationFile = configFileInfo.FullName;
      //domainInfo.ApplicationBase = Environment.CurrentDirectory; // sourceRootDirectoryInfo.FullName;
      //domainInfo.ApplicationName = "RetailWebApi";


      //var adEvidence = AppDomain.CurrentDomain.Evidence;
      //var tempDomain = AppDomain.CreateDomain("TempDomain", adEvidence, domainInfo);

      //Trace.WriteLine("Host domain: " + AppDomain.CurrentDomain.FriendlyName);
      //Trace.WriteLine("Child domain: " + tempDomain.FriendlyName);
      //Trace.WriteLine("");
      //Trace.WriteLine("Configuration file: " + tempDomain.SetupInformation.ConfigurationFile);
      //Trace.WriteLine("Application Base Directory: " + tempDomain.BaseDirectory);

      //var type = typeof(Proxy);
      //var value = (Proxy)tempDomain.CreateInstanceAndUnwrap(
      //  type.Assembly.FullName,
      //  type.FullName);


      //var sourceFileAsBytes = File.ReadAllBytes(sourceFileInfo.FullName);
      //var assembly = value.GetAssembly(sourceFileInfo.FullName);
      //var apiClasses = tempDomain.Load(sourceFileAsBytes);
      //  .GetTypes()
      //  .Where(t => t.IsNotCompilerGenerated() && !t.IsEnum && t.IsClass);


      var sourceFileAsBytes = File.ReadAllBytes(sourceFileInfo.FullName);
      var apiClasses = Assembly.LoadFrom(sourceFileInfo.FullName)
      //var assembly = Assembly.Load(sourceFileAsBytes);
      //var apiClasses = assembly
      .GetTypes()
      .Where(t => t.IsNotCompilerGenerated() && !t.IsEnum && t.IsClass);

      IEnumerable<Type> controllers = GetAllRelevantControllerClasses(apiClasses, controllerNamespace, baseControllerName);

      var routes = new List<Tuple<string, string, bool>>();

      foreach (var controller in controllers)
      {
        var controllerMethods = controller.GetAllMethodsCreatedByHumans(controllerNamespace)
          .Where(i => i.IsPublic && !i.DeclaringType.FullName.Contains(helpPageControllerNamespace));

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
      //var basePath = AppDomain.CurrentDomain.BaseDirectory;
      //var returnFilename = basePath + "\\" + outputFilename;
      using (StreamWriter writer = new StreamWriter(outputFilename, false))
      {
        foreach (var route in routes)
        {
          writer.WriteLine(route.Item1 + "|" + route.Item2 + "|" + route.Item3);
        }

      }

      //AppDomain.Unload(tempDomain);

      var returnFileInfo = new FileInfo(outputFilename);
      return returnFileInfo;
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

      //string signature = $"{mi.ReturnType.Name} {mi.Name}({String.Join(",", param)})";
      string signature = $"{mi.Name}({String.Join(",", param)})";

      return signature;
    }

/*
    private static IEnumerable<Type> GenerateControllerListFromAssembly(FileInfo sourceFile, string controllerNamespace)
    {

      var enterpriseApiClasses = Assembly.LoadFrom(sourceFile.FullName) // GetAssembly(typeof(BaseRetailApiController))
        .GetTypes()
        .Where(t => t.IsNotCompilerGenerated() && !t.IsEnum && t.IsClass);

      var controllers = GetAllRelevantControllerClasses(enterpriseApiClasses, controllerNamespace);
      return controllers;
    }

    private static IEnumerable<Type> GetAllRelevantControllerClasses(IEnumerable<Type> enterpriseAdminApiClasses,
      string controllerNamespace)
    { 
      return enterpriseAdminApiClasses
        .Where(x => x.FullName.Contains(controllerNamespace)
          && x.Name.EndsWith("Controller") && !x.Name.Equals("BaseEpAdminApiController"))
        .ToList();
    } */
  } 
}
