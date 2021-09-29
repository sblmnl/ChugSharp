using ChugSharp.PaddingAlgorithms;
using System;

namespace ChugSharp
{
    public class Chug
    {
        public ChugPaddingAlgorithm Padding { get; set; }
        public bool UsePadding { get; set; }
        public int BlockSize { get; set; }
        
        public Chug(ChugPaddingAlgorithm padding = ChugPaddingAlgorithm.LengthPrefixedRandom, int blockSize = 0, bool usePadding = false)
        {
            Padding = padding;
            BlockSize = blockSize;
            UsePadding = usePadding;
        }

        private byte[] Forward(byte[] data, byte[] key)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (key is null)
                throw new ArgumentNullException(nameof(key), "The key cannot be null!");

            byte[] result = new byte[data.Length * 4];

            for (int i = 0; i < data.Length; i++)
            {
                float value = data[i];

                for (int j = 0; j < key.Length; j++)
                {
                    if (key[j] == 0)
                        continue;

                    switch (j % 4)
                    {
                        case 0:
                            value += key[j];
                            break;
                        case 1:
                            value -= key[j];
                            break;
                        case 2:
                            value *= key[j];
                            break;
                        case 3:
                            value /= key[j];
                            break;
                    }

                    byte[] buffer = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(buffer);

                    Buffer.BlockCopy(buffer, 0, result, i * 4, 4);
                }
            }

            return result;
        }

        private byte[] Reverse(byte[] data, byte[] key)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (key is null)
                throw new ArgumentNullException(nameof(key), "The key cannot be null!");

            byte[] result = new byte[data.Length / 4];

            for (int i = 0; i < data.Length; i += 4)
            {
                byte[] buffer = new byte[4];
                Buffer.BlockCopy(data, i, buffer, 0, 4);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);

                float value = BitConverter.ToSingle(buffer, 0);

                for (int j = key.Length - 1; j >= 0; j--)
                {
                    if (key[j] == 0)
                        continue;

                    switch (j % 4)
                    {
                        case 0:
                            value -= key[j];
                            break;
                        case 1:
                            value += key[j];
                            break;
                        case 2:
                            value /= key[j];
                            break;
                        case 3:
                            value *= key[j];
                            break;
                    }
                }

                result[i / 4] = (byte)value;
            }

            return result;
        }

        public byte[] Encrypt(byte[] data, byte[] key)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (key is null)
                throw new ArgumentNullException(nameof(key), "The key cannot be null!");

            if (UsePadding)
            {
                switch (Padding)
                {
                    case ChugPaddingAlgorithm.LengthPrefixedRandom:
                        return Forward(LengthPrefixedRandom.Pad(data, BlockSize), key);
                    case ChugPaddingAlgorithm.ZeroSuffixedRandom:
                        return Forward(ZeroSuffixedRandom.Pad(data, BlockSize), key);
                    default:
                        return Forward(LengthPrefixedRandom.Pad(data, BlockSize), key);
                }
            }

            return Forward(data, key);
        }

        public byte[] Decrypt(byte[] data, byte[] key)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "The data cannot be null!");

            if (key is null)
                throw new ArgumentNullException(nameof(key), "The key cannot be null!");

            if (UsePadding)
            {
                switch (Padding)
                {
                    case ChugPaddingAlgorithm.LengthPrefixedRandom:
                        return LengthPrefixedRandom.Unpad(Reverse(data, key), BlockSize);
                    case ChugPaddingAlgorithm.ZeroSuffixedRandom:
                        return ZeroSuffixedRandom.Unpad(Reverse(data, key), BlockSize);
                    default:
                        return LengthPrefixedRandom.Unpad(Reverse(data, key), BlockSize);
                }
            }

            return Reverse(data, key);
        }
    }
}
