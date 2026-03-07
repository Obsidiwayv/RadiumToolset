using System.Text;

namespace AtomicDSL;

public class AtomicLexer
{
    private static Dictionary<string[], AtomicLexerTokens> TokenMap = new()
    {
        { ["{", "}"], AtomicLexerTokens.Bracket },
        { ["="], AtomicLexerTokens.Symbol }
    };
    public static List<AtomicNode> Run(char[] charArray)
    {
        AtomicPosition position = new(0, 0);
        StringBuilder str = new();
        List<AtomicNode> tokens = [];

        for (int index = 0; index < charArray.Length; index++)
        {
            char pos = charArray[index];

            if (pos == '\n')
            {
                position.Row += 1;
                position.Column = 0;
            }
            else
            {
                position.Column += 1;
            }

            if (char.IsWhiteSpace(pos))
            {
                if (string.IsNullOrWhiteSpace(str.ToString()))
                {
                    continue;
                }
                var token = TokenMap.FirstOrDefault(element =>
                    element.Key.Contains(str.ToString()));
                tokens.Add(new()
                {
                    Value = str.ToString(),
                    TokenType = token.Value
                });
                str.Clear();
                continue;
            } else
            {
                if (pos != '"')
                {
                    str.Append(pos);
                }
            }

            if (pos == '"')
            {
                index++;
                StringBuilder subStr = new();
                for (int subIndex = 0 + index; subIndex < charArray.Length; subIndex++)
                {
                    char subPos = charArray[subIndex];
                    if (subPos == '"')
                    {
                        tokens.Add(new()
                        {
                            Value = subStr.ToString(),
                            TokenType = AtomicLexerTokens.StringLiteral
                        });
                        subStr.Clear();
                        break;
                    }
                    index++;
                    subStr.Append(charArray[subIndex]);
                }
            } 
        }

        return tokens;
    }
}