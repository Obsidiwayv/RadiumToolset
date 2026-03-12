namespace Fusion.Pipeline;

public class SourceFiles
{
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
}