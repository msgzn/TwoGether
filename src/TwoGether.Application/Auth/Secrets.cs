using System.Security.Cryptography;
using System.Text;

namespace TwoGether.Application.Auth;

public static class Secrets
{
    public const int CodeLength = 6;

    public static string GenerateCode() =>
        RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, CodeLength)).ToString($"D{CodeLength}");

    public static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static string Hash(string secret) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));
}
