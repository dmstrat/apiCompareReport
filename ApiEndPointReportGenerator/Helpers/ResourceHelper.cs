﻿using Core.Generators;
using Core.Resources;
using Newtonsoft.Json;
using System.IO;

namespace ApiEndPointReportGenerator.Helpers
{
  internal static class ResourceHelper
  {
    public static EndPointReportConfigResource GenerateResourceFromFile(FileInfo resourceFileInfo)
    {
#if DEBUG
      SampleGenerator.GenerateEndPointReportConfigResource();
#endif

      using (StreamReader r = new StreamReader(resourceFileInfo.FullName))
      {
        var json = r.ReadToEnd();
        var config = JsonConvert.DeserializeObject<EndPointReportConfigResource>(json);
        return config;
      }
    }
  }
}
