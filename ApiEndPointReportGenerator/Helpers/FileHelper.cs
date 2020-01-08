using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace ApiEndPointReportGenerator.Helpers
{
  public static class FileHelper
  {
    public static FileInfo CreateTempFile()
    {
      var filename = Path.GetTempFileName();
      var fileInfo = new FileInfo(filename);
      return fileInfo;
    }

    public static DirectoryInfo GetDirectoryInfo(string directory)
    {
      try
      { 
        var directoryInfo = new DirectoryInfo(directory);
        Trace.WriteLine("DirectoryInfo generated:Exists:" + directoryInfo.Exists + ":" + directoryInfo.FullName);
        return directoryInfo;
      }
      catch (ArgumentNullException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (SecurityException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (ArgumentException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (PathTooLongException e)
      {
        Trace.WriteLine(e.Message);
      }
      return null;
    }

    public static FileInfo GetFileInfo(string filename)
    {
      try
      {
        var fileInfo = new FileInfo(filename);
        Trace.WriteLine("FileInfo generated:Exists:" + fileInfo.Exists + ":" + fileInfo.FullName);
        return fileInfo;
      }
      catch (ArgumentNullException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (SecurityException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (ArgumentException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (UnauthorizedAccessException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (PathTooLongException e)
      {
        Trace.WriteLine(e.Message);
      }
      catch (NotSupportedException e)
      {
        Trace.WriteLine(e.Message);
      }
      return null;
    }
  }
}
