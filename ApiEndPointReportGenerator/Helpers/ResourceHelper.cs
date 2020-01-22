using Core.Generators;
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

      using (var resourceStreamReader = new StreamReader(resourceFileInfo.FullName))
      {
        var json = resourceStreamReader.ReadToEnd();
        var resource = JsonConvert.DeserializeObject<EndPointReportConfigResource>(json);
        return resource;
      }
    }
  }
}
