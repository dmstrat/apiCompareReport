using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ApiEndPointReportGenerator
{
  public static class TypeExtensions
  {
    private const BindingFlags AllScopes =
      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;

    public static bool IsNotCompilerGenerated(this Type type)
    {
      const string likelyCompilerGeneratedTypeNameToken1 = "<";
      const string likelyCompilerGeneratedTypeNameToken2 = ">";

      var typeIsLikelyCompilerGenerated = type.IsDefined(typeof(CompilerGeneratedAttribute))
        || type.Name.Contains(likelyCompilerGeneratedTypeNameToken1)
        || type.Name.Contains(likelyCompilerGeneratedTypeNameToken2);

      return !typeIsLikelyCompilerGenerated;
    }

    public static IEnumerable<MethodInfo> GetAllMethodsCreatedByHumans(this Type type, string controllerNamespace)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      bool NoCustomAttributesCreatedByCompiler(MethodInfo m) => !m.GetCustomAttributes(true).
        Any(a => a is CompilerGeneratedAttribute);    

      return type.GetMethods(AllScopes)
        .Where((Func<MethodInfo, bool>) NoCustomAttributesCreatedByCompiler)
        .Where(i => i.DeclaringType.FullName.Contains(controllerNamespace));
    }
  }
}
