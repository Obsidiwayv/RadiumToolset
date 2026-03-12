using System.Text.Json;
using RadiumCommon;

namespace Fusion.Pipeline;

public class FusionCompileCommands
{
    public static List<CompileCommands> Database = [];

    private static readonly JsonSerializerOptions DatabaseOpt = new()
    {
        WriteIndented = true
    };

    public static void Finish()
    {
        File.WriteAllText(
            "compile_commands.json",
            JsonSerializer.Serialize(Database, DatabaseOpt));
    }
}