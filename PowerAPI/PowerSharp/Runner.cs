using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using System.IO;
using PowerAPI.Extensions;

namespace PowerAPI.PowerSharp
{
    public static class Runner
    {
        public static void RunFile(string file) { RunString(File.ReadAllText(file)); }
        public static void RunString(string str) { CSharpScript.RunAsync(str, ScriptOptions.Default.AddReferences(MetadataReference.CreateFromFile(Path.Combine(ApplicationExtensions.GetApplicationLocation(), "PowerAPI.dll")))); }
    }
}