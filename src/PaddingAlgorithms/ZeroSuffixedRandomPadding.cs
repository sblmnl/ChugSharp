using System;
using System.Security.Cryptography;

namespace ChugSharp.PaddingAlgorithms
{
    public class ZeroSuffixedRandomPadding : IChugPaddingAlgorithm
    {
        private int _blockSize;

        public int BlockSize
        {
            get => _blockSize;
            set
            {
                if (value < 2)
                    throw new ArgumentOutOfRangeException(nameof(BlockSize), "The block size cannot be less than 2!");

                _blockSize = value;
            }
        }

        public ZeroSuffixedRandomPadding(int blockSize)
        {
            BlockSize = blockSize;
        }

        /// <summary>
        /// Pads the data to a multiple of the block size with random non-zero bytes followed by a zero.
        /// </summary>
        /// <param name="data">The data to pad.</param>
        /// <returns>The data padded to a multiple of the block size.</returns>
        public byte[] Pad(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            int paddedLength = data.Length < BlockSize
                ? BlockSize
                : BlockSize * (data.Length / BlockSize + 1);

            byte[] paddedData = new byte[data.Length == paddedLength ? paddedLength + BlockSize : paddedLength];

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

        /// <summary>
        /// Removes the padding from the padded data.
        /// </summary>
        /// <param name="data">The padded data.</param>
        /// <returns>The unpadded data.</returns>
        public byte[] Unpad(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length % BlockSize != 0)
                throw new ArgumentException("The data length is not a multiple of the block size!", nameof(data));

            if (data.Length == 0)
                return new byte[0];

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
