using System.Diagnostics;
using Fusion.Pipeline;
using RadiumCommon;

namespace Fusion;

public class FusionClang
{
    // Uses llvm-ar
    public static void UseARTool(string binaryName, List<string> ObjectFiles)
    {
        CreateInvoke($"{FusionLocation.LLVMLocation}llvm-ar", [
            $"{BuildTool.LibsOutDir}/{binaryName}{FileExtensions.GetSharedLibraryEXT()}",
            ..ObjectFiles
        ]);
    }

    public static void Invoke(List<string> arguments)
    {
        CreateInvoke(FusionLocation.GetClangExecutable(), arguments);
    }

    private static void CreateInvoke(string program, List<string> arguments)
    {
        Process proc = new();
        proc.StartInfo.FileName = program;
        proc.StartInfo.Arguments = string.Join(" ", arguments);
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();

        while (!proc.StandardOutput.EndOfStream)
        {
            string? line = proc.StandardOutput.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
                RadiumLogger.Write(line);
        }
    }
}