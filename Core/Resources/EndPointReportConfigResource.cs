﻿namespace Core.Resources
{
  public class EndPointReportConfigResource
  {
    public string SourceFilenameOnly { get; set; } //retailwebapi.dll
    public string SourceFilename { get; set; } //<tempDirectory>\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\retailwebapi.dll 
    public string InstallationDirectory { get; set; } //\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\RetailWebAPI\\bin\\
    public string ConfigFileRelativeToInstall { get; set; } // ..\\web.config
    public string EndpointReportFilename { get; set; } //<knownLocation>\\retailwebapi.rptx
    public string ControllerNamespace { get; set; } //RP.RetailWebApi
    public string HelpPageControllerNamespaceToAvoid { get; set; } //RP.RetailWebApi.Areas.HelpPage
    public string BaseControllerName { get; set; } //BaseWebRetailWebApiController
  }
}
