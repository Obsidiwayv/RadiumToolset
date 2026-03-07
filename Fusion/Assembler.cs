using System.Diagnostics;
using System.Text.Json;
using AtomicDSL;
using Fusion.Pipeline;
using RadiumCommon;

namespace Fusion;


public class FusionCompilationStep(Dictionary<string, AtomicLanguageNode> dict)
{
    private List<string> IncludeArgument = [];
    private List<string> SourcesArgument = [];
    private List<string> LibraryArguments = [];
    private List<CompileCommands> Database = [];

    private readonly JsonSerializerOptions DatabaseOpt = new()
    {
        WriteIndented = true
    };

    public void Assemble()
    {
        dict.TryGetValue("strings", out AtomicLanguageNode? @strings);
        dict.TryGetValue("includes", out AtomicLanguageNode? @includes);
        dict.TryGetValue("sources", out AtomicLanguageNode? @sources);
        dict.TryGetValue("assets", out AtomicLanguageNode? @assets);
        dict.TryGetValue("flags", out AtomicLanguageNode? @flags);

        // This will always be available 
        var binaryName = GetKeyValuePair(@strings!, "target");
        var libDir = GetKeyValuePair(@strings!, "libs");

        foreach (var include in @includes!.ArrayChildren)
        {
            IncludeArgument.Add($"-I{include}");
        }
        foreach (var source in @sources!.ArrayChildren)
        {
            if (source.EndsWith('/'))
            {
                Directory.EnumerateFiles(
                    $"{Directory.GetCurrentDirectory()}/{source}",
                    "*", SearchOption.AllDirectories)
                    .ToList()
                    .ForEach(file =>
                    {
                        Console.WriteLine(
                            $"Fusion.Assembler >> Found source file {file}");
                        SourcesArgument.Add(file);
                    });
            }
            else
            {
                SourcesArgument.Add(source);
            }
        }

        if (libDir.HasValue)
        {
            foreach (var lib in FusionLibrarySearcher.Search(libDir.Value.Value))
            {
                if (lib.OSLibraryDir != null)
                {
                    Directory.EnumerateFiles(lib.OSLibraryDir)
                    .ToList()
                    .ForEach(file =>
                    {
                        if (file.EndsWith(FileExtensions.GetStaticLibraryEXT()))
                        {
                            LibraryArguments.Add(file);
                        }
                        if (file.EndsWith(FileExtensions.GetSharedLibraryEXT()))
                        {
                            @assets!.ArrayChildren.Add(file);
                        }
                    });
                }
                IncludeArgument.Add($"-I{lib.LibraryIncludeDir}");
            }
        }

        List<string> clangArguments = [
            ..SourcesArgument,
            ..IncludeArgument,
            ..LibraryArguments,
            ..@flags!.ArrayChildren,
            $"-o {BuildTool.BinPath}/{binaryName!.Value.Value}",
        ];
        Console.WriteLine(
            $"Fusion.Assembler >> Starting compilation of {SourcesArgument.Count} sources");
        Process proc = new();
        proc.StartInfo.FileName = "clang++";
        proc.StartInfo.Arguments = string.Join(" ", clangArguments);
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();

        LogProcessOutput(proc);

        if (@assets!.ArrayChildren.Count != 0)
        {
            Console.WriteLine(
                $"Populating {BuildTool.BinPath}...");
        }
        foreach (var asset in @assets.ArrayChildren)
        {
            FusionAssetPipeline.Copy(asset);
        }
        foreach (var s in SourcesArgument)
        {
            Database.Add(new()
            {
                Arguments = clangArguments.ToArray(),
                File = s.Trim()
            });
        }
        File.WriteAllText(
            "compile_commands.json",
            JsonSerializer.Serialize(Database, DatabaseOpt));
    }

    private static KeyValuePair<string, string>? GetKeyValuePair(
        AtomicLanguageNode pairList,
        string key
    )
    {
        foreach (KeyValuePair<string, string> pair in pairList.KeywordPair)
        {
            if (pair.Key == key) return pair;
        }
        // If there was no match then return null
        return null;
    }

    private static void LogProcessOutput(Process proc)
    {
        while (!proc.StandardOutput.EndOfStream)
        {
            string? line = proc.StandardOutput.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
                Console.WriteLine(line);
        }
    }
}