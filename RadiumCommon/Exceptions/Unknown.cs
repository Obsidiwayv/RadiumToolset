namespace RadiumCommon.Exceptions;

public class RadiumUnknownOS : Exception
{
    public RadiumUnknownOS() : base($"current OS isnt supported by this program") {}
}