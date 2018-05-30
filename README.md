unity-texture-packer [![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?style=flat)](http://mit-license.org)
==========


Utility to merge different texture channels into a final texture output. 

![screenshot](Screenshots/screen00.gif)

## Usage
From Code:

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

## Note
This is still under development and may be buggy.


<br>
<a href='https://ko-fi.com/Y8Y0B9TU' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://az743702.vo.msecnd.net/cdn/kofi5.png?v=0' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
