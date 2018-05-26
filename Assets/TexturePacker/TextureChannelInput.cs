
using System.Collections.Generic;
using UnityEngine;

namespace TexPacker
{
    public class TextureChannelInput
    {
        private int[] _channelStates = new int[4];
        public int[] Channels {
            get { return _channelStates; }
        }

        // public IsEnabled

        public void SetChannelState(TextureChannel channel, bool state)
        {
            _channelStates[(int)channel] = state == true ? 1 : 0;
        }

        public bool GetChannelState(TextureChannel channel)
        {
            return _channelStates[(int)channel] == 1;
        }
    }
}