using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using EndPointCompare.Resources;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace EndPointCompare.Helpers
{
  public static class FileHelper
  {
    public static EndPointCompareConfigResource LoadJsonFile(FileInfo executionConfigFileInfo)
    {
      EndPointCompareConfigResource configResource = new EndPointCompareConfigResource();
      Trace.Write($"Checking input file by loading to associated class....");
      try
      {
        using (var stream = new StreamReader(executionConfigFileInfo.FullName))
        {
          var ser = new DataContractJsonSerializer(typeof(EndPointCompareConfigResource),
            new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });
          configResource = ser.ReadObject(stream.BaseStream) as EndPointCompareConfigResource;
          var fileFailedToSerialize = configResource == null;
          if (fileFailedToSerialize)
          {
            throw new Exception("Failed to read json file to object");
          }
        }
      }
      catch (Exception e)
      {
        Trace.WriteLine("...Exception!");
        Trace.WriteLine(e);
        throw;
      }
      Trace.WriteLine("...Completed.");
      return configResource;
    }

    public static string GenerateTempDirectoryInOutput()
    {
      var newDirectory = "c:\\output\\" + Guid.NewGuid();
      var newTempDirectory = Directory.CreateDirectory(newDirectory);
      return newTempDirectory.FullName;
    }

    public static string GenerateTempDirectory()
    {
      var newDirectory = Path.GetTempPath() + Guid.NewGuid();
      var newTempDirectory = Directory.CreateDirectory(newDirectory);
      return newTempDirectory.FullName;
    }

    public static void EnsureDirectoryExists(string baseOutputDirectory)
    {
      var dirInfo = new DirectoryInfo(baseOutputDirectory);
      if (!dirInfo.Exists)
      {
        dirInfo.Create();
      }
    }


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
