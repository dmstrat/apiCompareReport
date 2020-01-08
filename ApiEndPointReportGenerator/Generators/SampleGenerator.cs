using System.IO;
using Core.Resources;
using Newtonsoft.Json;

namespace ApiEndPointReportGenerator.Generators
{
  internal static class SampleGenerator
  {
    public static void GenerateEndPointReportConfigResource()
    {
      var resource = new EndPointReportConfigResource();
      resource.SourceFilename = "C:\\output\\msiPrev\\files\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\RetailWebApi.dll";
      resource.OutputFilename = "c:\\output\\retailwebapi.rptx";
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
    }
  }
}
