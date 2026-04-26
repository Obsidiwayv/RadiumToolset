using AtomicDSL;
using AtomicDSL.Language;
using Fusion.Pipeline;
using RadiumCommon;

namespace Fusion;

public class BuildTool
{
    public static readonly string OutputPath = @"Bin";
    public static readonly string LibsOutDir = @$"{OutputPath}/Libs";
    public static readonly string BinPath = @$"{OutputPath}/Output";
    public static readonly string ObjCachePath = @$"{OutputPath}/Cache";
    public static readonly string OutputRequiredPath = @$"{BinPath}/Required";
    
    // These are all blocks, attributes will be added later in the compilation process
    private static readonly List<RadiumKeyValueStore<AtomicKeyTypes>> DefaultParseRules = [
        new("target") { KeyStoreType = AtomicKeyTypes.Block },
        new("type") { KeyStoreType = AtomicKeyTypes.Block },
        new("library_type") { KeyStoreType = AtomicKeyTypes.Block },
        new("libs") { KeyStoreType = AtomicKeyTypes.Block },
        new("sources") { KeyStoreType = AtomicKeyTypes.Block },
        new("includes") { KeyStoreType = AtomicKeyTypes.Block },
        new("flags") { KeyStoreType = AtomicKeyTypes.Block }
    ];

    public static void Main(string[] args)
    {
        CreateDir(OutputPath);
        CreateDir(BinPath);
        CreateDir(LibsOutDir);
        CreateDir(ObjCachePath);
        CreateDir(OutputRequiredPath);

        //AnsiColors.Init();

        if (args.Length > 1)
        {
            foreach (var arg in args)
            {
                RunSteps(arg);
            }
        }
        else
        {
            string path = "";
            if (!args[0].EndsWith(AtomicConstants.FileExtention))
            {
                FusionAssetPipeline.MapAtomicFiles(args[0], (file) =>
                {
                    path = file;
                });
            }
            else
            {
                // Its already an atomic file just pass the argument
                path = args[0];
            }
            RunSteps(path);
        }

        FusionCompileCommands.Finish();
        //Directory.Delete(ObjCachePath, true);
    }

    public static string ManageProjectCacheDir(string project)
    {
        var CacheDir = Path.Join(ObjCachePath, project);
        CreateDir(CacheDir);
        return CacheDir;
    }

    private static void RunSteps(string path)
    {
        List<AtomicNode> nodes =
            AtomicLexer.Run(File.ReadAllText(path).ToCharArray());
        FusionCompilationStep step = new(AtomicParser.Use(nodes, DefaultParseRules));
        step.Assemble();
        foreach (var Store in DefaultParseRules)
        {
            Store.Value.Clear();
        }
    }

    private static void CreateDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        };
    }
}