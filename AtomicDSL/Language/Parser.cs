using System.Reflection;
using System.Runtime.InteropServices;
using RadiumCommon;

namespace AtomicDSL.Language;

public class AtomicParser
{
    public static List<RadiumKeyValueStore> Use(
        List<AtomicNode> nodes, 
        List<RadiumKeyValueStore> store)
    {
        for (int index = 0; index < nodes.Count; index++)
        {
            AtomicNode node = nodes[index];

            if (index != 0
                && nodes[index].TokenType == AtomicLexerTokens.StringLiteral
                && VerifyStringIndex(index, nodes))
            {
                string AtomicKeyName = index > 1
                        ? nodes[index - 2].Value
                        : "target";

                var StringValue = ParseString(node.Value);

                foreach (var kv in store)
                {
                    if (kv.Key.Equals(AtomicKeyName))
                    {
                        kv.Value.Add(StringValue);
                    }
                }
            }

            if (node.TokenType == AtomicLexerTokens.Bracket
                && nodes.Count > index
                // Make sure THIS is not a target
                && nodes[index - 1].Value == "="
                && node.Value == "{")
            {
                AtomicNode AtomicKeyName = nodes[index - 2];
                if (AtomicKeyName.Value == "strings") continue;
                index++;

                for (int sIndex = 0 + index; sIndex < nodes.Count; sIndex++)
                {
                    AtomicNode subNode = nodes[sIndex];

                    if (subNode.Value == "}") break;
                    foreach (var kv in store)
                    {
                        if (kv.Key.Equals(AtomicKeyName.Value))
                        {
                            kv.Value.Add(subNode.Value);
                        }
                    }
                }
            }
        }
        return store;
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