﻿using System.IO;
using Core.Resources;
using Newtonsoft.Json;

namespace Core.Generators
{
  public static class SampleGenerator
  {
    public static EndPointReportConfigResource GenerateEndPointReportConfigResource(string reportName = "retailwebapi.rptx")
    {
      var resource = new EndPointReportConfigResource
      {
        SourceFilenameOnly = "RetailWebApi.dll",
        SourceFilename =
          "c:\\output\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\RetailWebApi.dll",
        InstallationDirectory = "RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\",
        ConfigFileRelativeToInstall = "..\\web.config",
        EndpointReportFilename = "c:\\output\\" + reportName,
        ControllerNamespace = "RP.RetailWebApi",
        HelpPageControllerNamespaceToAvoid = "RP.RetailWebApi.Areas.HelpPage",
        BaseControllerName = "BaseWebRetailWebApiController"
      };

      using (StreamWriter sw = File.CreateText(@"C:\output\sampleEndPointReportResource.json"))
      {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(sw, resource);
      }
      
      return resource;
    }
  }
}
