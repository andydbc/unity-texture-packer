using UnityEngine;
using UnityEditor;

namespace TexPacker
{
    public class TexturePreview
    {
        private int toolbarItem = 0;

        public void Draw(TexturePacker texPacker)
        {
            GUILayout.Label("Preview", TexturePackerStyles.Heading);

            GUILayout.BeginVertical(TexturePackerStyles.Section);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Vector2 previewSize = new Vector2(256, 256);
            GUILayout.Label("", TexturePackerStyles.MidBox, GUILayout.Width(previewSize.x), GUILayout.Height(previewSize.y));
            Rect previewRect = GUILayoutUtility.GetLastRect();
            Rect alphaRect = new Rect(previewRect.x + 5, previewRect.y + 5, previewRect.width - 10, previewRect.height - 10);

            texPacker.ClearProperties();

            Texture2D preview = texPacker.Create();
            preview.alphaIsTransparency = true;

            if(toolbarItem == 0)
                EditorGUI.DrawPreviewTexture(alphaRect, preview);
            else
                EditorGUI.DrawTextureAlpha(alphaRect, preview);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            string[] toolbarItems = { "RBG", "Alpha" };
            toolbarItem = GUILayout.Toolbar(toolbarItem, toolbarItems);

            GUILayout.EndVertical();
        }
    }
}