namespace Fusion;

public class FusionAssetPipeline
{
    public static void Copy(string input)
    {
        if (File.Exists(input))
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
        Console.WriteLine($"Fusion.Assets >> Copy file {input} -> {file}");
        File.Copy(input, file, true);
    }
}