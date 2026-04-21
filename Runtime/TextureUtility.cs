using UnityEngine;

namespace TexPacker
{
    public static class TextureUtility
    {
        public static RenderTexture GenerateTexture(int width, int height, Material mat)
        {
            var rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(Texture2D.blackTexture, rt, mat);
            return rt;
        }
    }
}
