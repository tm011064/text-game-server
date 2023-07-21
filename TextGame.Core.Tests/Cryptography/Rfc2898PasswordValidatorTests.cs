namespace TextGame.Core.Tests.Cryptography;

using System.Security.Cryptography;
using TextGame.Core.Cryptography;

public class Rfc2898PasswordValidatorTests
{
    [Fact]
    public void Valid()
    {
        var encryptor = new Rfc2898PasswordEncryptor();
        var validator = new Rfc2898PasswordValidator();

        var password = "123";

        var result = encryptor.Encrypt(password);

        Assert.True(validator.IsValid(password, result));

        Assert.False(validator.IsValid("1234", result));
        Assert.False(validator.IsValid("123", result with { InitializationVector = CreateRandomBytes() }));
        Assert.False(validator.IsValid("123", result with { Salt = CreateRandomBytes() }));
        Assert.False(validator.IsValid("123", result with { Iterations = result.Iterations + 1 }));
        Assert.False(validator.IsValid("123", result with { Data = Guid.NewGuid().ToString() }));
    }

    private static byte[] CreateRandomBytes()
    {
        var bytes = new byte[8];

        var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(bytes);

        return bytes;
    }
}
