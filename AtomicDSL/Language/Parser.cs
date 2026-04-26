using System.Reflection;
using System.Runtime.InteropServices;
using RadiumCommon;

namespace AtomicDSL.Language;

public class AtomicParser
{
    /// <summary>
    /// Takes a List of Atomic nodes and stores
    /// using the stores it will match up the correct keywords to add to the stores
    /// returning the key values to use with an assembler
    /// </summary>
    /// <returns>A list of radium key value stores</returns>
    /// <exception cref="Exception"/>
    /// <remarks>May throw a little error if not careful</remarks>
    public static List<RadiumKeyValueStore<AtomicKeyTypes>> Use(
        List<AtomicNode> nodes, 
        List<RadiumKeyValueStore<AtomicKeyTypes>> store)
    {
        //int PreloadedVariableIndex = PreloadVariableLines(nodes);

        int TargetBinaryIndex = nodes.FindIndex(INode => INode.Value == "target");

        for (int index = 0; index < nodes.Count; index++)
        {
            AtomicNode node = nodes[index];

            // This should be a string
            // Array is below attribute checking
            if (index != 0
                && nodes[index].TokenType == AtomicLexerTokens.StringLiteral
                && VerifyStringIndex(index, nodes))
            {
                string AtomicKeyName = nodes[index - 2].Value;

                var StringValue = ParseString(node.Value);
                
                foreach (var kv in store)
                {
                    // The binary has no name yet so check if it doesnt then add the name
                    if (kv.Key.Equals("target") && kv.IsEmpty())
                    {
                        kv.Value.Add(ParseString(nodes[TargetBinaryIndex + 1].Value));
                    }
                    
                    if (kv.Key.Equals(AtomicKeyName))
                    {
                        kv.Value.Add(StringValue);
                    }
                }
            }

            // Take the token type of the node and match up to see if its an attribute
            if (node.TokenType == AtomicLexerTokens.AttributeKey)
            {
                RadiumKeyValueStore<AtomicKeyTypes> AttributeStore = new(node.Value)
                {
                    KeyStoreType = AtomicKeyTypes.Attribute
                };

                // Store the name of the value first
                store.Add(AttributeStore);

                // then get that value to add the attributes name
                // throw an exception if it cant be found (it will never happen in a million years)
                var SelectedStore = store.Find((PStore) => PStore.Key == node.Value) 
                    ?? throw new Exception($"{node.Value} is invalid!");

                SelectedStore.Value.Add(nodes[index + 1].Value);
            }

            // Array packaging
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