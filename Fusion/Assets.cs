using AtomicDSL;
using RadiumCommon;

namespace Fusion;

public class FusionAssetPipeline
{
    public static void Copy(string input)
    {
        if (!File.GetAttributes(input).HasFlag(FileAttributes.Directory))
        {
            CopyFile(input);
            return;
        }
        foreach (string dir in Directory.GetDirectories(input, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(input, BuildTool.BinPath));
        }

        foreach (string file in Directory.GetFiles(input, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, file.Replace(input, BuildTool.BinPath), true);
        }
    }

    private static void CopyFile(string input)
    {
        var file = Path.Combine(BuildTool.BinPath, Path.GetFileName(input));
        RadiumLogger.Write($"Fusion.Assets >> Copy file %b{input}%c -> %b{file}%c");
        File.Copy(input, file, true);
    }

    public static void MapAtomicFiles(string dir, Action<string> cbEnumerate)
    {
        Directory.EnumerateFiles(dir)
            .ToList()
            .ForEach(file =>
            {
                if (!file.EndsWith(AtomicConstants.FileExtention))
                    return;
                // We found an atomic file!
                cbEnumerate(file);
            });
    }
}