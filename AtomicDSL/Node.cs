namespace AtomicDSL;

public class AtomicNode
{
    public AtomicLexerTokens TokenType { get; init; }
    public required string Value { get; init; }
}