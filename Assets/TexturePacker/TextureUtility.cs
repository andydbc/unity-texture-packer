using UnityEngine;

namespace TexPacker
{
    public static class TextureUtility
    {
        public static Texture2D GenerateTexture(int width, int height, Material mat)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(Texture2D.blackTexture, tempRT, mat);

            Texture2D output = new Texture2D(tempRT.width, tempRT.height, UnityEngine.TextureFormat.RGBA32, false);
            RenderTexture.active = tempRT;

            output.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            output.Apply();
            output.filterMode = FilterMode.Bilinear;

            RenderTexture.ReleaseTemporary(tempRT);
            RenderTexture.active = null;

            return output;
        }
    }
}
