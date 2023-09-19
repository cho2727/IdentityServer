using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace IdentityServer.Shared.Utility;

public static class JwtTokenConfigHelper
{
    private static RSA _encryptionKey = RSA.Create(3072); // public key for encryption, private key for decryption
    private static ECDsa? _signingKey = ECDsa.Create(ECCurve.NamedCurves.nistP256); // private key for signing, public key for validation

    private static string _encryptionKid = "8524e3e6674e494f85c5c775dcd602c5";
    private static string _signingKid = "29b4adf8bcc941dc8ce40a6d0227b6d3";

    public static RsaSecurityKey PrivateEncryptionKey = new RsaSecurityKey(_encryptionKey) { KeyId = _encryptionKid };
    public static RsaSecurityKey PublicEncryptionKey = new RsaSecurityKey(_encryptionKey.ExportParameters(false)) { KeyId = _encryptionKid };
    public static ECDsaSecurityKey PrivateSigningKey = new ECDsaSecurityKey(_signingKey) { KeyId = _signingKid };
    public static ECDsaSecurityKey PublicSigningKey = new ECDsaSecurityKey(ECDsa.Create(_signingKey.ExportParameters(false))) { KeyId = _signingKid };
}
