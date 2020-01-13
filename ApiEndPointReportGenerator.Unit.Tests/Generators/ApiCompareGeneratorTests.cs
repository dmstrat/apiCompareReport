using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiEndPointReportGenerator.Unit.Tests.Generators
{
  [TestClass]
  public class ApiCompareGeneratorTests
  {
    [TestMethod]
    public void StripLeadVersionInfoWorksAsExpected()
    {
      var sourceLine = @"~/api/v1/employeeAttributes|Get(FreeText name)|False";
      var actual =
        EndPointCompare.Generators.ApiCompareGenerator.StripBaseRouteFromString(sourceLine);

      var expected = @"/employeeAttributes|Get(FreeText name)|False";

      Assert.AreEqual(expected, actual, "Failed to strip correct items from string.");
    }

    [TestMethod]
    public void StripLeadVersionInfoWorksWhenNoTildeApiPresent()
    {
      var sourceLine = @"/employeeAttributes/{attributeId}|Get(Int attributeId)|False";
      var actual =
        EndPointCompare.Generators.ApiCompareGenerator.StripBaseRouteFromString(sourceLine);

      var expected = @"/employeeAttributes/{attributeId}|Get(Int attributeId)|False";

      Assert.AreEqual(expected, actual, "Failed to strip correct items from string.");

    }
  }
}
