using System.Collections.Generic;

namespace Core.Resources
{
  public class EndPointReportConfigResource
  {

    public EndPointReportConfigResource()
    {
      AssembliesNeeded = new List<AssemblyNeeded>();
    }

    public string SourceFilenameOnly { get; set; } //retailwebapi.dll
    public string SourceFilename { get; set; } //<tempDirectory>\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\retailwebapi.dll 
    public string InstallationDirectory { get; set; } //\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\
    public string ConfigFileRelativeToInstall { get; set; } // ..\\web.config
    public string OutputFilename { get; set; } //<knownLocation>\\retailwebapi.rptx
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
