using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TexPacker
{
    public class TexturePackerWindow : EditorWindow
    {
        private readonly string _windowTitle = "Channel Packer";
        private readonly Vector2 _windowSize = new Vector2(300, 450);
        private readonly int _maxInputCount = 4;
        private readonly int _textureSupportedResolutionMin = 64;
        private readonly int _textureSupportedResolutionMax = 8192;

        Vector2 _windowScrollPos;

        private TextureFormat _textureFormat = TextureFormat.PNG;

        private TexturePacker _texturePacker = new TexturePacker();

        private List<int> _textureResolutions = new List<int>();
        private List<string> _textureResolutionsNames = new List<string>();

        private List<TextureItem> _items = new List<TextureItem>();
        private TexturePreview _preview;

        private bool _useCustomTexSize = false;
        private bool _overrideDefaultTexSize = true;

        [MenuItem("Window/Channel Packer")]
        static void Open()
        {
            TexturePackerWindow window = GetWindow<TexturePackerWindow>();
            window.Initialize();
        }

        public void Initialize()
        {
            minSize = _windowSize;
            titleContent = new GUIContent(_windowTitle);

            for (int i = _textureSupportedResolutionMin; i <= _textureSupportedResolutionMax; i *= 2)
            {
                _textureResolutions.Add(i);
                _textureResolutionsNames.Add(i.ToString());
            }

            _texturePacker.Initialize();
            _preview = new TexturePreview();
        }

        private void RefreshItems()
        {
            if (_items.Count == 0)
                return;

            var toDeleteItems = _items.Where(x => x.toDelete == true).ToList();
            foreach (var item in toDeleteItems)
            {
                _texturePacker.Remove(item.input);
                _items.Remove(item);
            }
        }

        private void SetTexSizeFromInput(TextureInput input)
        {
            _texturePacker.texSize = input.texture == null ? Vector2Int.zero : new Vector2Int(input.texture.width, input.texture.height);
        }

        private void OnGUI()
        {
            _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos, false, false);

            RefreshItems();

            GUILayout.Label(_windowTitle, TexturePackerStyles.Heading);
            GUILayout.BeginVertical(TexturePackerStyles.Section);

            GUILayout.Label("Inputs", TexturePackerStyles.Heading);
            foreach (TextureItem item in _items)
            {
                item.Draw();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = _items.Count < _maxInputCount;

            if (GUILayout.Button("+"))
            {
                TextureInput entry = new TextureInput();
                _texturePacker.Add(entry);
                _items.Add(new TextureItem(entry));
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            Vector2Int prevTexSize = _texturePacker.texSize;
            _texturePacker.resolution = 128;

            _preview.Draw(_texturePacker);

            _texturePacker.texSize = prevTexSize;

            GUILayout.Label("Options", TexturePackerStyles.Heading);
            GUILayout.BeginVertical(TexturePackerStyles.Section);
            _textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("> Format:", _textureFormat);

            _useCustomTexSize = EditorGUILayout.Toggle("> Custom texture size:", _useCustomTexSize);

            if (_useCustomTexSize)
            {
                if (_overrideDefaultTexSize && _texturePacker.texInputs.Count == 1)
                {
                    SetTexSizeFromInput(_texturePacker.texInputs[0]);
                }
                _overrideDefaultTexSize = false;

                _texturePacker.texSize.x = Mathf.Abs(EditorGUILayout.IntField("> Texture width:", _texturePacker.texSize.x));
                _texturePacker.texSize.y = Mathf.Abs(EditorGUILayout.IntField("> Texture height:", _texturePacker.texSize.y));

                if (_texturePacker.texInputs.Count > 0)
                {
                    EditorGUILayout.Separator();
                    if (GUILayout.Button("Use size from input"))
                    {
                        var menu = new GenericMenu();
                        for (int i = 0; i < _texturePacker.texInputs.Count; i++)
                        {
                            TextureInput input = _texturePacker.texInputs[i];
                            menu.AddItem(new GUIContent($"Input {i + 1}"), on: false, () => SetTexSizeFromInput(input));
                        }

                        var dropdownPos = new Rect(Event.current.mousePosition, size: Vector2.zero);
                        menu.DropDown(dropdownPos);
                    }
                }
            }
            else
            {
                _texturePacker.texSize = Vector2Int.one * EditorGUILayout.IntPopup("> Resolution:", _texturePacker.resolution, _textureResolutionsNames.ToArray(), _textureResolutions.ToArray());
                _overrideDefaultTexSize = true;
            }

            GUILayout.EndVertical();

            if (GUILayout.Button("Generate Texture", TexturePackerStyles.Button))
            {
                string defaultPath = Application.dataPath;
                if (_texturePacker.texInputs.Count > 0 && _texturePacker.texInputs[0].texture != null)
                {
                    string path = AssetDatabase.GetAssetPath(_texturePacker.texInputs[0].texture);
                    if (path != null && !string.IsNullOrEmpty(path))
                    {
                        path = Path.Combine(Application.dataPath, "..", path);
                        defaultPath = Path.GetDirectoryName(path);
                    }
                }
                string savePath = EditorUtility.SaveFilePanel("Save", defaultPath, "texture.png", _textureFormat.ToString());
                if (savePath != string.Empty)
                {
                    Texture2D output = _texturePacker.Create();

                    if (_textureFormat == TextureFormat.JPG)
                        File.WriteAllBytes(savePath, output.EncodeToJPG());
                    else if (_textureFormat == TextureFormat.PNG)
                        File.WriteAllBytes(savePath, output.EncodeToPNG());
                    else
                        File.WriteAllBytes(savePath, output.EncodeToEXR());

                    AssetDatabase.Refresh();
                }
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
