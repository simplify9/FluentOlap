using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SW.FluentOlap.Utilities
{
    public static class Hashing
    {
        public static string byteArrToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        public static string HashTypeMaps(IDictionary<string, NodeProperties> typeMaps)
        {
            using (var sha256 = SHA256.Create())
            {
                string mapString = typeMaps.ToString();
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(mapString));
                return byteArrToString(hash);
            }
        }
    }
}
