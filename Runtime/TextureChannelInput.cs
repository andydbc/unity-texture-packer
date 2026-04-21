using System;

namespace TexPacker
{
    public enum ChannelSourceMode
    {
        FromTexture,
        Constant
    }

    [Serializable]
    public class TextureChannelInput
    {
        public bool enabled;
        public TextureChannel sourceChannel;
        public TextureChannel output;
        public bool invert;
        public ChannelSourceMode sourceMode = ChannelSourceMode.FromTexture;
        public float constantValue;
    }
}
