using System.Collections.Generic;
using UnityEngine;

namespace TexPacker
{
    public class TexturePacker
    {
        private readonly string _shaderName = "Hidden/TexturePacker";
        private Material _material;

        private List<TextureInput> _texInputs = new List<TextureInput>();
        public List<TextureInput> texInputs {
            get { return _texInputs; }
        }

        public int resolution = 2048;

        public void Initialize()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find(_shaderName));
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        public void Add(TextureInput entry)
        {
            _texInputs.Add(entry);
        }

        public void Remove(TextureInput input)
        {
            _texInputs.Remove(input);
        }

        private string GetPropertyName(int i, string param)
        {
            return string.Format("_Input0{0}{1}", i, param);
        }

        public void ClearProperties()
        {
            for (int i = 0; i < 6; ++i)
            {
                _material.SetTexture(GetPropertyName(i, "Tex"), Texture2D.blackTexture);
                _material.SetVector(GetPropertyName(i, "In"), Vector4.zero);
            }
        }

        private Vector4 GetInputs(TextureInput texInput)
        {
            Vector4 states = Vector4.zero;

            for (int i = 0; i < 4; ++i)
            {
                var state = texInput.GetChannelInput((TextureChannel)i).enabled;
                states[i] = state ? 1f : 0f;
            }

            return states;
        }

        private Matrix4x4 GetOutputs(TextureInput texInput)
        {
            Matrix4x4 m = Matrix4x4.zero;

            for (int i = 0; i < 4; ++i)
            {
                Vector4 inChannel = Vector4.zero;
                var output = texInput.GetChannelInput((TextureChannel)i).output;
                inChannel[(int)output] = 1f;
                m.SetRow(i, inChannel);
            }

            return m;
        }

        public Texture2D Create()
        {
            int idx = 0;
            foreach(var input in _texInputs)
            {
                var Tex = input.texture;
                _material.SetTexture(GetPropertyName(idx, "Tex"), Tex);

                var In = GetInputs(input);
                _material.SetVector(GetPropertyName(idx, "In"), In);

                var Out = GetOutputs(input);
                _material.SetMatrix(GetPropertyName(idx, "Out"), Out);
                ++idx;
            }

            var texture = TextureUtility.GenerateTexture(resolution, resolution, _material);

            return texture;
        }
    }
}