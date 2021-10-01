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

            byte[] result = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                int n = data[i];

                for (int j = i % key.Length; j < key.Length + i; j++)
                {
                    switch ((j + i) % 2)
                    {
                        case 0:
                            n += key[j % key.Length];
                            
                            break;
                        case 1:
                            n -= key[j % key.Length];
                            break;
                    }
                }

                result[i] = (byte)(n % 256);
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

            byte[] result = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                int n = data[i];

                for (int j = (key.Length - 1) + i; j >= i % key.Length; j--)
                {
                    switch ((j + i) % 2)
                    {
                        case 0:
                            n -= key[j % key.Length];
                            break;
                        case 1:
                            n += key[j % key.Length];
                            break;
                    }
                }

                result[i] = (byte)(n % 256);
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
