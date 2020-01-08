using Core.Resources;

namespace EndPointCompare.Resources
{
  public class EndPointReportConfigPairResource
  {

    public EndPointReportConfigPairResource()
    {

    }
    public string Persona { get; set; }
    public EndPointReportConfigResource Resource_Old { get; set; }
    public EndPointReportConfigResource Resource_New { get; set; }
  }
}