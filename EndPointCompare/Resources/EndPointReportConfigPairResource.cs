using Core.Resources;

namespace EndPointCompare.Resources
{
  public class EndPointReportConfigPairResource
  {

    public EndPointReportConfigPairResource()
    {

    }
    public string Persona { get; set; }
    public string DeprecatedEndPointReportFilename { get; set; } 
    public string NewEndPointReportFilename { get; set; }        

    public EndPointReportConfigResource Resource_Old { get; set; }
    public EndPointReportConfigResource Resource_New { get; set; }
  }
}