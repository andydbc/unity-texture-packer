using UnityEditor;
using UnityEngine;

namespace TexPacker
{
    class TextureItem
    {
        public TextureEntry entry;
        public bool toDelete { get; private set; }

        private bool _fold = true;

        public TextureItem(TextureEntry entry)
        {
            this.entry = entry;
        }

        private Rect GetFoldRect()
        {
            var r = EditorGUILayout.GetControlRect();
            var rFold = r;
            rFold.width = 20;
            return rFold;
        }

        public void Draw()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

            EditorGUILayout.BeginHorizontal();

            _fold = EditorGUI.Foldout(GetFoldRect(), _fold, "Input");

            var gearStyle = new GUIStyle("Icon.Options");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(gearStyle.normal.background, new GUIStyle("IconButton")))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove"), false, data =>
                {
                    var item = data as TextureItem;
                    item.toDelete = true;

                }, this);

                menu.ShowAsContext();
            }

            using (new EditorGUI.IndentLevelScope(1))
            {
                EditorGUILayout.EndHorizontal();
                if (_fold)
                {

                    GUILayout.BeginHorizontal(TexturePackerStyles.Heading);

                    GUILayout.BeginVertical(TexturePackerStyles.Heading);

                    GUILayout.Label("Channels Selection:");

                    string[] channels = new string[] { "Red", "Green", "Blue", "Alpha" };

                    for (int i = 0; i < 4; ++i)
                    {
                        GUILayout.BeginHorizontal();

                        var activeTexChannel = (TextureChannel)i;

                        bool enabled = entry.inputs.GetChannelState(activeTexChannel);
                        enabled = GUILayout.Toggle(enabled, new GUIContent(" " + channels[i]), GUILayout.Width(60));
                        entry.inputs.SetChannelState(activeTexChannel, enabled);

                        GUILayout.Label(">");

                        TextureChannel output = entry.outputs[(int)activeTexChannel];
                        output = (TextureChannel)EditorGUILayout.Popup((int)output, channels, GUILayout.Width(80));
                        entry.outputs[(int)activeTexChannel] = output;
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();

                    entry.texture = EditorGUILayout.ObjectField(entry.texture, typeof(Texture2D), false, GUILayout.Width(90), GUILayout.Height(80)) as Texture2D;

                    GUILayout.EndHorizontal();

                }
            }
        }
    }
}