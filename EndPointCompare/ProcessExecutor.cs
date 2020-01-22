using System;
using System.Diagnostics;
using System.IO;

namespace EndPointCompare
{
  internal class ProcessExecutor : IProcessExecutor
  {
    public void ExecuteEndPointReporter(string apiReportGeneratorExeFilename, FileInfo configFilename)
    {
      var generatorFileInfo = new FileInfo(apiReportGeneratorExeFilename);
      if (generatorFileInfo.Exists)
      {
        ExecuteAndWait(generatorFileInfo.FullName, configFilename.FullName);
      }
      else
      {
        Trace.WriteLine("Report Generator NOT at: " + apiReportGeneratorExeFilename);
      }
    }

    public void ExtractMsiToDirectory(string msiFilename, string targetDirectory)
    {
      var executable = "msiexec";
      var arguments = " /a " + msiFilename + " /qb TARGETDIR=" + targetDirectory;
      ExecuteAndWait(executable, arguments);
    }

    public void ExecuteAndWait(string sourceFilename, string arguments)
    {
      try
      {
        var process = new Process
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = sourceFilename,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
          }
        };

        process.Start();

        while (!process.StandardOutput.EndOfStream)
        {
          var line = process.StandardOutput.ReadLine();
          Trace.WriteLine(line);
        }

        process.WaitForExit();
      }

      catch (Exception e)
      {
        Trace.WriteLine(e);
        throw;
      }
    }
  }
}
