using System.Collections.Generic;
using UnityEngine;

namespace TexPacker
{
    public class TexturePacker
    {
        private readonly string _shaderName = "Hidden/TexturePacker";
        private Material _material;

        private List<TextureEntry> _entries = new List<TextureEntry>();
        public List<TextureEntry> entries {
            get { return _entries; }
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

        public void Add(TextureEntry entry)
        {
            _entries.Add(entry);
        }

        public void Remove(TextureEntry entry)
        {
            _entries.Remove(entry);
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

        public Texture2D Create()
        {
            var idx = 0;
            foreach(var entry in _entries)
            {
                _material.SetTexture(GetPropertyName(idx, "Tex"), entry.texture);
                _material.SetVector(GetPropertyName(idx, "In"), entry.GetInputShaderFormat());
                _material.SetMatrix(GetPropertyName(idx, "Out"), entry.GetOutputShaderFormat());

                idx++;
            }

            var texture = TextureUtility.GenerateTexture(resolution, resolution, _material);

            return texture;
        }
    }
}