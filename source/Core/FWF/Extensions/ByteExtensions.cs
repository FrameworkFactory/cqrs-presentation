using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace FWF
{
    [DebuggerStepThrough]
    public static class ByteExtensions
    {

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool IsEqualByte(this byte[] original, byte[] compare)
        {
            // Same instance
            if (ReferenceEquals(original, compare))
            {
                return true;
            }

            // Both null
            if (original == null && compare == null)
            {
                return true;
            }

            // Only one is null
            if (original == null)
            {
                return false;
            }
            if (compare == null)
            {
                return false;
            }

            // Different length
            if (original.Length != compare.Length)
            {
                return false;
            }

            var rc = 0;

            for (int i = 0; i < original.Length; i++)
            {
                rc = rc | (original[i] ^ compare[i]);

                if (rc != 0)
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] FromHex(this string data)
        {
            if (data.IsMissing())
            {
                return null;
            }

            if (data.StartsWith("0x"))
            {
                data = data.Substring(2);
            }

            var getHexValue = new Func<char,int>( (c) => 
            {
                var val = (int)c;
                return val - (val < 58 ? 48 : 55);
            });

            var hexAsBytes = new byte[data.Length / 2];
            for (int i = 0; i < hexAsBytes.Length; i++)
            {
                hexAsBytes[i] = (byte)((getHexValue(data[i << 1]) << 4) + getHexValue(data[(i << 1) + 1]));
            }

            return hexAsBytes;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static string ToHex(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var hex = new char[data.Length * 2];

            for (int iter = 0; iter < data.Length; iter++)
            {
                var hexChar = ((byte)(data[iter] >> 4));
                hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
                hexChar = ((byte)(data[iter] & 0xF));
                hex[(iter * 2) + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
            }

            return string.Concat("0x", new string(hex));
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            byte[] result;
            using (SHA256 sha = SHA256.Create())
            {
                result = sha.ComputeHash(input);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static byte[] Concat(this byte[] input, byte[] other)
        {
            var newData = new byte[input.Length + other.Length];

            Buffer.BlockCopy(
                input,
                0,
                newData,
                0,
                input.Length
                );

            Buffer.BlockCopy(
                other,
                0,
                newData,
                0,
                other.Length
                );

            return newData;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static byte[] Concat(
            this byte[] input,
            int inputOffset,
            int inputLength,
            byte[] other,
            int otherOffset,
            int otherLength
            )
        {
            var totalSize = (inputLength + otherLength);

            var newData = new byte[totalSize];

            Buffer.BlockCopy(
                input,
                inputOffset,
                newData,
                0,
                inputLength
                );

            Buffer.BlockCopy(
                other,
                otherOffset,
                newData,
                inputLength,
                otherLength
                );

            return newData;
        }


    }
}



