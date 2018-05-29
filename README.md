unity-texture-packer [![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?style=flat)](http://mit-license.org)
==========


Utility to merge different texture channels into a final texture output. 

![screenshot](Screenshots/screen00.gif)

## Usage
From Code:

```cs
TextureInput texInput00 = new TextureInput();
texInput00.texture = Resources.Load<Texture2D>("input00");
// Input Red into Output Red
var texChanInRed = new TextureChannelInput(TextureChannel.ChannelRed, true);
texInput00.SetChannelInput(TextureChannel.ChannelRed, texChanInRed);
// Input Green into Output Blue
var texChanInBlue = new TextureChannelInput(TextureChannel.ChannelBlue, true);
texInput00.SetChannelInput(TextureChannel.ChannelGreen, texChanInBlue);

TextureInput texInput01 = new TextureInput();
texInput01.texture = Resources.Load<Texture2D>("input01");
// Input Green into Output Green
var texChanInGreen =  new TextureChannelInput(TextureChannel.ChannelGreen, true);
texInput01.SetChannelInput(TextureChannel.ChannelGreen, texChanInGreen);

TexturePacker texPacker = new TexturePacker();

texPacker.Add(texInput00);
texPacker.Add(texInput01);

Texture2D texMerged = texPacker.Create();
```

## Note
This is still under development and may be buggy.
