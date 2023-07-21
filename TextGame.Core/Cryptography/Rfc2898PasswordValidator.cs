namespace TextGame.Core.Cryptography;

using System.IO;
using System.Text;
using System.Security.Cryptography;

public class Rfc2898PasswordValidator
{
    private readonly UTF8Encoding encoding = new(false);

    public bool IsValid(string password, Rfc2898EncryptionResult result)
    {
        try
        {
            var data = Decrypt(password, result);

            return string.Equals(data, result.Data);
        }
        catch
        {
            return false;
        }
    }

    private string Decrypt(string password, Rfc2898EncryptionResult encryptionResult)
    {
        var keyGenerator = new Rfc2898DeriveBytes(password, encryptionResult.Salt, encryptionResult.Iterations, HashAlgorithmName.SHA256);

        var algorithm = Aes.Create();

        algorithm.Key = keyGenerator.GetBytes(16);
        algorithm.IV = encryptionResult.InitializationVector;

        using var decryptionStreamBacking = new MemoryStream();
        using var decrypt = new CryptoStream(decryptionStreamBacking, algorithm.CreateDecryptor(), CryptoStreamMode.Write);

        decrypt.Write(encryptionResult.CipherBytes, 0, encryptionResult.CipherBytes.Length);

        decrypt.Flush();
        decrypt.Close();

        keyGenerator.Reset();

        return encoding.GetString(decryptionStreamBacking.ToArray());
    }
}

