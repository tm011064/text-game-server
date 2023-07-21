﻿namespace TextGame.Core.Cryptography;

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class Rfc2898PasswordEncryptor
{
    private readonly UTF8Encoding encoding = new(false);

    public Rfc2898EncryptionResult Encrypt(string password)
    {
        var data = Guid.NewGuid().ToString();
        var salt = CreateRandomBytes();
        var iterations = 100_000;

        var keyGenerator = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);

        var algorithm = Aes.Create();

        algorithm.Key = keyGenerator.GetBytes(16);

        using var encryptionStream = new MemoryStream();
        using var encrypt = new CryptoStream(encryptionStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write);

        var textBytes = encoding.GetBytes(data);

        encrypt.Write(textBytes, 0, textBytes.Length);

        encrypt.FlushFinalBlock();
        encrypt.Close();

        var cipherBytes = encryptionStream.ToArray();

        keyGenerator.Reset();

        return new Rfc2898EncryptionResult(
            InitializationVector: algorithm.IV,
            Salt: salt,
            Iterations: iterations,
            Data: data,
            CipherBytes: cipherBytes);
    }

    private byte[] CreateRandomBytes()
    {
        var salt = new byte[8];

        var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(salt);

        return salt;
    }
}

