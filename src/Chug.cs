using ChugSharp.PaddingAlgorithms;
using System;

namespace ChugSharp
{
    public class Chug
    {
        public bool UsePadding { get; set; }
        public IChugPaddingAlgorithm Padding { get; set; }

        public Chug()
        {
            UsePadding = false;
        }

        public Chug(bool usePadding)
        {
            UsePadding = usePadding;
            Padding = new LengthPrefixedRandomPadding(32);
        }

        public Chug(
            bool usePadding,
            IChugPaddingAlgorithm padding)
        {
            UsePadding = usePadding;
            Padding = padding;
        }

        /// <summary>
        /// Encrypts the data with the key.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key.</param>
        /// <returns>The data encrypted with the key.</returns>
        private byte[] Forward(byte[] data, byte[] key)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (key is null)
                throw new ArgumentNullException(nameof(key));

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

        /// <summary>
        /// Decrypts the data with the key.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="key">The key.</param>
        /// <returns>The data decrypted with the key.</returns>
        private byte[] Reverse(byte[] data, byte[] key)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (data.Length % 4 != 0)
                throw new ArgumentException("The data length is not a multiple of 4!", nameof(data));

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

        /// <summary>
        /// Encrypts the data with the key.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key.</param>
        /// <returns>The data encrypted with the key.</returns>
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            if (UsePadding)
            {
                if (Padding is null)
                    throw new ArgumentNullException(nameof(Padding));

                return Forward(Padding.Pad(data), key);
            }

            return Forward(data, key);
        }

        /// <summary>
        /// Decrypts the data with the key.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="key">The key.</param>
        /// <returns>The data decrypted with the key.</returns>
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            if (UsePadding)
            {
                if (Padding is null)
                    throw new ArgumentNullException(nameof(Padding));

                return Padding.Unpad(Reverse(data, key));
            }

            return Reverse(data, key);
        }
    }
}
