using System;
using System.Security.Cryptography;

namespace ChugSharp.PaddingAlgorithms
{
    public static class LengthPrefixedRandom
    {
        private static byte[] GetPrefix(int paddingLength)
        {
            if (paddingLength <= byte.MaxValue)
            {
                return new byte[] { (byte)(paddingLength - 1) };
            }

            if (paddingLength <= short.MaxValue)
            {
                return BitConverter.GetBytes((short)(paddingLength - 2));
            }

            if (paddingLength <= int.MaxValue)
            {
                return BitConverter.GetBytes(paddingLength - 4);
            }

            return null;
        }

        private static byte GetPrefixLength(int dataLength)
        {
            if (dataLength <= ((byte.MaxValue + 1) * 2) + 1)
            {
                return 1;
            }

            if (dataLength <= ((short.MaxValue + 1) * 2) + 2)
            {
                return 2;
            }

            return 4;
        }

        public static byte[] Pad(byte[] data, int blockSize)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (blockSize < 2)
                throw new ArgumentOutOfRangeException(nameof(blockSize), "The block size cannot be less than 2.");

            int paddedLength = data.Length < blockSize
                ? blockSize - data.Length
                : blockSize * (data.Length / blockSize + 1);

            byte[] paddedData = new byte[paddedLength];
            byte[] paddingLengthPrefix = GetPrefix(paddedLength - data.Length);

            Buffer.BlockCopy(paddingLengthPrefix, 0, paddedData, 0, paddingLengthPrefix.Length);

            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(
                data: paddedData,
                offset: paddingLengthPrefix.Length, 
                count: paddedLength - data.Length - paddingLengthPrefix.Length);

            Buffer.BlockCopy(data, 0, paddedData, paddedData.Length - data.Length, data.Length);

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

            byte prefixLength = GetPrefixLength(data.Length);
            int paddingLength = 0;

            switch (prefixLength)
            {
                case 1:
                    paddingLength = data[0];
                    break;
                case 2:
                    paddingLength = BitConverter.ToInt16(data, 0);
                    break;
                case 4:
                    paddingLength = BitConverter.ToInt32(data, 0);
                    break;
            }

            byte[] unpaddedData = new byte[data.Length - (prefixLength + paddingLength)];

            Buffer.BlockCopy(data, paddingLength + 1, unpaddedData, 0, unpaddedData.Length);

            return unpaddedData;
        }
    }
}
