using System.Diagnostics;
using System.Text.Json;
using AtomicDSL;
using Fusion.Pipeline;
using RadiumCommon;
using RadiumCommon.Exceptions;

namespace Fusion;

public class FusionCompilationStep(Dictionary<string, AtomicLanguageNode> dict)
{
    private List<string> IncludeFragment = [];
    private List<string> SourceFragment = [];
    private List<string> LibraryFragments = [];
    private List<string> FlagFragments = [];

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

        var binaryType = GetKeyValuePair(@strings!, "type");
        var binaryLibraryType = GetKeyValuePair(@strings!, "library_type");

        foreach (var include in @includes!.ArrayChildren)
        {
            IncludeFragment.Add($"-I{include}");
        }
        foreach (var source in @sources!.ArrayChildren)
        {
            if (source.EndsWith('/'))
            {
                SourceFiles.MapSources(source, (file) =>
                {
                    Console.WriteLine(
                        $"Fusion.Assembler >> Found source file {file}");
                    SourceFragment.Add(file);
                });
            }
            else
            {
                if (source.EndsWith(FileExtensions.GetSharedLibraryEXT())) continue;
                SourceFragment.Add(source);
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
                            LibraryFragments.Add(file);
                        }
                        if (file.EndsWith(FileExtensions.GetSharedLibraryEXT()))
                        {
                            @assets!.ArrayChildren.Add(file);
                        }
                    });
                }
                if (lib.LibrarySourceDir != null)
                {
                    SourceFiles.MapSources(lib.LibrarySourceDir, (file) =>
                    {
                        Console.WriteLine(
                            $"Fusion.Assembler >> Found library source file {file}");
                        SourceFragment.Add(file);
                    });
                }
                IncludeFragment.Add($"-I{lib.LibraryIncludeDir}");
            }
        }

        foreach (var flag in @flags!.ArrayChildren)
        {
            if (flag.Contains(';'))
            {
                string[] frag = flag.Split(";");
                if (frag[0] == FusionLibrarySearcher.Windows)
                {
                    FlagFragments.Add(frag[1]);
                }
            } else
            {
                FlagFragments.Add(flag);
            }
        }

        if (binaryType!.Value.Value == "binary")
        {
            if (OperatingSystem.IsWindows())
            {
                FlagFragments.Add("-Wl,/SUBSYSTEM:WINDOWS");
                FlagFragments.Add("-Wl,/NOIMPLIB");
                FlagFragments.Add("-Wl,/NOEXP");
            }
        }
        if (binaryType.Value.Value == "library")
        {
            if (OperatingSystem.IsWindows())
            {
                FlagFragments.Add($"-Wl,/IMPLIB:{BuildTool.LibsOutDir}/{binaryName!.Value.Value}.lib");
            }
        }

        List<string> clangFragments = [
            ..FlagFragments,
            ..SourceFragment,
            ..IncludeFragment,
            ..LibraryFragments,
        ];

        string binaryOutput = $"-o {BuildTool.BinPath}/{binaryName!.Value.Value}";
        if (binaryType.HasValue && binaryType.Value.Value == "library")
        {
            if (!binaryLibraryType.HasValue)
            {
                throw new Exception(
                    "Binary has 'type = \"library\"' set, 'library_type' is null"
                );
            }
            if (binaryLibraryType.Value.Value == "shared")
            {
                if (OperatingSystem.IsMacOS())
                {
                    clangFragments.Add("-dynamiclib -shared");
                }
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
                {
                    clangFragments.Add("-shared");
                }
                binaryOutput += FileExtensions.GetSharedLibraryEXT();
            }
        }

        if (binaryType.Value.Value == "binary" || binaryType.Value.Value == "console")
        {
            binaryOutput += FileExtensions.GetOSExecutableEXT();
        }

        clangFragments.Add(binaryOutput);

        Console.WriteLine(string.Join(" ", clangFragments));

        Console.WriteLine(
            $"Fusion.Assembler >> Starting compilation of {SourceFragment.Count} sources");
        Process proc = new();
        var compiler = $"{FusionLocation.LLVMLocation}clang++";
        proc.StartInfo.FileName = compiler;
        proc.StartInfo.Arguments = string.Join(" ", clangFragments);
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

        var withCompiler = new[] { compiler }.Concat(clangFragments.ToArray());
        foreach (var s in SourceFragment)
        {
            FusionCompileCommands.Database.Add(new()
            {
                Arguments = withCompiler.ToArray(),
                File = s.Trim()
            });
        }
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