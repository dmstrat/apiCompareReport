using EndPointCompare.Resources;
using Newtonsoft.Json;
using System.IO;

namespace EndPointCompare.Generators
{
  internal static class SampleGenerator
  {
    public static EndPointCompareConfigResource CreateEndPointCompareConfigResourceSample()
    {

      var retailWebApiPair = new EndPointReportConfigPairResource
      {
        Persona = "RetailWebApi",
        DeprecatedEndPointReportFilename = "c:\\output\\retailwebapi_deprecated.rpt",
        NewEndPointReportFilename= "c:\\output\\reatailwebapi_new.rpt",
        ResourceOld = Core.Generators.SampleGenerator.GenerateEndPointReportConfigResource("retailwebapi_old.rptx"),
        ResourceNew = Core.Generators.SampleGenerator.GenerateEndPointReportConfigResource("retailwebapi_new.rptx")
      };
      var resource = new EndPointCompareConfigResource
      {
        Msi_Old = "C:\\output\\Retail2019.1.0.1.msi",
        Msi_New = "C:\\output\\Retail2020.2.0.0.msi",
        ApiReportGeneratorExe =
          //"C:\\src\\dmstrat\\apiCompareReport\\ApiEndPointReportGenerator\\bin\\Debug\\ApiEndPointReportGenerator.exe"
          "C:\\src\\dmstrat\\apiCompareReport\\ApiEndPointReportGenerator\\bin\\Debug\\ApiEndPointReportGenerator.exe"
      };
      resource.PersonaPairs.Add(retailWebApiPair);

      using (StreamWriter sw = File.CreateText(@"C:\output\sampleEndPointCompareConfigResource.json"))
      {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(sw, resource);
      }
      return resource;
    }
  }
}
