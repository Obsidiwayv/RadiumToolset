namespace Fusion.Pipeline;

public class LibraryDir
{
    public string? OSLibraryDir { get; set; }
    public required string LibraryIncludeDir { get; set; }
}

public class FusionLibrarySearcher
{
    public static string Darwin = "MacOS";
    public static string Linux = "Linux";

    public static List<LibraryDir> Search(string directory)
    {
        List<LibraryDir> libs = [];
        foreach (string dir in Directory
            .GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
        {
            var osLib = MatchOSLibraryDir(dir);
            var includeDir = $"{dir}/include";
            if (Path.Exists(osLib))
            {
                Console.WriteLine($"Library Search >> Found library {dir}");
                libs.Add(new()
                {
                    LibraryIncludeDir = includeDir,
                    OSLibraryDir = osLib
                });
            } else
            {
                if (Path.Exists(includeDir))
                {
                    Console.WriteLine(
                        $"Library Search >> Found header only library {dir}");
                    libs.Add(new()
                    {
                        LibraryIncludeDir = includeDir
                    });
                }
            }
        }
        return libs;
    }

    public static string MatchOSLibraryDir(string dir)
    {
        string path = "unknown";
        if (OperatingSystem.IsMacOS())
        {
            path = Darwin;
        }
        return $"{dir}/{path}";
    }
}