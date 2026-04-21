using System.Collections.Generic;
using UnityEngine;

namespace TexPacker
{
    public class TextureInput
    {
        public Texture2D texture;

        private Dictionary<TextureChannel, TextureChannelInput> _inputs = new Dictionary<TextureChannel, TextureChannelInput>();

        public TextureInput()
        {
            _inputs[TextureChannel.ChannelRed]   = new TextureChannelInput() { sourceChannel = TextureChannel.ChannelRed,   output = TextureChannel.ChannelRed,   enabled = true };
            _inputs[TextureChannel.ChannelGreen] = new TextureChannelInput() { sourceChannel = TextureChannel.ChannelGreen, output = TextureChannel.ChannelGreen, enabled = true };
            _inputs[TextureChannel.ChannelBlue]  = new TextureChannelInput() { sourceChannel = TextureChannel.ChannelBlue,  output = TextureChannel.ChannelBlue,  enabled = true };
            _inputs[TextureChannel.ChannelAlpha] = new TextureChannelInput() { sourceChannel = TextureChannel.ChannelAlpha, output = TextureChannel.ChannelAlpha, enabled = false };
        }

        public TextureChannelInput GetChannelInput(TextureChannel channel)
        {
            return _inputs[channel];
        }

        public void SetChannelInput(TextureChannel channel, TextureChannelInput channelInput)
        {
            _inputs[channel] = channelInput;
        }
    }
}