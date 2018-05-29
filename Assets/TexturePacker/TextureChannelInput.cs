
using System.Collections.Generic;
using UnityEngine;

namespace TexPacker
{
    public class TextureChannelInput
    {
        public bool enabled;
        public TextureChannel output;

        public TextureChannelInput() { }
        public TextureChannelInput(TextureChannel output, bool enabled = false)
        {
            this.output = output;
            this.enabled = enabled;
        }
    }
}