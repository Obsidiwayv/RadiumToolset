namespace RadiumCommon.Exceptions;

public class RadiumUnknownOS : Exception
{
    public RadiumUnknownOS() : base($"OS isnt supported by this program") {}
}