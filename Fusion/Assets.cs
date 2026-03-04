namespace Fusion;

public class FusionAssetPipeline
{
    public static void Copy(string input)
    {
        foreach (string dir in Directory.GetDirectories(input, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(input, BuildTool.BinPath));
        }

        foreach (string file in Directory.GetFiles(input, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, file.Replace(input, BuildTool.BinPath), true);
        }
    }
}