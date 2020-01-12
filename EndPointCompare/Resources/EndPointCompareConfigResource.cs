using System.Collections.Generic;

namespace EndPointCompare.Resources
{
  public class EndPointCompareConfigResource
  {
    public EndPointCompareConfigResource()
    {
      PersonaPairs = new List<EndPointReportConfigPairResource>();
    }

    public string ApiReportGeneratorExe { get; set; }
    public string Msi_Old { get; set; }
    public string Msi_Old_ExtractionPoint { get; set; }
    public string Msi_New { get; set; }
    public string Msi_New_ExtractionPoint { get; set; }
    public IList<EndPointReportConfigPairResource> PersonaPairs { get; set; }
  }
}
