using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EndPointCompare.Resources;
using Newtonsoft.Json;

namespace EndPointCompare.Generators
{
  internal static class SampleGenerator
  {
    public static EndPointCompareConfigResource CreateEndPointCompareConfigResourceSample()
    {

      var retailWebApiPair = new EndPointReportConfigPairResource();
      retailWebApiPair.Persona = "RetailWebApi";
      retailWebApiPair.Resource_Old = Core.Generators.SampleGenerator.GenerateEndPointReportConfigResource("retailwebapi_old.rptx");
      retailWebApiPair.Resource_New = Core.Generators.SampleGenerator.GenerateEndPointReportConfigResource("retailwebapi_new.rptx");
      var resource = new EndPointCompareConfigResource
      {
        msi_old = "C:\\output\\Retail2019.1.0.1.msi",
        msi_new = "C:\\output\\Retail2020.2.0.0.msi",
        ApiReportGeneratorExe = 
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
