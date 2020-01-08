using System;
using System.Diagnostics;
using System.Reflection;

namespace EndPointCompare
{
  public class Proxy : MarshalByRefObject 
  {
    public Assembly GetAssembly(string assemblyPath)
    {
      try
      {
        return Assembly.LoadFile(assemblyPath);
      }
      catch (Exception e)
      {
        Trace.WriteLine("Proxy=>GetAssembly=>Exception:" + e.Message);
        return null;
      }
    }
  }
}
