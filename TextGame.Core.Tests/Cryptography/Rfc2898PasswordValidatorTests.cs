using System.Security.Cryptography;
using TextGame.Core.Cryptography;

namespace TextGame.Core.Tests.Cryptography;

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
        Assert.False(validator.IsValid("123", result.Copy(initializationVector: CreateRandomBytes())));
        Assert.False(validator.IsValid("123", result.Copy(salt: CreateRandomBytes())));
        Assert.False(validator.IsValid("123", result.Copy(iterations: result.Iterations + 1)));
        Assert.False(validator.IsValid("123", result.Copy(data: Guid.NewGuid().ToString())));
    }

    private static byte[] CreateRandomBytes()
    {
        var bytes = new byte[8];

        var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(bytes);

        return bytes;
    }
}
