using AtomicDSL;

namespace Fusion;

public class BuildTool
{
    public static void Main(string[] args)
    {
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
        AtomicLexer.Run(File.ReadAllText(path).ToCharArray());
    }
}