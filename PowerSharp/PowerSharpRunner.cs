using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace PowerSharp
{
    public static class PowerSharpRunner
    {
        public static void RunFile(string file) { RunString(File.ReadAllText(file)); }
        public static void RunString(string str) { CSharpScript.RunAsync(str, ScriptOptions.Default.AddReferences(MetadataReference.CreateFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PowerExtensions.dll")))); }
    }
}