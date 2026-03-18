using AtomicDSL;
using AtomicDSL.Language;
using Fusion.Pipeline;
using RadiumCommon;

namespace Fusion;

public class BuildTool
{
    public static string OutputPath = @"Bin";
    public static string LibsOutDir = @$"{OutputPath}/Libs";
    public static string BinPath = @$"{OutputPath}/Output";

    public static void Main(string[] args)
    {
        CreateDir(OutputPath);
        CreateDir(BinPath);
        CreateDir(LibsOutDir);

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
    }

    private static void RunSteps(string path)
    {
        List<AtomicNode> nodes =
            AtomicLexer.Run(File.ReadAllText(path).ToCharArray());
        FusionCompilationStep step = new(AtomicParser.Use(nodes));
        step.Assemble();
    }

    private static void CreateDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        };
    }
}