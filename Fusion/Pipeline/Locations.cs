namespace Fusion.Pipeline;

public class FusionLocation
{
    public static string WindowsLLVMLocation = "C:/Program Files/LLVM/bin/";

    public static string LLVMLocation = OperatingSystem.IsWindows() ? WindowsLLVMLocation : "";    
}