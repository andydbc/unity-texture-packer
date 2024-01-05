using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TexPacker
{
    public class TextureInput
    {
        public Texture2D texture;

        private Dictionary<TextureChannel, TextureChannelInput> _inputs = new Dictionary<TextureChannel, TextureChannelInput>();

        public TextureInput()
        {
            _inputs[TextureChannel.ChannelRed]      = new TextureChannelInput() { output = TextureChannel.ChannelRed };
            _inputs[TextureChannel.ChannelGreen]    = new TextureChannelInput() { output = TextureChannel.ChannelGreen };
            _inputs[TextureChannel.ChannelBlue]     = new TextureChannelInput() { output = TextureChannel.ChannelBlue };
            _inputs[TextureChannel.ChannelAlpha]    = new TextureChannelInput() { output = TextureChannel.ChannelAlpha };
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