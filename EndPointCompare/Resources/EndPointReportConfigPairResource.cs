using Core.Resources;

namespace EndPointCompare.Resources
{
  public class EndPointReportConfigPairResource
  {
    public string Persona { get; set; }
    public string DeprecatedEndPointReportFilename { get; set; } 
    public string NewEndPointReportFilename { get; set; }        

    public EndPointReportConfigResource ResourceOld { get; set; }
    public EndPointReportConfigResource ResourceNew { get; set; }
  }
}