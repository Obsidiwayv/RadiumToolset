using System.Runtime.InteropServices;

namespace AtomicDSL.Language;

public class AtomicParser
{
    public static Dictionary<string, AtomicLanguageNode> Use(List<AtomicNode> nodes)
    {
        Dictionary<string, AtomicLanguageNode> dict = new()
        {
            {"includes", new()},
            {"sources", new()},
            {"assets", new()},
            {"flags", new()},
            {"strings", new()},
        };

        dict.TryGetValue("strings", out AtomicLanguageNode? strs);

        for (int index = 0; index < nodes.Count; index++)
        {
            AtomicNode node = nodes[index];

            if (index != 0
                && nodes[index].TokenType == AtomicLexerTokens.StringLiteral
                && strs != null
                && VerifyStringIndex(index, nodes))
            {
                string keyType = index > 1 
                        ? nodes[index - 2].Value
                        : "target";
                strs.KeywordPair.Add(new(keyType, ParseString(node.Value)));
            }

            if (node.TokenType == AtomicLexerTokens.Bracket
                && nodes.Count > index
                // Make sure THIS is not a target
                && nodes[index - 1].TokenType != AtomicLexerTokens.StringLiteral
                && node.Value == "{")
            {
                AtomicNode keyType = nodes[index - 2];
                dict.TryGetValue(keyType.Value, out AtomicLanguageNode? keyValue);

                if (keyValue == null)
                {
                    continue;
                }
                if (keyType.Value == "strings") continue;
                index++;

                for (int sIndex = 0 + index; sIndex < nodes.Count; sIndex++)
                {
                    AtomicNode subNode = nodes[sIndex];

                    if (subNode.Value == "}") break;
                    keyValue.ArrayChildren.Add(subNode.Value);
                }
            }
        }
        return dict;
    }

    private static bool VerifyStringIndex(int index, List<AtomicNode> nodes)
    {
        if (nodes[index - 1].Value == "target") return true;
        if (nodes[index - 1].Value == "=") return true;
        return false;
    }

    private static string ParseString(string str)
    {
        return str
            .Replace("#arch", AtomicOSInformation.GetArchitecture())
            .Replace("#platform", AtomicOSInformation.GetName())
            .Replace("root://", Directory.GetCurrentDirectory());
    }
}