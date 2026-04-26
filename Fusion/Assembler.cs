using System.Diagnostics;
using System.Text.Json;
using AtomicDSL;
using Fusion.Pipeline;
using RadiumCommon;
using RadiumCommon.Exceptions;

namespace Fusion;

public class FusionCompilationStep
{
    private List<string> IncludeFragment = [];
    private List<string> SourceFragment = [];
    private List<string> LibraryFragments = [];
    private List<string> FlagFragments = [];

    public readonly SourceFiles SourceFileFactory;

    private readonly List<RadiumKeyValueStore<AtomicKeyTypes>> AtomicKeyValueStores;

    public FusionCompilationStep(List<RadiumKeyValueStore<AtomicKeyTypes>> store)
    {
        // Init factories
        SourceFileFactory = new(this);

        AtomicKeyValueStores = store;
    }

    public void Assemble()
    {
    //     dict.TryGetValue("strings", out AtomicLanguageNode? @strings);
    //     dict.TryGetValue("includes", out AtomicLanguageNode? @includes);
    //     dict.TryGetValue("sources", out AtomicLanguageNode? @sources);
    //     dict.TryGetValue("assets", out AtomicLanguageNode? @assets);
    //     dict.TryGetValue("flags", out AtomicLanguageNode? @flags);

        string AtomicBinaryName = KeyValueUtil.GetStoreFromName(AtomicKeyValueStores, "target").Get<string>();
        var VersionStore = KeyValueUtil.GetStoreFromName(AtomicKeyValueStores, "version");
        
        if (!VersionStore.IsEmpty())
        {
            RadiumLogger.Write(
                $"Project %m{AtomicBinaryName}%c is using atomic version %b{VersionStore.Get<string>()}%c");
        } else 
        {
            throw new Exception($"{AtomicBinaryName} must define a version attribute");
        }

    //     // This will always be available 
    //     var binaryName = GetKeyValuePair(@strings!, "target");
    //     var libDir = GetKeyValuePair(@strings!, "libs");

    //     var binaryType = GetKeyValuePair(@strings!, "type");
    //     var binaryLibraryType = GetKeyValuePair(@strings!, "library_type");

    //     var HashDatabase = SourceFiles.GetFileHashesFile(binaryName!.Value.Value);

    //     foreach (var include in @includes!.ArrayChildren)
    //     {
    //         IncludeFragment.Add($"-I{include}");
    //     }
    //     foreach (var source in @sources!.ArrayChildren)
    //     {
    //         if (source.EndsWith('/'))
    //         {
    //             SourceFiles.MapSources(source, (file) =>
    //             {
    //                 SourceFragment.Add(file);
    //                 ComputeHash(file);
    //                 RadiumLogger.Write(
    //                     $"Fusion.Assembler >> {CheckHash(file, HashDatabase)} >> Found source file %m{file}%c");
    //             });
    //         }
    //         else
    //         {
    //             if (source.EndsWith(FileExtensions.GetSharedLibraryEXT())) continue;
    //             SourceFragment.Add(source);
    //             ComputeHash(source);
    //             RadiumLogger.Write(
    //                 $"Fusion.Assembler >> {CheckHash(source, HashDatabase)} >> Source file added %m{source}%c"
    //             );
    //         }
    //     }

    SourceFileFactory.SearchSources(KeyValueUtil.GetStoreFromName(
        AtomicKeyValueStores, "sources").Get<List<string>>());

    //     if (libDir.HasValue)
    //     {
    //         foreach (var lib in FusionLibrarySearcher.Search(libDir.Value.Value))
    //         {
    //             if (lib.OSLibraryDir != null)
    //             {
    //                 Directory.EnumerateFiles(lib.OSLibraryDir)
    //                 .ToList()
    //                 .ForEach(file =>
    //                 {
    //                     if (file.EndsWith(FileExtensions.GetStaticLibraryEXT()))
    //                     {
    //                         LibraryFragments.Add(file);
    //                     }
    //                     if (file.EndsWith(FileExtensions.GetSharedLibraryEXT()))
    //                     {
    //                         @assets!.ArrayChildren.Add(file);
    //                     }
    //                 });
    //             }
    //             if (lib.LibrarySourceDir != null)
    //             {
    //                 SourceFiles.MapSources(lib.LibrarySourceDir, (file) =>
    //                 {
    //                     RadiumLogger.Write(
    //                         $"Fusion.Assembler >> Found library source file %m{file}%c");
    //                     SourceFragment.Add(file);
    //                 });
    //             }
    //             IncludeFragment.Add($"-I{lib.LibraryIncludeDir}");
    //         }
    //     }

    //     foreach (var flag in @flags!.ArrayChildren)
    //     {
    //         if (flag.Contains(';'))
    //         {
    //             string[] frag = flag.Split(";");
    //             if (frag[0] == FusionLibrarySearcher.Windows)
    //             {
    //                 FlagFragments.Add(frag[1]);
    //             }
    //         } else
    //         {
    //             FlagFragments.Add(flag);
    //         }
    //     }

    //     if (binaryType!.Value.Value == "binary")
    //     {
    //         if (OperatingSystem.IsWindows())
    //         {
    //             FlagFragments.Add("-Wl,/SUBSYSTEM:WINDOWS");
    //             FlagFragments.Add("-Wl,/NOIMPLIB");
    //             FlagFragments.Add("-Wl,/NOEXP");
    //         }
    //     }
    //     if (binaryType.Value.Value == "library")
    //     {
    //         if (OperatingSystem.IsWindows())
    //         {
    //             FlagFragments.Add($"-Wl,/IMPLIB:{BuildTool.LibsOutDir}/{binaryName!.Value.Value}.lib");
    //         }
    //     }

    //     string binaryOutput = $"-o {BuildTool.BinPath}/{binaryName!.Value.Value}";
    //     FusionLibraryType LibraryType = FusionLibraryType.None;

    //     if (binaryType.HasValue && binaryType.Value.Value == "library")
    //     {
    //         if (!binaryLibraryType.HasValue)
    //         {
    //             throw new Exception(
    //                 "Binary has 'type = \"library\"' set, 'library_type' is null"
    //             );
    //         }
    //         if (binaryLibraryType.Value.Value == "shared")
    //         {
    //             if (OperatingSystem.IsMacOS())
    //             {
    //                 FlagFragments.Add("-dynamiclib -shared");
    //             }
    //             if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
    //             {
    //                 FlagFragments.Add("-shared");
    //             }
    //             LibraryType = FusionLibraryType.SharedLibrary;
    //             binaryOutput += FileExtensions.GetSharedLibraryEXT();
    //             FlagFragments.Add("-c");
    //         } else
    //         {
    //             LibraryType = FusionLibraryType.StaticLibrary;
    //         }
    //     }

    //     if (binaryType.Value.Value == "binary" || binaryType.Value.Value == "console")
    //     {
    //         binaryOutput += FileExtensions.GetOSExecutableEXT();
    //     }

    //     RadiumLogger.Write(
    //         $"Fusion.Assembler >> Starting compilation of %b{SourceFragment.Count}%c sources");
        
    //     string CacheDir = BuildTool.ManageProjectCacheDir(binaryName.Value.Value);

    //     List<string> ObjectFiles = [];

    //     foreach (string SourceFile in SourceFragment)
    //     {
    //         string SourceFileName = Path.GetFileName(SourceFile);
    //         string ObjectFilePath = Path.Join(CacheDir, 
    //             $"{Path.GetFileNameWithoutExtension(SourceFile)}{FileExtensions.GetObjectFileEXT()}");

    //         List<string> clangFragments = [
    //             ..FlagFragments,
    //             ..IncludeFragment,
    //             ..LibraryFragments,
    //             SourceFile,
    //             $"-o {ObjectFilePath}"
    //         ];

    //         // Put this argument to the compile_commands.json
    //         var withCompiler = new[] { FusionLocation.GetClangExecutable() }.Concat(clangFragments.ToArray());
    //         FusionCompileCommands.Database.Add(new()
    //         {
    //             Arguments = withCompiler.ToArray(),
    //             File = SourceFile.Trim()
    //         });

    //         RadiumLogger.Write($"Compiling %b{SourceFile}%c");
    //         // Compile Object file
    //         FusionClang.Invoke(clangFragments);
    //         ObjectFiles.Add(ObjectFilePath);
    //     }

    //     if (LibraryType != FusionLibraryType.StaticLibrary)
    //     {
    //         FusionClang.Invoke([$"-o {binaryOutput}", ..ObjectFiles]);
    //     }

    //     VerifyHashes(binaryName!.Value.Value);

    //     if (@assets!.ArrayChildren.Count != 0)
    //     {
    //         RadiumLogger.Write(
    //             $"Populating {BuildTool.BinPath}...");
    //     }
    //     foreach (var asset in @assets.ArrayChildren)
    //     {
    //         FusionAssetPipeline.Copy(asset);
    //     }
    }

    // private static KeyValuePair<string, string>? GetKeyValuePair(
    //     AtomicLanguageNode pairList,
    //     string key
    // )
    // {
    //     foreach (KeyValuePair<string, string> pair in pairList.KeywordPair)
    //     {
    //         if (pair.Key == key) return pair;
    //     }
    //     // If there was no match then return null
    //     return null;
    // }
}