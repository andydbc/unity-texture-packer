using System.Collections.Generic;
using UnityEngine;

namespace TexPacker
{
    public class TexturePacker
    {
        private readonly string _shaderName = "Hidden/TexturePacker";
        private Material _material;

        private List<TextureInput> _texInputs = new List<TextureInput>();
        public List<TextureInput> texInputs => _texInputs;

        public int resolution  = 512;  // used by preview
        public int width       = 2048;
        public int height      = 2048;
        public int bitDepth    = 8;    // 8, 16, or 32

        public void Initialize()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find(_shaderName));
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        public void Add(TextureInput entry) => _texInputs.Add(entry);
        public void Remove(TextureInput input) => _texInputs.Remove(input);

        private string GetPropertyName(int i, string param) => $"_Input0{i}{param}";

        public void ClearProperties()
        {
            for (int i = 0; i < 4; ++i)
            {
                _material.SetTexture(GetPropertyName(i, "Tex"), Texture2D.blackTexture);
                _material.SetVector(GetPropertyName(i, "In"), Vector4.zero);
                _material.SetMatrix(GetPropertyName(i, "Ch"), Matrix4x4.identity);
                _material.SetVector(GetPropertyName(i, "Const"), Vector4.zero);
                _material.SetVector(GetPropertyName(i, "Mode"), Vector4.zero);
            }
        }

        private Vector4 GetInputs(TextureInput texInput)
        {
            Vector4 states = Vector4.zero;
            for (int i = 0; i < 4; ++i)
                states[i] = texInput.GetChannelInput((TextureChannel)i).enabled ? 1f : 0f;
            return states;
        }

        private Vector4 GetInverts(TextureInput texInput)
        {
            Vector4 states = Vector4.zero;
            for (int i = 0; i < 4; ++i)
                states[i] = texInput.GetChannelInput((TextureChannel)i).invert ? 1f : 0f;
            return states;
        }

        private Matrix4x4 GetOutputs(TextureInput texInput)
        {
            Matrix4x4 m = Matrix4x4.zero;
            for (int i = 0; i < 4; ++i)
            {
                Vector4 row = Vector4.zero;
                row[(int)texInput.GetChannelInput((TextureChannel)i).output] = 1f;
                m.SetRow(i, row);
            }
            return m;
        }

        // Each row is a channel selector: (1,0,0,0)=R  (0,1,0,0)=G  (0,0,1,0)=B  (0,0,0,1)=A
        private Matrix4x4 GetInputChannels(TextureInput texInput)
        {
            Matrix4x4 m = Matrix4x4.zero;
            for (int i = 0; i < 4; ++i)
            {
                Vector4 row = Vector4.zero;
                row[(int)texInput.GetChannelInput((TextureChannel)i).sourceChannel] = 1f;
                m.SetRow(i, row);
            }
            return m;
        }

        private Vector4 GetConstants(TextureInput texInput)
        {
            Vector4 v = Vector4.zero;
            for (int i = 0; i < 4; ++i)
                v[i] = texInput.GetChannelInput((TextureChannel)i).constantValue;
            return v;
        }

        private Vector4 GetModes(TextureInput texInput)
        {
            Vector4 v = Vector4.zero;
            for (int i = 0; i < 4; ++i)
                v[i] = texInput.GetChannelInput((TextureChannel)i).sourceMode == ChannelSourceMode.Constant ? 1f : 0f;
            return v;
        }

        public bool HasAlphaOutput()
        {
            foreach (var input in _texInputs)
                for (int i = 0; i < 4; i++)
                {
                    var ch = input.GetChannelInput((TextureChannel)i);
                    if (ch.enabled && ch.output == TextureChannel.ChannelAlpha)
                        return true;
                }
            return false;
        }

        private void ApplyInputs()
        {
            ClearProperties();
            int idx = 0;
            foreach (var input in _texInputs)
            {
                _material.SetTexture(GetPropertyName(idx, "Tex"), input.texture != null ? input.texture : Texture2D.blackTexture);
                _material.SetVector(GetPropertyName(idx, "In"),    GetInputs(input));
                _material.SetVector(GetPropertyName(idx, "Inv"),   GetInverts(input));
                _material.SetMatrix(GetPropertyName(idx, "Out"),   GetOutputs(input));
                _material.SetMatrix(GetPropertyName(idx, "Ch"),    GetInputChannels(input));
                _material.SetVector(GetPropertyName(idx, "Const"), GetConstants(input));
                _material.SetVector(GetPropertyName(idx, "Mode"),  GetModes(input));
                ++idx;
            }
            _material.SetFloat("_HasAlphaInput", HasAlphaOutput() ? 1f : 0f);
        }

        public RenderTexture Create()
        {
            ApplyInputs();
            _material.SetVector("_ChannelMask", Vector4.one);
            return TextureUtility.GenerateTexture(width, height, _material);
        }

        // Renders directly to a RenderTexture — no CPU readback, use for live previews.
        public void RenderTo(RenderTexture target, Vector4 channelMask)
        {
            ApplyInputs();
            _material.SetVector("_ChannelMask", channelMask);
            Graphics.Blit(Texture2D.blackTexture, target, _material);
        }
    }
}
