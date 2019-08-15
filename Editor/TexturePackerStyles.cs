using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TexPacker
{
    public class TexturePackerStyles
    {

        static GUIStyle heading = null;
        public static GUIStyle Heading
        {
            get
            {
                if (heading == null)
                {
                    try
                    {
                        heading = new GUIStyle("OL Title");
                        heading.padding = new RectOffset(10, 10, 10, 10);
                        heading.margin = new RectOffset(4, 4, 4, 4);
                        heading.fixedHeight = 0;
                        heading.fontSize = 15;
                        heading.alignment = TextAnchor.MiddleCenter;
                    }
                    catch { heading = GetErrorGUIStyle(); }
                }
                return heading;
            }
        }

        static GUIStyle midBox;
        public static GUIStyle MidBox
        {
            get
            {
                if (midBox == null)
                {
                    try
                    {
                        midBox = new GUIStyle(GUI.skin.GetStyle("Box"));
                        midBox.normal.background = GetBorderedTexture(new Color(0.26f, 0.26f, 0.26f), new Color(0.15f, 0.15f, 0.15f, 1));
                        midBox.border = new RectOffset(1, 1, 1, 1);
                        midBox.padding = new RectOffset(5, 5, 5, 5);
                        midBox.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
                        midBox.alignment = TextAnchor.MiddleCenter;
                        midBox.fontSize = 10;

                    }
                    catch
                    {
                        midBox = GetErrorGUIStyle();
                    }
                }
                return midBox;
            }
        }

        static GUIStyle section = null;
        public static GUIStyle Section
        {
            get
            {
                if (section == null)
                {
                    try
                    {
                        section = new GUIStyle(GUI.skin.GetStyle("sv_iconselector_back"));
                        section.padding = new RectOffset(4, 4, 4, 4);
                        section.margin = new RectOffset(4, 4, 4, 4);
                        section.stretchHeight = false;
                        section.stretchWidth = true;

                    }
                    catch { section = GetErrorGUIStyle(); }
                }
                return section;
            }
        }

        private static GUIStyle m_Button = null;
        public static GUIStyle Button
        {
            get
            {
                if (m_Button == null)
                {
                    try
                    {
                        m_Button = new GUIStyle(GUI.skin.button);

                        m_Button.padding = new RectOffset(1, 1, 15, 15);
                        m_Button.margin = new RectOffset(5, 5, 5, 0);
                        m_Button.contentOffset = Vector2.zero;

                        m_Button.fontSize = 11;

                        m_Button.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.35f, 0.35f, 0.35f);
                        m_Button.hover.textColor = EditorGUIUtility.isProSkin ? Color.white : new Color(0.2f, 0.2f, 0.2f);
                        m_Button.active.textColor = EditorGUIUtility.isProSkin ? Color.gray : new Color(0.2f, 0.2f, 0.2f);
                        m_Button.focused.textColor = Color.gray;

                        m_Button.alignment = TextAnchor.MiddleCenter;

                        m_Button.border = new RectOffset(3, 3, 3, 3);
                    }
                    catch { m_Button = GetErrorGUIStyle(); }

                }
                return m_Button;
            }
        }

        static Texture2D GetBorderedTexture(Color border, Color centre)
        {
            Texture2D returnTex = new Texture2D(3, 3);
            returnTex.filterMode = FilterMode.Point;
            for (int x = 0; x < returnTex.width; x++)
            {
                for (int y = 0; y < returnTex.height; y++)
                {
                    returnTex.SetPixel(x, y, border);
                    if (x == 1 && y == 1)
                    {
                        returnTex.SetPixel(x, y, centre);
                    }
                }
            }
            returnTex.Apply();
            returnTex.hideFlags = HideFlags.HideAndDontSave;
            return returnTex;
        }

        private static GUIStyle GetErrorGUIStyle()
        {
            GUIStyle s = new GUIStyle();
            s.normal.textColor = Color.magenta;
            return s;
        }
    }
}
