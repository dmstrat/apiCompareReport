using ApiEndPointReportGenerator.Unit.Tests.Helpers;
using Core.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ApiEndPointReportGenerator.Unit.Tests.Generators
{
  [TestClass]
  public class WebConfigReaderTests
  {
    [TestMethod]
    [DeploymentItem("Sources\\Web.Config")]
    public void BuiltListOfAssembliesNeededIsCorrectBasedOnSourceFile()
    {
      var webOrAppConfigFilename = "Web.Config";
      var apiReportGenerator = new ApiEndPointReportGenerator.Generators.ApiReportGenerator();
      var actualAssemblyNeededList = apiReportGenerator.GenerateRedirectAssembliesList(webOrAppConfigFilename);

      var assemblyNeededNewtonsoftJson = new AssemblyNeeded()
        {
          Name = "Newtonsoft.Json",
          PublicKey = "30ad4fe6b2a6aeed",
          Version = "6.0.0.0"
        };
      var assemblyNeededSysWebHttp = new AssemblyNeeded()
        {
          Name = "System.Web.Http",
          PublicKey = "31bf3856ad364e35",
          Version = "5.2.3.0"
      };
      var assemblyNeededSysNetHttpFormatting = new AssemblyNeeded()
        {
          Name = "System.Net.Http.Formatting",
          PublicKey = "31bf3856ad364e35",
          Version = "5.2.3.0"
      };
      var assemblyNeededSysWebMvc = new AssemblyNeeded()
        {
          Name = "System.Web.Mvc",
          PublicKey = "31bf3856ad364e35",
          Version = "5.2.3.0"
      };
      var assemblyNeededSysWebHelpers = new AssemblyNeeded()
        {
          Name = "System.Web.Helpers",
          PublicKey = "31bf3856ad364e35",
          Version = "3.0.0.0"
      };
      var assemblyNeededSysWebWebPages = new AssemblyNeeded()
        {
          Name = "System.Web.WebPages",
          PublicKey = "31bf3856ad364e35",
          Version = "3.0.0.0"
      };
      var assemblyNeededLog4Net = new AssemblyNeeded()
        {
          Name = "log4net",
          PublicKey = "1b44e1d426115821",
          Version = "2.0.8.0"
        };
      var expectedAssembliesNeededList = new List<AssemblyNeeded>()
      {
        assemblyNeededNewtonsoftJson,
        assemblyNeededSysWebHttp,
        assemblyNeededSysNetHttpFormatting,
        assemblyNeededSysWebMvc,
        assemblyNeededSysWebHelpers,
        assemblyNeededSysWebWebPages,
        assemblyNeededLog4Net
      };

      Assert.AreEqual(expectedAssembliesNeededList.Count, actualAssemblyNeededList.Count(), "AssemblyBuiltList NOT matching expected.");
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededNewtonsoftJson);
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededSysWebHttp);
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededSysNetHttpFormatting);
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededSysWebMvc);
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededSysWebHelpers);
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededSysWebWebPages);
      AssemblyNeededHelper.AssertAssemblyNeededInList(actualAssemblyNeededList, assemblyNeededLog4Net);
    }
  }
}
