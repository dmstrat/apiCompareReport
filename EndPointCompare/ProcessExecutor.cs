using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;

namespace EndPointCompare
{
  internal class ProcessExecutor
  {
    public void ExecuteEndPointReporter(FileInfo configFilename)
    {
      var currentLocation = AppDomain.CurrentDomain.BaseDirectory;
      var debugExecutableLocation = Path.Combine(currentLocation, "..\\..\\..\\", 
        "ApiEndPointReportGenerator\\bin\\Debug\\ApiEndPointReportGenerator.exe");
      var releaseExecutableLocation = Path.Combine(currentLocation, "..\\..\\..\\", 
        "ApiEndPointReportGenerator\\bin\\Release\\ApiEndPointReportGenerator.exe");
      var hasDebugExecutable = (new FileInfo(debugExecutableLocation)).Exists;
      var hasReleaseExecutable = (new FileInfo(releaseExecutableLocation)).Exists;
      string executable = "fail.exe";
      if (hasReleaseExecutable)
      {
        executable = releaseExecutableLocation;
      }else if (hasDebugExecutable)
      {
        executable = debugExecutableLocation;
      }
      ExecuteAndWait(executable, configFilename.FullName);
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
