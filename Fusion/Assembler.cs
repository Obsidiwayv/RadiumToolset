using System.Diagnostics;
using AtomicDSL;

namespace Fusion;

public class FusionCompilationStep(Dictionary<string, AtomicLanguageNode> dict)
{
    private string IncludeArgument = "";
    private string SourcesArgument = "";

    public void Assemble()
    {
        dict.TryGetValue("strings", out AtomicLanguageNode @strings);
        dict.TryGetValue("includes", out AtomicLanguageNode @includes);
        dict.TryGetValue("sources", out AtomicLanguageNode @sources);
        dict.TryGetValue("assets", out AtomicLanguageNode @assets);

        // This will always be available 
        var binaryName = @strings.KeywordPair
            .Find(pair => pair.Key == "target");
        
        foreach (var include in @includes.ArrayChildren)
        {
            IncludeArgument += $"-I{include} ";
        }
        foreach (var source in @sources.ArrayChildren)
        {
            if (source.EndsWith('/'))
            {
                Directory.EnumerateFiles(
                    $"{Directory.GetCurrentDirectory()}/{source}")
                    .ToList()
                    .ForEach(file => SourcesArgument += $"{file} ");
            } else
            {
                SourcesArgument += $"{source} ";
            }
        }

        List<string> clangArguments = [
            $"-o {BuildTool.BinPath}/{binaryName.Value}",
            SourcesArgument,
            IncludeArgument
        ];
        Process proc = new();
        proc.StartInfo.FileName = "clang";
        proc.StartInfo.Arguments = string.Join(" ", clangArguments);
        proc.Start();

        if (@assets.ArrayChildren.Count != 0)
        {
            Console.WriteLine($"Populating {BuildTool.BinPath} with assets");
        }
        foreach (var asset in @assets.ArrayChildren)
        {
            FusionAssetPipeline.Copy(asset);
        }
    }
}