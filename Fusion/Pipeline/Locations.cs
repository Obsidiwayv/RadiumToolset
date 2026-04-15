namespace Fusion.Pipeline;

public class FusionLocation
{
    public static string WindowsLLVMLocation = "C:/Program Files/LLVM/bin/";

    public static string LLVMLocation = OperatingSystem.IsWindows() ? WindowsLLVMLocation : "";    

    // TODO: Make this return the right clang type depending on the file
    public static string GetClangExecutable()
    {
        return $"{LLVMLocation}clang++";
    }
}