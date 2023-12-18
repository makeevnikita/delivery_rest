using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;



namespace delivery.Helpers;

public static class AuthHelper
{
    public static readonly byte[] keyHmac256 = Encoding.UTF8.GetBytes("T!0XKKZCF0EU_*LW#%P4!09!^SULRG4KYK8R7U!J!H&5UV0_AQENECY$B91@!N4");
    public static SymmetricSecurityKey GetKey() => new SymmetricSecurityKey(keyHmac256);
}
