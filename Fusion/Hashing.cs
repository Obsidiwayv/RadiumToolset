using System.Security.Cryptography;
using System.Text;

namespace Fusion;

public class FusionHash
{
    /// <summary>
    /// Takes a string, ex. a file, converts it into a SHA256 and returns it in A Hexadecimal Format <br/>
    /// This is just a wrapper around <seealso cref="ToHash"/>
    /// </summary>
    public static string ToHashString(string contents)
    {
        return Convert.ToHexString(ToHash(contents));
    }

    public static byte[] ToHash(string contents)
    {
        byte[] MessageBytes = Encoding.UTF8.GetBytes(contents);
        return SHA256.HashData(MessageBytes);
    }

    public static bool Compare(string hash, string contents)
    {
        byte[] ConvertedContentsHash = Convert.FromHexString(contents);
        byte[] OriginalByteHash = Convert.FromHexString(hash);
        return OriginalByteHash.SequenceEqual(ConvertedContentsHash);
    }
}