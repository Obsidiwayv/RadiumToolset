namespace AtomicDSL;

public class AtomicNode
{
    public AtomicLexerTokens TokenType { get; init; }
    public required string Value { get; init; }
}

public class AtomicLanguageNode(bool arr)
{
    public List<string> ArrayChildren = [];

    public List<KeyValuePair<string, string>> KeywordPair { get; set; } = [];

    public bool IsArray { get; } = arr;
}