using EndPointCompare.Helpers;
using EndPointCompare.Resources;
using System;
using System.Diagnostics;
using System.IO;
using Core.Helpers;
using FileHelper = EndPointCompare.Helpers.FileHelper;

namespace EndPointCompare.Generators
{
  public class ApiCompareGenerator
  {
    private EndPointCompareConfigResource _Config { get; set; }
    private string _OldMsiFolderExtractionPoint;
    private string _NewMsiFolderExtractionPoint;

    public const string BaseRouteWithoutVersion = "~/api/";

    public void Generate(string configFilename) 
    {
      var configFileInfo = new FileInfo(configFilename);
      var config = FileHelper.LoadJsonFile(configFileInfo);
      _Config = config;
      //_Config = SampleGenerator
        //.CreateEndPointCompareConfigResourceSample(); 

      var baseOutputDirectory = "c:\\output\\";
      FileHelper.EnsureDirectoryExists(baseOutputDirectory);

      //pre-extraction setup
      _OldMsiFolderExtractionPoint = FileHelper.GenerateTempDirectoryInOutput();
      _Config.Msi_Old_ExtractionPoint = _OldMsiFolderExtractionPoint;
      Trace.WriteLine("Old MSI Extraction point: " + _OldMsiFolderExtractionPoint);
      _NewMsiFolderExtractionPoint = FileHelper.GenerateTempDirectoryInOutput();
      _Config.Msi_New_ExtractionPoint = _NewMsiFolderExtractionPoint;
      Trace.WriteLine("New MSI Extraction point: " + _NewMsiFolderExtractionPoint);

      //_OldMsiFolderExtractionPoint = config.Msi_Old_ExtractionPoint;
      //_NewMsiFolderExtractionPoint = config.Msi_New_ExtractionPoint;

      var oldMsiFilename = _Config.Msi_Old;
      var newMsiFilename = _Config.Msi_New;
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
        Core.Helpers.FileHelper.SaveAsJson(oldFileConfig, oldFileConfigFilename.FullName);
        pe.ExecuteEndPointReporter(generatorExeFilename, oldFileConfigFilename);

        var newFilename = Path.Combine(_NewMsiFolderExtractionPoint, personaPair.Resource_New.InstallationDirectory,
          personaPair.Resource_New.SourceFilenameOnly);
        var newFileConfig = personaPair.Resource_New;
        newFileConfig.SourceFilename = newFilename;
        var newOutputFilename = FileHelper.CreateTempFile();
        newFileConfig.OutputFilename = newOutputFilename.FullName;
        var newFileConfigFilename = FileHelper.CreateTempFile();
        Core.Helpers.FileHelper.SaveAsJson(newFileConfig, newFileConfigFilename.FullName);
        pe.ExecuteEndPointReporter(generatorExeFilename, newFileConfigFilename);

        Trace.WriteLine("Developing Deprecated and New Endpoint Reports...");
        Trace.WriteLine("Old Api Report:" + oldOutputFilename);
        Trace.WriteLine("New Api Report: " + newOutputFilename);
        //do compare
        var deprecatedReportFilename = personaPair.DeprecatedEndPointReportFilename;
        var deprecatedReport = GenerateDeprecatedEndPointsReport(oldOutputFilename, 
                                                                       newOutputFilename, 
                                                                       deprecatedReportFilename );
        Trace.WriteLine("Deprecated Endpoints Report Generated:" + deprecatedReport.FullName);
        var newReportFilename = personaPair.NewEndPointReportFilename;
        var newReport = GenerateNewEndPointsReport(oldOutputFilename, newOutputFilename, newReportFilename);
        Trace.WriteLine("New Endpoints Report Generated:" + newReport.FullName);

        Trace.WriteLine("...Completed");
      }
    }

    private FileInfo GenerateDeprecatedEndPointsReport(FileInfo leftFileReport, FileInfo rightFileReport, string outputReportFilename)
    {
      return GenerateReportWhereLeftEntriesMissingFromRightEntries(leftFileReport, rightFileReport,
        outputReportFilename);
    }

    private FileInfo GenerateNewEndPointsReport(FileInfo oldFileReport, FileInfo newFileReport, string outputReportFilename)
    {
      return GenerateReportWhereLeftEntriesMissingFromRightEntries(newFileReport, oldFileReport,
        outputReportFilename);
    }

    private FileInfo GenerateReportWhereLeftEntriesMissingFromRightEntries(FileInfo leftFileInfo,
      FileInfo rightFileInfo, string outputReportFilename)
    {
      string leftLineOriginal, leftLine, rightLine;

      var deprecatedReport = new FileInfo(outputReportFilename);
      //loop through each left record trying to find a match in the right, no match means deprecated route/method
      using (StreamWriter reportStream = new StreamWriter(deprecatedReport.FullName, false))
      {
        using (StreamReader leftStream = new StreamReader(leftFileInfo.FullName))
        {
          while ((leftLineOriginal = leftStream.ReadLine()) != null)
          {
            leftLine = StripBaseRouteFromString(leftLineOriginal);
            using (StreamReader rightStream = new StreamReader(rightFileInfo.FullName))
            {
              while ((rightLine = rightStream.ReadLine()) != null)
              {
                rightLine = StripBaseRouteFromString(rightLine);
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

    public static string StripBaseRouteFromString(string sourceLine)
    {
      var pointerAfterBaseRouteInString = sourceLine.IndexOf(BaseRouteWithoutVersion, StringComparison.CurrentCultureIgnoreCase);

      var hasBaseRouteInSourceLine = pointerAfterBaseRouteInString > -1;
      if (hasBaseRouteInSourceLine)
      {
        var pointAfterVersion =
          sourceLine.IndexOf("/", BaseRouteWithoutVersion.Length, StringComparison.CurrentCultureIgnoreCase);
        var pointPositionIsValid = pointAfterVersion > 0;
        if (pointPositionIsValid)
        {
          return sourceLine.Substring(pointAfterVersion);
        }
      }

      return sourceLine;
    }
  }
}
