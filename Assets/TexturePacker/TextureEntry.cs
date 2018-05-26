using UnityEngine;
using UnityEditor;

namespace TexPacker
{
    public class TextureEntry
    {
        public Texture2D texture;

        private TextureChannelInput _inputs = new TextureChannelInput();
        public TextureChannelInput inputs {
            get { return _inputs; }
        }

        private TextureChannel[] _outputs = new TextureChannel[4];
        public TextureChannel[] outputs {
            get { return _outputs; }
        }

        public Vector4 GetInputShaderFormat()
        {
            Vector4 input = Vector4.zero;

            input.x = _inputs.GetChannelState(TextureChannel.ChannelRed) ? 1f : 0f;
            input.y = _inputs.GetChannelState(TextureChannel.ChannelGreen) ? 1f : 0f;
            input.z = _inputs.GetChannelState(TextureChannel.ChannelBlue) ? 1f : 0f;
            input.w = _inputs.GetChannelState(TextureChannel.ChannelAlpha) ? 1f : 0f;

            return input;
        }

        public Matrix4x4 GetOutputShaderFormat()
        {
            Matrix4x4 m = Matrix4x4.zero;

            for (int i = 0; i < 4; ++i)
            {
                Vector4 inputChannel = Vector4.zero;
                inputChannel[(int)_outputs[i]] = 1f;
                m.SetRow(i, inputChannel);
            }

            return m;
        }
    }
}