using ApiEndPointReportGenerator.Generators;
using ApiEndPointReportGenerator.Helpers;
using System;

namespace ApiEndPointReportGenerator
{
  class Program
  {
    static void Main(string[] args)
    {
      //only arg should be the Resource File to accommodate this execution. 
      var resourceFileInfo = FileHelper.GetFileInfo(args[0]);

      var generator = new ApiReportGenerator();
      generator.Generate(resourceFileInfo);

      // The code provided will print ‘Hello World’ to the console.
      // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
#if DEBUG
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();
#endif
      // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
    }
  }
}
