using EndPointCompare.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;

namespace EndPointCompare.Generators
{
  internal class ApiCompareGenerator
  {
    public void Generate()
    {
      var baseLeftFilename = "C:\\output\\msiPrev\\files\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\";
      var baseRightFilename = "C:\\output\\msiNew\\files\\RedPrairieRetail\\DefaultInstance\\Personae\\IIS\\";
      var leftRootFoldername = baseLeftFilename + "RetailWebAPI\\bin\\";
      var leftRootFolderInfo = FileHelper.GetDirectoryInfo(leftRootFoldername);
      var rightRootFoldername = baseRightFilename + "RetailWebAPI\\bin\\";
      var rightRootFolderInfo = FileHelper.GetDirectoryInfo(rightRootFoldername);
      var leftFilename = leftRootFoldername + "retailwebapi.dll";
      var leftConfigFilename = baseLeftFilename + "RetailWebAPI\\web.config";
      var leftConfigFileInfo = FileHelper.GetFileInfo(leftConfigFilename);
      var rightFilename = rightRootFoldername + "retailwebapi.dll";
      var rightConfigFilename = baseRightFilename + "RetailWebAPI\\web.config";
      var rightConfigFileInfo = FileHelper.GetFileInfo(rightConfigFilename);
      var leftFileInfo = FileHelper.GetFileInfo(leftFilename);
      var leftFileRootInfo = FileHelper.GetDirectoryInfo(leftRootFoldername);
      Trace.WriteLine("leftFile:Exists=" + leftFileInfo.Exists + ":" + leftFileInfo.FullName);
      var rightFileInfo = FileHelper.GetFileInfo(rightFilename);
      var rightFileRootInfo = FileHelper.GetDirectoryInfo(rightRootFoldername);
      Trace.WriteLine("rightFile:Exists=" + rightFileInfo.Exists + ":" + rightFileInfo.FullName);
      var generator = new ApiReportGenerator();
      var leftFileReport = generator.Generate(leftFileInfo, leftRootFolderInfo, leftConfigFileInfo);
      Trace.WriteLine("leftFile Report Generated:" + leftFileReport.FullName);
      generator = new ApiReportGenerator();
      var rightFileReport = generator.Generate(rightFileInfo, rightRootFolderInfo, rightConfigFileInfo); 
      Trace.WriteLine("rightFile Report Generated:" + rightFileReport.FullName);

      //do compare
      var deprecatedReport = GenerateDeprecatedEndPointsReport(leftFileReport, rightFileReport);
      Trace.WriteLine("Deprecated Endpoints Report Generated:" + deprecatedReport.FullName);
      //var newReport = GenerateNewEndPointsReport(leftFileReport, rightFileReport);
      //Trace.WriteLine("New Endpoints Report Generated:" + newReport.FullName);

    }

    private FileInfo GenerateDeprecatedEndPointsReport(FileInfo leftFileReport, FileInfo rightFileReport)
    {
      var leftVersionBaseRoute = "~/api/v1/";//"-beta6/";
      var rightVersionBaseRoute = "~/api/v1/";
      string leftLineOriginal, leftLine, rightLine;

      var deprecatedReport = new FileInfo(Path.GetTempFileName());
      //loop through each left record trying to find a match in the right, no match means deprecated route/method
      using (StreamWriter reportStream = new StreamWriter(deprecatedReport.FullName,true))
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
