namespace TextGame.Data.Contracts;

public class UserPassword
{
    internal UserPassword()
    {
    }

    public UserPassword(
        byte[] initializationVector,
        byte[] salt,
        int iterations,
        string data,
        byte[] cipherBytes)
    {
        InitializationVector = initializationVector;
        Salt = salt;
        Iterations = iterations;
        Data = data;
        CipherBytes = cipherBytes;
    }

    public byte[] InitializationVector { get; internal set; } = null!;

    public byte[] Salt { get; internal set; } = null!;

    public int Iterations { get; internal set; }

    public string Data { get; internal set; } = null!;

    public byte[] CipherBytes { get; internal set; } = null!;

    public UserPassword Copy(
        byte[]? initializationVector = null,
        byte[]? salt = null,
        int? iterations = null,
        string? data = null,
        byte[]? cipherBytes = null)
    {
        return new UserPassword(
             initializationVector ?? InitializationVector,
             salt ?? Salt,
             iterations ?? Iterations,
             data ?? Data,
             cipherBytes ?? CipherBytes);
    }
}
