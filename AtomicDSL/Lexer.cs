using System.Text;
using RadiumCommon;

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


            // Match up the hashtag
            if (pos == '#')
            {
                StringBuilder AttributeString = new();
                BasicKeyValue<string, string> AttributeKeyValue = new();
                index++;
                // If its a hashtag then we loop over to verify that it has an attribute attached
                for (int subIndex = 0 + index; subIndex < charArray.Length; subIndex++)
                {
                    // WE have to make sure if we ever come across a whitespace then it shouldnt have a newline
                    // then an exception would be thrown
                    if (charArray[subIndex] == ' ')
                    {
                        AttributeKeyValue.Key = AttributeString.ToString();
                        AttributeString.Clear();
                    } 

                    // this time the variable is no longer unknown to us
                    if (charArray[subIndex] == '\n') 
                    {
                        AttributeKeyValue.Value = AttributeString.ToString();
                        if (charArray[subIndex] == '\n' && AttributeString.Length == 0)
                        {
                            // we already have the name of the attribute
                            throw new Exception($"Attribute {AttributeString} has no variable!");
                        }
                        // were done parsing this section
                        index++;
                        if (AttributeKeyValue.Key != null && AttributeKeyValue.Value != null)
                        {
                            tokens.Add(new()
                            {
                                TokenType = AtomicLexerTokens.AttributeKey,
                                Value = AttributeKeyValue.Key
                            });
                            tokens.Add(new()
                            {
                                TokenType = AtomicLexerTokens.AttributeValue,
                                Value = AttributeKeyValue.Value.Trim()
                            });
                        }
                        break;
                    }
                    index++;
                    AttributeString.Append(charArray[subIndex]);
                }
            }

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
                if (pos != '"' && pos != '#')
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