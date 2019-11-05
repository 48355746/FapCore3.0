using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Fap.Core.Infrastructure.Encryption
{
    /// <summary>
	/// DES加密/解密类。
	/// </summary>
	public class EncryptionDes
    {
        private const string key1 = "5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTAJu3hN1m75Zr195ju7g9v4Hk=";
        private const string key2 = "5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/FAPJu3hN1m75Zr195ju7g9v4Hk=";
        private const string key3 = "5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjREGv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/FAPJu3hN1m75Zr195ju7g9v4Hk=";
        public EncryptionDes()
        {
        }

        #region ========加密======== 

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, key1);
        }

        public static string Encrypt2(string Text)
        {
            return Encrypt(Text, key2);
        }

        public static string Encrypt3(string Text)
        {
            return Encrypt(Text, key3);
        }

        public static string Encrypt(string input, string sKey)
        {

            byte[] inputArray = Encoding.UTF8.GetBytes(input);
            var tripleDES = TripleDES.Create();
            var byteKey = Encoding.UTF8.GetBytes(sKey.Md5().Substring(0, 16));
            byte[] allKey = new byte[24];
            Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
            Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
            tripleDES.Key = allKey;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }



        #endregion

        #region ========解密======== 


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, key1);
        }

        public static string Decrypt2(string Text)
        {
            return Decrypt(Text, key2);
        }

        public static string Decrypt3(string Text)
        {
            return Decrypt(Text, key3);
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string input, string sKey)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            var tripleDES = TripleDES.Create();
            var byteKey = Encoding.UTF8.GetBytes(sKey.Md5().Substring(0, 16));
            byte[] allKey = new byte[24];
            Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
            Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
            tripleDES.Key = allKey;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

    }
}
