using System.Runtime.InteropServices;

namespace AtomicDSL.Language;

public class AtomicOSInformation
{
    public static string GetArchitecture()
    {
        var arch = RuntimeInformation.OSArchitecture;
        if (arch == Architecture.X86) return "x32";
        if (arch == Architecture.X64) return "x64";
        return "NONE";
    }

    public static string GetName()
    {
        if (OperatingSystem.IsMacOS()) return "Mac";
        return "NONE";
    } 
}