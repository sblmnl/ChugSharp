namespace ChugSharp.PaddingAlgorithms
{
    public interface IChugPaddingAlgorithm
    {
        int BlockSize { get; set; }

        byte[] Pad(byte[] data);
        byte[] Unpad(byte[] data);
    }
}