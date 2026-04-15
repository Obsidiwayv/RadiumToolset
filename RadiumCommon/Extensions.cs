using RadiumCommon.Exceptions;

namespace RadiumCommon;

public class FileExtensions
{
    /// <summary>
    /// Returns .a for unix, .lib for windows
    /// </summary>
    /// <exception cref="RadiumUnknownOS"></exception>
    public static string GetStaticLibraryEXT()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            return ".a";
        }
        if (OperatingSystem.IsWindows())
        {
            return ".lib";
        }
        throw new RadiumUnknownOS();
    }

    /// <summary>
    /// Returns .dylib for MacOS, .lib for Windows, .so for Linux
    /// </summary>
    /// <exception cref="RadiumUnknownOS"></exception>
    public static string GetSharedLibraryEXT()
    {
        if (OperatingSystem.IsMacOS())
        {
            return ".dylib";
        }
        if (OperatingSystem.IsLinux())
        {
            return ".so";
        }
        if (OperatingSystem.IsWindows())
        {
            return ".dll";
        }
        throw new RadiumUnknownOS();
    }

    public static string GetObjectFileEXT()
    {
        if (OperatingSystem.IsWindows()) return ".obj";
        else return ".o"; // Mach-O (MacOS) or Object (Linux/BSD)
    }

    public static string GetOSExecutableEXT()
    {
        if (OperatingSystem.IsWindows()) return ".exe";
        return "";
    }
}