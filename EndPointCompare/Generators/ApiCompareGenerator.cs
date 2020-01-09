using EndPointCompare.Helpers;
using EndPointCompare.Resources;
using System;
using System.Diagnostics;
using System.IO;

namespace EndPointCompare.Generators
{
  internal class ApiCompareGenerator
  {
    private EndPointCompareConfigResource _Config { get; set; }
    private string _OldMsiFolderExtractionPoint;
    private string _NewMsiFolderExtractionPoint;

    public void Generate() //EndPointCompareConfigResource resource)
    {
      _Config = SampleGenerator
        .CreateEndPointCompareConfigResourceSample(); // new EndPointCompareConfigResource();//TODO: Fix!!! resource;

      var baseOutputDirectory = "c:\\output\\";
      FileHelper.EnsureDirectoryExists(baseOutputDirectory);

      //pre-extraction setup
      _OldMsiFolderExtractionPoint = FileHelper.GenerateTempDirectoryInOutput();
      Trace.WriteLine("Old MSI Extraction point: " + _OldMsiFolderExtractionPoint);
      _NewMsiFolderExtractionPoint = FileHelper.GenerateTempDirectoryInOutput();
      Trace.WriteLine("New MSI Extraction point: " + _NewMsiFolderExtractionPoint);

      var oldMsiFilename = _Config.msi_old;
      var newMsiFilename = _Config.msi_new;
      //extract msi to temp directories
      var pe = new ProcessExecutor();
      pe.ExtractMsiToDirectory(oldMsiFilename, _OldMsiFolderExtractionPoint);
      pe.ExtractMsiToDirectory(newMsiFilename, _NewMsiFolderExtractionPoint);

      var generatorExeFilename = _Config.ApiReportGeneratorExe;
      foreach (var personaPair in _Config.PersonaPairs)
      {
        Trace.WriteLine("Processing :" + personaPair.Persona + ":");
        Trace.WriteLine("Developing Api Endpoint Reports...");
        var oldFilename = Path.Combine(_OldMsiFolderExtractionPoint, personaPair.Resource_Old.InstallationDirectory,
          personaPair.Resource_Old.SourceFilenameOnly);
        var oldFileConfig = personaPair.Resource_Old;
        oldFileConfig.SourceFilename = oldFilename;
        var oldOutputFilename = FileHelper.CreateTempFile();
        oldFileConfig.OutputFilename = oldOutputFilename.FullName;
        var oldFileConfigFilename = FileHelper.CreateTempFile();
        FileHelper.SaveAsJson(oldFileConfig, oldFileConfigFilename.FullName);
        pe.ExecuteEndPointReporter(generatorExeFilename, oldFileConfigFilename);


        var newFilename = Path.Combine(_NewMsiFolderExtractionPoint, personaPair.Resource_New.InstallationDirectory,
          personaPair.Resource_New.SourceFilenameOnly);
        var newFileConfig = personaPair.Resource_New;
        newFileConfig.SourceFilename = newFilename;
        var newOutputFilename = FileHelper.CreateTempFile();
        newFileConfig.OutputFilename = newOutputFilename.FullName;
        var newFileConfigFilename = FileHelper.CreateTempFile();
        FileHelper.SaveAsJson(newFileConfig, newFileConfigFilename.FullName);
        pe.ExecuteEndPointReporter(generatorExeFilename, newFileConfigFilename);

        Trace.WriteLine("Developing Deprecated and New Endpoint Reports...");
        Trace.WriteLine("Old Api Report:" + oldOutputFilename);
        Trace.WriteLine("New Api Report: " + newOutputFilename);
        //do compare
        var deprecatedReport = GenerateDeprecatedEndPointsReport(oldOutputFilename, newOutputFilename);
        Trace.WriteLine("Deprecated Endpoints Report Generated:" + deprecatedReport.FullName);
        var newReport = GenerateNewEndPointsReport(oldOutputFilename, newOutputFilename);
        Trace.WriteLine("New Endpoints Report Generated:" + newReport.FullName);

        Trace.WriteLine("...Completed");
      }
    }

    private FileInfo GenerateNewEndPointsReport(FileInfo oldFileReport, FileInfo newFileReport)
    {
      var oldVersionBaseRoute = "~/api/v1-beta6/";
      var newVersionBaseRoute = "~/api/v1/";
      string newLineOriginal, newLine, oldLine;

      var newReport = new FileInfo(Path.GetTempFileName());
      //loop through each new record trying to find a match in the old, no match means new route/method
      using (StreamWriter reportStream = new StreamWriter(newReport.FullName, true))
      {
        using (StreamReader newStream = new StreamReader(newFileReport.FullName))
        {
          while ((newLineOriginal = newStream.ReadLine()) != null)
          {
            newLine = StripBaseRouteFromString(newLineOriginal, newVersionBaseRoute);// leftLineOriginal.Substring(leftVersionBaseRoute.Length);
            using (StreamReader oldStream = new StreamReader(oldFileReport.FullName))
            {
              while ((oldLine = oldStream.ReadLine()) != null)
              {
                oldLine = StripBaseRouteFromString(oldLine, oldVersionBaseRoute);
                var bothLinesMatch = newLine.Equals(oldLine, StringComparison.InvariantCultureIgnoreCase);
                if (bothLinesMatch)
                {
                  break;
                }

              }
              var noMatchFound = oldLine == null;
              if (noMatchFound)
              {
                reportStream.WriteLine(newLineOriginal);
              }
            }
          }
        }
      }
      return newReport;
    }

    private FileInfo GenerateDeprecatedEndPointsReport(FileInfo leftFileReport, FileInfo rightFileReport)
    {
      var leftVersionBaseRoute = "~/api/v1-beta6/";
      var rightVersionBaseRoute = "~/api/v1/";
      string leftLineOriginal, leftLine, rightLine;

      var deprecatedReport = new FileInfo(Path.GetTempFileName());
      //loop through each left record trying to find a match in the right, no match means deprecated route/method
      using (StreamWriter reportStream = new StreamWriter(deprecatedReport.FullName, true))
      {
        using (StreamReader leftStream = new StreamReader(leftFileReport.FullName))
        {
          while ((leftLineOriginal = leftStream.ReadLine()) != null)
          {
            leftLine = StripBaseRouteFromString(leftLineOriginal, leftVersionBaseRoute);// leftLineOriginal.Substring(leftVersionBaseRoute.Length);
            using (StreamReader rightStream = new StreamReader(rightFileReport.FullName))
            {
              while ((rightLine = rightStream.ReadLine()) != null)
              {
                rightLine = StripBaseRouteFromString(rightLine, rightVersionBaseRoute);
                var bothLinesMatch = leftLine.Equals(rightLine, StringComparison.InvariantCultureIgnoreCase);
                if (bothLinesMatch)
                {
                  break;
                }
              }
              var noMatchFound = rightLine == null;
              if (noMatchFound)
              {
                reportStream.WriteLine(leftLineOriginal);
              }
            }
          }
        }
      }
      return deprecatedReport;
    }

    private static string StripBaseRouteFromString(string sourceLine, string baseRouteToStripOut)
    {
      var sourceLongEnough = sourceLine.Length > baseRouteToStripOut.Length;
      if (sourceLongEnough)
      {
        var hasBaseRoute = sourceLine.Substring(0, baseRouteToStripOut.Length).Equals(baseRouteToStripOut);
        if (hasBaseRoute)
        {
          return sourceLine.Substring(baseRouteToStripOut.Length);
        }
      }
      return sourceLine;
    }
  }
}
