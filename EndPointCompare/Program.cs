using System;
using System.Diagnostics;
using EndPointCompare.Generators;

namespace EndPointCompare
{
  class Program
  {
    static void Main(string[] args)
    {
      Trace.WriteLine("Program BEGINS...");
      var generator = new ApiCompareGenerator();
      generator.Generate();
      //ApiCompareGenerator.Generate();
      Trace.WriteLine("Program ENDS...");
#if DEBUG
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();
#endif
    }
  }
}
