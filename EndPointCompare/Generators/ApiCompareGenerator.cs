using Core.Resources;
using EndPointCompare.Resources;
using System;
using System.Diagnostics;
using System.IO;
using FileHelper = EndPointCompare.Helpers.FileHelper;

namespace EndPointCompare.Generators
{
  public class ApiCompareGenerator
  {
    private EndPointCompareConfigResource _Config;
    private string _OldMsiFolderExtractionPoint;
    private string _NewMsiFolderExtractionPoint;

    public const string BaseRouteWithoutVersion = "~/api/";

    public void Generate(string configFilename)
    {
      LoadConfigFile(configFilename);

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
        ProcessPersonaPair(personaPair, pe, generatorExeFilename);
        Trace.WriteLine("...Completed");
      }
    }

    private void LoadConfigFile(string configFilename)
    {
      var configFileInfo = new FileInfo(configFilename);
      var config = FileHelper.LoadJsonFile(configFileInfo);
      //_Config = SampleGenerator.CreateEndPointCompareConfigResourceSample(); 
      _Config = config;
    }

    private void ProcessPersonaPair(EndPointReportConfigPairResource personaPair, IProcessExecutor pe, string apiReportGeneratorExeFilename)
    {
      Trace.WriteLine("Developing Api Endpoint Reports...");
      var oldFilename = Path.Combine(_OldMsiFolderExtractionPoint, personaPair.ResourceOld.InstallationDirectory,
          personaPair.ResourceOld.SourceFilenameOnly);
      var oldReportFileInfo = GenerateApiReport(pe, apiReportGeneratorExeFilename, oldFilename, personaPair.ResourceOld);

      var newFilename = Path.Combine(_NewMsiFolderExtractionPoint, personaPair.ResourceNew.InstallationDirectory,
          personaPair.ResourceNew.SourceFilenameOnly);
      var newReportFileInfo = GenerateApiReport(pe, apiReportGeneratorExeFilename, newFilename, personaPair.ResourceNew);

      Trace.WriteLine("Developing Deprecated and New Endpoint Reports...");
      Trace.WriteLine("Old Api Report:" + oldReportFileInfo.FullName);
      Trace.WriteLine("New Api Report: " + newReportFileInfo.FullName);
      //do compare
      var deprecatedReportFilename = personaPair.DeprecatedEndPointReportFilename;
      var deprecatedReport = GenerateDeprecatedEndPointsReport(oldReportFileInfo,
          newReportFileInfo,
          deprecatedReportFilename);
      Trace.WriteLine("Deprecated Endpoints Report Generated:" + deprecatedReport.FullName);
      var newReportFilename = personaPair.NewEndPointReportFilename;
      var newReport = GenerateNewEndPointsReport(oldReportFileInfo, newReportFileInfo, newReportFilename);
      Trace.WriteLine("New Endpoints Report Generated:" + newReport.FullName);
    }

    private FileInfo GenerateApiReport(IProcessExecutor pe, string apiReportGeneratorExeFilename, string dllFilename, 
      EndPointReportConfigResource configResource)
    {
      configResource.SourceFilename = dllFilename;
      var oldOutputFilename = FileHelper.CreateTempFile();
      configResource.EndpointReportFilename = oldOutputFilename.FullName;
      var oldFileConfigFilename = FileHelper.CreateTempFile();
      Core.Helpers.FileHelper.SaveAsJson(configResource, oldFileConfigFilename.FullName);
      pe.ExecuteEndPointReporter(apiReportGeneratorExeFilename, oldFileConfigFilename);
      return oldOutputFilename;
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
      var outputReport = new FileInfo(outputReportFilename);
      //loop through each left record trying to find a match in the right, no match means deprecated route/method
      using (var reportStream = new StreamWriter(outputReport.FullName, false))
      {
        using (var leftStream = new StreamReader(leftFileInfo.FullName))
        {
          string leftLineOriginal;
          while ((leftLineOriginal = leftStream.ReadLine()) != null)
          {
            var leftLine = StripBaseRouteFromString(leftLineOriginal);
            using (var rightStream = new StreamReader(rightFileInfo.FullName))
            {
              string rightLine;
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
      return outputReport;
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
