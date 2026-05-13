using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MMI_SP.Helpers
{
    internal static class Diagnostics
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentDiagnostics(int offset = 0)
        {
            var methodInfo = new System.Diagnostics.StackTrace().GetFrame(1 + offset).GetMethod();
            var className = methodInfo.ReflectedType.Name;
            return $"{className}.{methodInfo.Name}";
        }
    }
}