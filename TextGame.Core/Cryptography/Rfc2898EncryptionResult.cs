namespace TextGame.Core.Cryptography;

public readonly record struct Rfc2898EncryptionResult(
    byte[] InitializationVector,
    byte[] Salt,
    int Iterations,
    string Data,
    byte[] CipherBytes);

