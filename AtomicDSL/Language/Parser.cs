namespace AtomicDSL.Language;

public class AtomicParser
{
    public static AtomicOutput Use(List<AtomicNode> nodes)
    {
        AtomicOutput output = new();
        for (int index = 0; index < nodes.Count; index++)
        {
            AtomicNode node = nodes[index];

            if (nodes.Count > index
                && nodes[index - 1].TokenType == AtomicLexerTokens.StringLiteral)
            {
                
            }

            if (node.TokenType == AtomicLexerTokens.Bracket
                && nodes.Count > index
                // Make sure THIS is not a target
                && nodes[index - 1].TokenType != AtomicLexerTokens.StringLiteral
                && node.Value == "{")
            {
                AtomicNode keyType = nodes[index - 2];
                for (int sIndex = 0 + index; sIndex < nodes.Count; sIndex++)
                {
                    AtomicNode subNode = nodes[sIndex];

                    if (subNode.Value == "}") break;
                    SetOutput(
                        keyType.Value, 
                        subNode.Value, 
                        output);
                }
            }
        }
        return output;
    }

    public static void SetOutput(
        string input, 
        string data, 
        AtomicOutput output)
    {
        if (input == "includes")
        {
            output.Includes.Add(data);
        }
    }
}