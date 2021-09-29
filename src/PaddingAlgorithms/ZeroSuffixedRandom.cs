using System;
using System.Security.Cryptography;

namespace ChugSharp.PaddingAlgorithms
{
    public static class ZeroSuffixedRandom
    {
        public static byte[] Pad(byte[] data, int blockSize)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (blockSize < 1)
                throw new ArgumentOutOfRangeException(nameof(blockSize), "The block size cannot be less than 1.");

            int paddedLength = data.Length < blockSize
                ? blockSize
                : blockSize * (data.Length / blockSize + 1);

            byte[] paddedData = new byte[data.Length == paddedLength ? paddedLength + blockSize : paddedLength];

            int paddingLength = paddedLength - 1 - data.Length;

            if (paddingLength != 0)
            {
                using var rng = new RNGCryptoServiceProvider();
                byte[] padding = new byte[paddingLength];
                rng.GetNonZeroBytes(padding);

                Buffer.BlockCopy(padding, 0, paddedData, 0, padding.Length);
            }

            Buffer.BlockCopy(data, 0, paddedData, paddingLength + 1, data.Length);

            return paddedData;
        }

        public static byte[] Unpad(byte[] data, int blockSize)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (blockSize < 1)
                throw new ArgumentOutOfRangeException(nameof(blockSize), "The block size cannot be less than 1.");

            if (data.Length % blockSize != 0)
                throw new ArgumentException("Invalid data or block size!");

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    int offset = i + 1;
                    byte[] unpaddedData = new byte[data.Length - offset];
                    Buffer.BlockCopy(data, offset, unpaddedData, 0, unpaddedData.Length);
                    return unpaddedData;
                }
            }

            return data;
        }
    }
}
