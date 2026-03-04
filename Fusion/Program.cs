using AtomicDSL;
using AtomicDSL.Language;

namespace Fusion;

public class BuildTool
{
    public static string BinPath = @"Bin";

    public static void Main(string[] args)
    {
        if (!Directory.Exists(BinPath)) Directory.CreateDirectory(BinPath);
        
        string path = "";
        if (!args[0].EndsWith(AtomicConstants.FileExtention))
        {
            Directory.EnumerateFiles(args[0])
                .ToList()
                .ForEach(file =>
                {
                    if (!file.EndsWith(AtomicConstants.FileExtention))
                        return;
                    // We found an atomic file!
                    path = file;
                });
        } else
        {
            // Its already an atomic file just pass the argument
            path = args[0];
        }
        List<AtomicNode> nodes = 
            AtomicLexer.Run(File.ReadAllText(path).ToCharArray());
        FusionCompilationStep step = new(AtomicParser.Use(nodes));
        step.Assemble();
    }
}