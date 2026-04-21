using System;
using UnityEngine;

namespace TexPacker
{
    [Serializable]
    public class TextureInput
    {
        public Texture2D texture;

        [SerializeField]
        private TextureChannelInput[] _inputs;

        public TextureInput()
        {
            _inputs = new TextureChannelInput[4];
            for (int i = 0; i < 4; i++)
            {
                _inputs[i] = new TextureChannelInput
                {
                    sourceChannel = (TextureChannel)i,
                    output        = (TextureChannel)i,
                    enabled       = i != (int)TextureChannel.ChannelAlpha,
                };
            }
        }

        public TextureChannelInput GetChannelInput(TextureChannel channel) => _inputs[(int)channel];
        public void SetChannelInput(TextureChannel channel, TextureChannelInput channelInput) => _inputs[(int)channel] = channelInput;
    }
}
