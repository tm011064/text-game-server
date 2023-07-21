namespace TextGame.Core.Cryptography;

using System.IO;
using System.Text;
using System.Security.Cryptography;
using TextGame.Data.Contracts;

public class Rfc2898PasswordValidator
{
    private readonly UTF8Encoding encoding = new(false);

    public bool IsValid(string password, UserPassword userPassword)
    {
        try
        {
            var data = Decrypt(password, userPassword);

            return string.Equals(data, userPassword.Data);
        }
        catch
        {
            return false;
        }
    }

    private string Decrypt(string password, UserPassword userPassword)
    {
        var keyGenerator = new Rfc2898DeriveBytes(password, userPassword.Salt, userPassword.Iterations, HashAlgorithmName.SHA256);

        var algorithm = Aes.Create();

        algorithm.Key = keyGenerator.GetBytes(16);
        algorithm.IV = userPassword.InitializationVector;

        using var decryptionStreamBacking = new MemoryStream();
        using var decrypt = new CryptoStream(decryptionStreamBacking, algorithm.CreateDecryptor(), CryptoStreamMode.Write);

        decrypt.Write(userPassword.CipherBytes, 0, userPassword.CipherBytes.Length);

        decrypt.Flush();
        decrypt.Close();

        keyGenerator.Reset();

        return encoding.GetString(decryptionStreamBacking.ToArray());
    }
}

