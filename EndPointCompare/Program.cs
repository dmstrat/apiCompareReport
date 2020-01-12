using System;
using System.Diagnostics;
using System.IO;
using EndPointCompare.Generators;

namespace EndPointCompare
{
  class Program
  {
    static void Main(string[] args)
    {
      Trace.WriteLine("Program BEGINS...");
#if DEBUG
      SampleGenerator.CreateEndPointCompareConfigResourceSample();
#endif
      var configFilename = args[0];
      var configFileExists = new FileInfo(configFilename).Exists;
      if (configFileExists)
      {
        var generator = new ApiCompareGenerator();
        generator.Generate(configFilename);
      }
      Trace.WriteLine("Program ENDS...");
#if DEBUG
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();
#endif
    }
  }
}
