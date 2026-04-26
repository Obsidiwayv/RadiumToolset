namespace AtomicDSL;

public class AtomicConstants
{
    public static string FileExtention = ".atomic";
}

public enum AtomicLexerTokens
{
    Keyword, // Default token
    Symbol,
    StringLiteral,
    Bracket,
    AttributeKey,
    AttributeValue
}

public enum AtomicKeyTypes
{
    /// <summary>
    /// Define in the block of the projects target
    /// </summary>
    Block,

    /// <summary>
    /// Defined outside of the target using #variable_name variable
    /// </summary>
    Attribute
}