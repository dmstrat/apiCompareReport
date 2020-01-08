using System.IO;
using Core.Resources;
using Newtonsoft.Json;

namespace Core.Generators
{
  public static class SampleGenerator
  {
    public static EndPointReportConfigResource GenerateEndPointReportConfigResource(string reportName = "retailwebapi.rptx")
    {
      var resource = new EndPointReportConfigResource();
      resource.SourceFilenameOnly = "RetailWebApi.dll";
      resource.SourceFilename = "c:\\output\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\RetailWebApi.dll";
      resource.InstallationDirectory = "RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\";
      resource.OutputFilename = "c:\\output\\" + reportName;
      resource.ControllerNamespace = "RP.RetailWebApi";
      resource.HelpPageControllerNamespaceToAvoid = "RP.RetailWebApi.Areas.HelpPage";
      resource.BaseControllerName = "BaseWebRetailWebApiController";
      var assemblySystemWebHttp = new AssemblyNeeded()
      {
        Name = "System.Web.Http",
        Version = "5.2.3.0",
        PublicKey = "31bf3856ad364e35"
      };
      resource.AssembliesNeeded.Add(assemblySystemWebHttp);

      var assemblySystemNetHttpFormatting = new AssemblyNeeded()
      {
        Name = "System.Net.Http.Formatting",
        Version = "5.2.3.0",
        PublicKey = "31bf3856ad364e35"
      };
      resource.AssembliesNeeded.Add(assemblySystemNetHttpFormatting);
      using (StreamWriter sw = File.CreateText(@"C:\output\sampleEndPointReportResource.json"))
      {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(sw, resource);
      }

      return resource;
    }
  }
}
