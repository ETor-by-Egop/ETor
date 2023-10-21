namespace ETor.Client.Gl;

public class TextureFileData
{
    public int Width { get; set; }
    public int Height { get; set; }

    public byte[] RgbaPixels { get; set; }

    public TextureFileData(int width, int height, byte[] rgbaPixels)
    {
        Width = width;
        Height = height;
        RgbaPixels = rgbaPixels;
    }
}