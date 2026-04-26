using System.Text.Json.Serialization;
namespace RadiumCommon;

#pragma warning disable CS8618

public class CompileCommands
{
    [JsonPropertyName("directory")]
    public string Directory { get; set; } 
        = System.IO.Directory.GetCurrentDirectory();

    [JsonPropertyName("file")]
    public string File { get; set; }

    [JsonPropertyName("arguments")]
    public string[] Arguments { get; set; }

}