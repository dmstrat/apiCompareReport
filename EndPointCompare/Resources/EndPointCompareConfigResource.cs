using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndPointCompare.Resources
{
  public class EndPointCompareConfigResource
  {
    public EndPointCompareConfigResource()
    {
      PersonaPairs = new List<EndPointReportConfigPairResource>();
    }

    public string ApiReportGeneratorExe { get; set; }
    public string msi_old { get; set; }
    public string msi_new { get; set; }
    public IList<EndPointReportConfigPairResource> PersonaPairs { get; set; }
  }
}
