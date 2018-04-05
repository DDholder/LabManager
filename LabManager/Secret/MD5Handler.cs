using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Secret
{
    public enum Permission
    {
        All=0,
        Guest=1
    }
    public class Md5Handler
    {
        public static string getMD5(string key)
        {
            MD5 mD5 = MD5.Create();
            var result = mD5.ComputeHash(Encoding.Default.GetBytes(key));
            return Convert.ToBase64String(result);
        }
    }
}
