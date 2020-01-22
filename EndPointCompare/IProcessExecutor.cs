using System.IO;

namespace EndPointCompare
{
  internal interface IProcessExecutor
  {
    void ExecuteEndPointReporter(string apiReportGeneratorExeFilename, FileInfo configFilename);
    void ExtractMsiToDirectory(string msiFilename, string targetDirectory);
    void ExecuteAndWait(string sourceFilename, string arguments);
  }
}