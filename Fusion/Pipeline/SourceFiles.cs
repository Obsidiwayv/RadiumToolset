using System.Text;

namespace Fusion.Pipeline;

public class SourceFiles
{
    public static string HashFileCache = $"{BuildTool.OutputPath}/.sourcehashes";

    public static void MapSources(string dir, Action<string> cbSource)
    {
        Directory.EnumerateFiles(
            $"{Directory.GetCurrentDirectory()}/{dir}",
            "*", SearchOption.AllDirectories)
            .ToList()
            .ForEach(file =>
            {
                cbSource(file);
            });
    }

    public static void WriteHashFile(Dictionary<string, string> hashes, string projName)
    {
        StringBuilder HashList = new();
        foreach (var (file, hash) in hashes) 
        {
            HashList.Append($"{file};{hash}\n");
        }
        File.WriteAllText($"{HashFileCache}-{projName}", HashList.ToString());
    }

    public static Dictionary<string, string> GetFileHashesFile(string projName)
    {
        Dictionary<string, string> Hashes = [];
        // The file doesnt exist so just return the empty dictionary
        if (!File.Exists($"{HashFileCache}-{projName}")) return Hashes;
        foreach (string line in File.ReadLines($"{HashFileCache}-{projName}"))
        {
            string[] Values = line.Split(";");
            //         FilePath   Computed Hashes
            Hashes.Add(Values[0], Values[1]);
        }
        return Hashes;
    }
}