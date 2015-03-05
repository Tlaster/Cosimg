using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace CosImg.Common
{
    public static class StringExtension
    {
        public static string GetHashedString(this string str)
        {
            Windows.Storage.Streams.IBuffer buff_utf8 = CryptographicBuffer.ConvertStringToBinary(str, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
            HashAlgorithmProvider algprov = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            Windows.Storage.Streams.IBuffer buff_hash = algprov.HashData(buff_utf8);
            return CryptographicBuffer.EncodeToHexString(buff_hash);
        }

    }
}
