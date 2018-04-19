using System;
using System.Security.Cryptography;
using System.Text;

namespace LabManager.Secret
{
    public enum Permission
    {
        All=0,
        Guest=1
    }
    public class Md5Handler
    {
        public static string GetMD5(string key) 
        {
            MD5 mD5 = MD5.Create();
            var result = mD5.ComputeHash(Encoding.Default.GetBytes(key));
            return Convert.ToBase64String(result);
        }
    }
}
