using System.Collections.Generic;

namespace Core.Resources
{
  public class ApiEndPointReport
  {
    public ApiEndPointReport()
    {
      Rows = new List<ApiEndPointReportRow>();
      
    }
    public string ApiProject { get; set; }
    public List<ApiEndPointReportRow> Rows { get; set; }
  }
}
