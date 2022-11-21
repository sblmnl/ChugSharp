using System;
using System.Security.Cryptography;

namespace ChugSharp.PaddingAlgorithms;

public class LengthPrefixedRandomPadding : IChugPaddingAlgorithm
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

    public LengthPrefixedRandomPadding(int blockSize)
    {
        BlockSize = blockSize;
    }

    /// <summary>
    /// Builds the padding length prefix based on the padding length.
    /// </summary>
    /// <param name="paddingLength">The padding length.</param>
    /// <returns>The padding length prefix.</returns>
    private byte[] GetPrefix(int paddingLength)
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

    /// <summary>
    /// Determines the number of bytes used to store the padding length prefix.
    /// </summary>
    /// <param name="dataLength">The length of the padded data.</param>
    /// <returns>The number of bytes used to store the padding length prefix.</returns>
    private byte GetPrefixLength(int dataLength)
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

    /// <summary>
    /// Pads the data to a multiple of the block size with random bytes prefixed by the number of random bytes.
    /// </summary>
    /// <param name="data">The data to pad.</param>
    /// <returns>The padded data.</returns>
    public byte[] Pad(byte[] data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        int paddedLength = data.Length < BlockSize
            ? BlockSize
            : BlockSize * (data.Length / BlockSize + 1);

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
