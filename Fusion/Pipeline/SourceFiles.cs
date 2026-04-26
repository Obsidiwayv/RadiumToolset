using System.Text;
using RadiumCommon;

namespace Fusion.Pipeline;

public class SourceFiles(FusionCompilationStep step)
{
    public string HashFileCache = $"{BuildTool.OutputPath}/.sourcehashes";
    private readonly FusionHash HashFactory = new(step);
    public List<string> Fragments = [];

    public void MapSources(string dir, Action<string> cbSource)
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

    public void SearchSources(List<string> sources)
    {
        foreach (var source in sources)
        {
            if (source.EndsWith('/'))
            {
                MapSources(source, (file) =>
                {
                    Fragments.Add(file);
                    HashFactory.ComputeHash(file);
                    RadiumLogger.Write(
                        $"Fusion.Assembler >> {HashFactory.CheckHash(file, HashFactory.FileHashes)} >> Found source file %m{file}%c");
                });
            }
            else
            {
                if (source.EndsWith(FileExtensions.GetSharedLibraryEXT())) continue;
                Fragments.Add(source);
                HashFactory.ComputeHash(source);
                RadiumLogger.Write(
                    $"Fusion.Assembler >> {HashFactory.CheckHash(source, HashFactory.FileHashes)} >> Source file added %m{source}%c"
                );
            }
        }
    }

    public void WriteHashFile(Dictionary<string, string> hashes, string projName)
    {
        StringBuilder HashList = new();
        foreach (var (file, hash) in hashes) 
        {
            HashList.Append($"{file};{hash}\n");
        }
        File.WriteAllText($"{HashFileCache}-{projName}", HashList.ToString());
    }

    public Dictionary<string, string> GetFileHashesFile(string projName)
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