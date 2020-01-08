using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EndPointCompare.Helpers
{
  internal static class AssemblyRedirectHandler
  {
    public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
    {
      ResolveEventHandler handler = null;

      handler = (sender, args) => {
        // Use latest strong name & version when trying to load SDK assemblies
        var requestedAssembly = new AssemblyName(args.Name);
        if (requestedAssembly.Name != shortName)
          return null;

        Debug.WriteLine("Redirecting assembly load of " + args.Name
          + ",\tloaded by " + (args.RequestingAssembly == null ? "(unknown)" : args.RequestingAssembly.FullName));

        var alreadyLoadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
          .FirstOrDefault(a => a.GetName().Name == requestedAssembly.Name);
        var assemblyIsAlreadyLoaded = alreadyLoadedAssembly != null;
        if (assemblyIsAlreadyLoaded)
        {
          return alreadyLoadedAssembly;
        }

        requestedAssembly.Version = targetVersion;
        requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
        requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

        //AppDomain.CurrentDomain.AssemblyResolve -= handler;

        return Assembly.Load(requestedAssembly);
      };
      AppDomain.CurrentDomain.AssemblyResolve += handler;
    }
  }
}
