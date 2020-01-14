using ApiEndPointReportGenerator.Generators;
using Core.Helpers;
using System.Diagnostics;

namespace ApiEndPointReportGenerator
{
  class Program
  {
    private const string _Module = "ApiEndPointReportGenerator ";
    static void Main(string[] args)
    {
      //only arg should be the Resource File to accommodate this execution. 
      var resourceFileInfo = FileHelper.GetFileInfo(args[0]);
      Trace.WriteLine("{" + _Module + "} - Processing Api using config file: " + resourceFileInfo.FullName);
      var generator = new ApiReportGenerator();
      generator.Generate(resourceFileInfo);
      Trace.WriteLine("{" + _Module + "} - Api Report completed.");
#if DEBUG
      //Console.WriteLine("Press any key to continue...");
      //Console.ReadKey();
#endif
    }
  }
}
