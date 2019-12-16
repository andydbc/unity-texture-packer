using UnityEditor;
using UnityEngine;

namespace TexPacker
{
    class TextureItem
    {
        public TextureInput input;
        public bool toDelete { get; private set; }

        private bool _fold = true;

        public TextureItem(TextureInput input)
        {
            this.input = input;
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

            var gearStyle = new GUIStyle();
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

                        var texChannel = (TextureChannel)i;

                        var channelInput = input.GetChannelInput(texChannel);

                        channelInput.enabled = GUILayout.Toggle(channelInput.enabled, new GUIContent(" " + channels[i]), GUILayout.Width(60));

                        GUILayout.Label(">");

                        channelInput.output = (TextureChannel)EditorGUILayout.Popup((int)channelInput.output, channels, GUILayout.Width(80));

                        input.SetChannelInput(texChannel, channelInput);

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();

                    input.texture = EditorGUILayout.ObjectField(input.texture, typeof(Texture2D), false, GUILayout.Width(90), GUILayout.Height(80)) as Texture2D;

                    GUILayout.EndHorizontal();

                }
            }
        }
    }
}