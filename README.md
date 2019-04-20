unity-texture-packer [![License](https://img.shields.io/badge/License-MIT-lightgrey.svg?style=flat)](http://mit-license.org) [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/andyduboc/5usd)
==========


:hammer: Utility to merge different texture channels into a final texture output. 

![screenshot](Screenshots/screen00.gif)

### Example in code :floppy_disk:

```cs
TextureInput texInput00 = new TextureInput();
TextureInput texInput01 = new TextureInput();

texInput00.texture = Resources.Load<Texture2D>("input00");
texInput01.texture = Resources.Load<Texture2D>("input01");

var texChanInRed = new TextureChannelInput(TextureChannel.ChannelRed, true);
var texChanInBlue = new TextureChannelInput(TextureChannel.ChannelBlue, true);

// Input Red into Output Red
texInput00.SetChannelInput(TextureChannel.ChannelRed, texChanInRed);
// Input Green into Output Blue
texInput00.SetChannelInput(TextureChannel.ChannelGreen, texChanInBlue);

// Input Green into Output Green
var texChanInGreen =  new TextureChannelInput(TextureChannel.ChannelGreen, true);
texInput01.SetChannelInput(TextureChannel.ChannelGreen, texChanInGreen);

TexturePacker texPacker = new TexturePacker();

texPacker.Add(texInput00);
texPacker.Add(texInput01);

Texture2D texMerged = texPacker.Create();
```

### Note

This is still under development and may be buggy.

### License :pencil:

MIT. See [LICENSE](https://github.com/andydbc/unity-texture-packer/blob/master/LICENSE) for details.
