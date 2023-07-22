namespace TextGame.Data.Contracts;

public class UserPassword
{
    internal UserPassword()
    {
    }

    public UserPassword(byte[] initializationVector, byte[] salt, int iterations, string data, byte[] cipherBytes)
    {
        InitializationVector = initializationVector;
        Salt = salt;
        Iterations = iterations;
        Data = data;
        CipherBytes = cipherBytes;
    }

    public byte[] InitializationVector { get; internal set; }

    public byte[] Salt { get; internal set; }

    public int Iterations { get; internal set; }

    public string Data { get; internal set; }

    public byte[] CipherBytes { get; internal set; }
}
