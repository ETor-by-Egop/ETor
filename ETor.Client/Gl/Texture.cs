using Silk.NET.OpenGL;

namespace ETor.Client.Gl
{
    public enum TextureCoordinate
    {
        S = TextureParameterName.TextureWrapS,
        T = TextureParameterName.TextureWrapT,
        R = TextureParameterName.TextureWrapR
    }

    public class Texture : IDisposable
    {
        public const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)GLEnum.Srgb8Alpha8;
        public const SizedInternalFormat Rgb32F = (SizedInternalFormat)GLEnum.Rgb32f;

        public const GLEnum MaxTextureMaxAnisotropy = (GLEnum)0x84FF;

        public static float? MaxAniso;
        private readonly GL _gl;
        public readonly string Name;
        public readonly uint GlTexture;
        public readonly uint Width, Height;
        public readonly uint MipmapLevels;
        public readonly SizedInternalFormat InternalFormat;

        public Texture(GL gl, int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
        {
            _gl            =   gl;
            MaxAniso       ??= gl.GetFloat(MaxTextureMaxAnisotropy);
            Width          =   (uint)width;
            Height         =   (uint)height;
            InternalFormat =   srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
            MipmapLevels   =   (uint)(generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2)));

            GlTexture = _gl.GenTexture();
            Bind();

            _gl.TexStorage2D(GLEnum.Texture2D, MipmapLevels, InternalFormat, Width, Height);
            _gl.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            if (generateMipmaps) _gl.GenerateTextureMipmap(GlTexture);

            SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
            SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
        }

        public Texture(GL gl, int width, int height, Memory<byte> data,  bool generateMipmaps = false, bool srgb = false)
        {
            _gl            =   gl;
            MaxAniso       ??= gl.GetFloat(MaxTextureMaxAnisotropy);
            Width          =   (uint)width;
            Height         =   (uint)height;
            InternalFormat =   srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
            MipmapLevels   =   (uint)(generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2)));

            GlTexture = _gl.GenTexture();
            Bind();

            _gl.TexStorage2D(GLEnum.Texture2D, MipmapLevels, InternalFormat, Width, Height);
            using (var pin = data.Pin())
            {
                unsafe
                {
                    _gl.TexSubImage2D(
                        GLEnum.Texture2D,
                        0,
                        0,
                        0,
                        Width,
                        Height,
                        PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        pin.Pointer
                    );
                }
            }

            if (generateMipmaps) _gl.GenerateTextureMipmap(GlTexture);

            SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
            SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
        }

        public Texture(GL gl, int width, int height, Memory<byte> data, PixelFormat pixelFormat, bool generateMipmaps = false, bool srgb = false)
        {
            _gl            =   gl;
            MaxAniso       ??= gl.GetFloat(MaxTextureMaxAnisotropy);
            Width          =   (uint)width;
            Height         =   (uint)height;
            InternalFormat =   srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
            MipmapLevels   =   (uint)(generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2)));

            GlTexture = _gl.GenTexture();
            Bind();

            _gl.TexStorage2D(GLEnum.Texture2D, MipmapLevels, InternalFormat, Width, Height);
            using (var pin = data.Pin())
            {
                unsafe
                {
                    _gl.TexSubImage2D(
                        GLEnum.Texture2D,
                        0,
                        0,
                        0,
                        Width,
                        Height,
                        pixelFormat,
                        PixelType.UnsignedByte,
                        pin.Pointer
                    );
                }
            }

            if (generateMipmaps) _gl.GenerateTextureMipmap(GlTexture);

            SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
            SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
        }

        public unsafe void UpdateData(Memory<byte> data, PixelFormat pixelFormat)
        {
            Bind();

            using (var pin = data.Pin())
            {
                _gl.TexSubImage2D(
                    GLEnum.Texture2D,
                    0,
                    0,
                    0,
                    Width,
                    Height,
                    pixelFormat,
                    PixelType.UnsignedByte,
                    pin.Pointer
                );
            }
        }

        public unsafe Memory<byte> DownloadData()
        {
            Bind();

            Memory<byte> bytes = new byte[Width * Height * 4];
            using (var pin = bytes.Pin())
            {
                _gl.GetTexImage(
                    TextureTarget.Texture2D,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    pin.Pointer
                );
            }

            return bytes;
        }

        public void Bind()
        {
            _gl.BindTexture(GLEnum.Texture2D, GlTexture);
        }

        public void SetMinFilter(TextureMinFilter filter)
        {
            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
        }

        public void SetMagFilter(TextureMagFilter filter)
        {
            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);
        }

        // public void SetAnisotropy(float level)
        // {
        //     const TextureParameterName textureMaxAnisotropy = (TextureParameterName)0x84FE;
        //     _gl.TexParameter(GLEnum.Texture2D, (GLEnum)textureMaxAnisotropy, Util.Clamp(level, 1, MaxAniso.GetValueOrDefault()));
        // }

        public void SetLod(int @base, int min, int max)
        {
            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureLodBias, @base);
            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMinLod, min);
            _gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLod, max);
        }

        public void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
        {
            _gl.TexParameterI(GLEnum.Texture2D, (TextureParameterName)coord, (int)mode);
        }

        public void Dispose()
        {
            _gl.DeleteTexture(GlTexture);
        }
    }
}