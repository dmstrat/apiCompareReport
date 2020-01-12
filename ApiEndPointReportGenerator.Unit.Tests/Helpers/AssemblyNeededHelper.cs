using Core.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ApiEndPointReportGenerator.Unit.Tests.Helpers
{
  public static class AssemblyNeededHelper
  {
    public static void AssertAssemblyNeededInList(IEnumerable<AssemblyNeeded> assemblyNeededList,
      AssemblyNeeded assemblyNeeded)
    {
      var foundMatch = false;
      foreach (var assemblyNeededToTest in assemblyNeededList)
      {
        var matchOnName = assemblyNeededToTest.Name.Equals(assemblyNeeded.Name);
        if (matchOnName)
        {
          Assert.AreEqual(assemblyNeeded.PublicKey, assemblyNeededToTest.PublicKey, "Mismatch on Public Key");
          Assert.AreEqual(assemblyNeeded.Version, assemblyNeededToTest.Version, "Mismatch on Version");
          foundMatch = true;
        }
      }
      Assert.IsTrue(foundMatch, $"Unfound match for {assemblyNeeded.Name}");
    }
  }
}
