using System.Collections.Generic;

namespace Core.Resources
{
    public class EndPointReportConfigResource
    {

      public EndPointReportConfigResource()
      {
        AssembliesNeeded = new List<AssemblyNeeded>();
      }

      public string SourceFilename { get; set; } //retailwebapi.dll 
      public string OutputFilename { get; set; } //retailwebapi.rptx
      public string ControllerNamespace { get; set; } //RP.RetailWebApi
      public string HelpPageControllerNamespaceToAvoid { get; set; } //RP.RetailWebApi.Areas.HelpPage
    public string BaseControllerName { get; set; } //BaseWebRetailWebApiController
    public IList<AssemblyNeeded> AssembliesNeeded { get; set; }


    }

    public class AssemblyNeeded
    {
      public string Name { get; set; }
      public string Version { get; set; }
      public string PublicKey { get; set; }

    }
}
