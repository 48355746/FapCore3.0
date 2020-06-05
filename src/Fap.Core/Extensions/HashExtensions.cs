using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class HashExtensions
    {
        /// <summary>
        /// Creates a MD5 hash of the specified input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5(this string input, string salt = "")
        {
            if (input.IsMissing())
            {
                return string.Empty;
            }
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes($"*&YY!!@~{input}++!#{salt}");
                bytes = md5.ComputeHash(bytes);

                return Convert.ToBase64String(bytes);
            }

        }
        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha256(this string input)
        {
            if (input.IsMissing()) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash.</returns>
        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
            {
                return null;
            }

            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(input);
            }
        }

        /// <summary>
        /// Creates a SHA512 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha512(this string input)
        {
            if (input.IsMissing()) return string.Empty;

            using (var sha = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        public static string ToBase64String(this string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }
        public static string FromBase64String(this string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
    }
}
