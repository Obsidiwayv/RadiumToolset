using System.Security.Cryptography;
using System.Text;
using RadiumCommon;

namespace Fusion;

public class FusionHash(FusionCompilationStep Step)
{
    public readonly Dictionary<string, string> FileHashes = [];

    public void ComputeHash(string filePath)
    {
        string FileContentHash = ToHashString(File.ReadAllText(filePath));
        FileHashes.Add(filePath, FileContentHash);
    }

    public void VerifyHashes(string proj)
    {
        Dictionary<string, string> HashKeys = [];
        foreach (var (file, hash) in FileHashes)
        {
            // If the file doesnt exist then we shouldnt count this one in the dictionary
            if (!File.Exists(file))
            {
                RadiumLogger.Write($"%rFile Deleted%c >> {file}");
                continue;
            }
            // Add the keys to the new dictionary for writing
            HashKeys.Add(file, hash);
        }
        Step.SourceFileFactory.WriteHashFile(HashKeys, proj);
    }

    public string CheckHash(string filePath, Dictionary<string, string> hashDict)
    {
        hashDict.TryGetValue(filePath, out string? hash);
        FileHashes.TryGetValue(filePath, out string? newHash);
        if (hash != null && newHash != null)
        {
            if (Compare(hash, newHash))
            {
                return "%gOriginal%c";
            } else
            {
                return "%rUpdated%c";
            }
        } else
        {
            return "%rAdded%c";
        }
    }
    
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