using RadiumCommon;

namespace Fusion.Pipeline;

public enum FusionLibraryType
{
    SharedLibrary,
    StaticLibrary,
    None
}

public class LibraryDir(string dir)
{
    public string? OSLibraryDir { get; set; }
    public string? LibrarySourceDir { get; set; }
    public required string LibraryIncludeDir { get; set; }

    public string GetMessage()
    {
        if (OSLibraryDir != null)
        {
            return  $"LibraryDir >> Found library '%b{dir}%c'";
        } 
        return $"LibraryDir >> Found header only library '%b{dir}%c'";
    }
}

public class FusionLibrarySearcher
{
    public static string Darwin = "MacOS";
    public static string Linux = "Linux";
    public static string Windows = "Windows";

    public static List<LibraryDir> Search(string directory)
    {
        List<LibraryDir> libs = [];
        foreach (string dir in Directory
            .GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
        {
            var osLib = MatchOSLibraryDir(dir);
            var includeDir = $"{dir}/include";
            var sourceDir = $"{dir}/Source";
            
            LibraryDir lib = new(dir)
            {
                LibraryIncludeDir = includeDir
            };
            if (Path.Exists(osLib)) lib.OSLibraryDir = osLib;
            if (Path.Exists(sourceDir)) lib.LibrarySourceDir = sourceDir; 
            RadiumLogger.Write(lib.GetMessage());
            
            libs.Add(lib);
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
        if (OperatingSystem.IsWindows())
        {
            path = Windows;
        }
        return $"{dir}/{path}";
    }
}