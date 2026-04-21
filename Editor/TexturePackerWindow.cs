using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TexPacker
{
    public class TexturePackerWindow : EditorWindow
    {
        private const int MaxInputCount = 4;
        private const int ResolutionMin = 64;
        private const int ResolutionMax = 8192;

        private TextureFormat _textureFormat = TextureFormat.PNG;
        private TexturePacker _texturePacker;
        private TexturePreview _texturePreview;
        private List<TextureItem> _items;
        private VisualElement _inputsContainer;
        private Button _addButton;
        private Label _resValue;
        private Label _resWarning;

        [MenuItem("Window/Texture Packer")]
        static void Open() => GetWindow<TexturePackerWindow>().titleContent = new GUIContent("Texture Packer");

        public void CreateGUI()
        {
            _texturePacker = new TexturePacker();
            _texturePacker.Initialize();
            _items = new List<TextureItem>();

            minSize = new Vector2(360, 500);

            var root = rootVisualElement;
            var styleSheet = Resources.Load<StyleSheet>("TexturePacker");
            if (styleSheet != null) root.styleSheets.Add(styleSheet);

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            root.Add(scroll);

            // Inputs
            scroll.Add(MakeSection("Inputs", out var inputsBody));
            _inputsContainer = new VisualElement();
            inputsBody.Add(_inputsContainer);
            _addButton = new Button(AddInput) { text = "+" };
            _addButton.AddToClassList("center-btn");
            inputsBody.Add(_addButton);

            // Preview
            _texturePreview = new TexturePreview(_texturePacker);
            scroll.Add(_texturePreview.Root);
            
            // Options
            scroll.Add(MakeSection("Options", out var optionsBody));

            var formatField = new EnumField("Format", _textureFormat);
            formatField.AddToClassList("options-field");
            formatField.RegisterValueChangedCallback(e => _textureFormat = (TextureFormat)e.newValue);
            optionsBody.Add(formatField);

            // var bitDepths = new List<int> { 8, 16, 32 };
            // var bitDepthField = new PopupField<int>("Bit Depth", bitDepths, _texturePacker.bitDepth,
            //     v => $"{v}-bit", v => $"{v}-bit");
            // bitDepthField.AddToClassList("options-field");
            // bitDepthField.RegisterValueChangedCallback(e => _texturePacker.bitDepth = e.newValue);
            // optionsBody.Add(bitDepthField);

            // Resolution — read-only info row, auto-set from first input
            var resRow = new VisualElement();
            resRow.AddToClassList("info-row");
            var resKey = new Label("Resolution");
            resKey.AddToClassList("info-row__label");
            resRow.Add(resKey);
            _resValue = new Label("—");
            _resValue.AddToClassList("info-row__value");
            resRow.Add(_resValue);
            optionsBody.Add(resRow);

            _resWarning = new Label("⚠  Inputs have different resolutions — content will be resampled.");
            _resWarning.AddToClassList("res-warning");
            _resWarning.style.display = DisplayStyle.None;
            optionsBody.Add(_resWarning);

            var generateBtn = new Button(GenerateTexture) { text = "Export" };
            generateBtn.AddToClassList("generate-btn");
            scroll.Add(generateBtn);
        }

        private static VisualElement MakeSection(string title, out VisualElement body)
        {
            var section = new VisualElement();
            section.AddToClassList("section");
            var header = new Label(title);
            header.AddToClassList("section-header");
            section.Add(header);
            body = new VisualElement();
            body.AddToClassList("section-body");
            section.Add(body);
            return section;
        }

        private void AddInput()
        {
            if (_items.Count >= MaxInputCount) return;
            var input = new TextureInput();
            _texturePacker.Add(input);
            var item = new TextureItem(input, RemoveItem, OnTextureInputChanged);
            _items.Add(item);
            _inputsContainer.Add(item.Root);
            _addButton.SetEnabled(_items.Count < MaxInputCount);
        }

        private void RemoveItem(TextureItem item)
        {
            _texturePacker.Remove(item.Input);
            _inputsContainer.Remove(item.Root);
            _items.Remove(item);
            _addButton.SetEnabled(_items.Count < MaxInputCount);
            ApplyAutoResolution();
            CheckResolutionMismatch();

            _texturePreview.Update();

        }

        private void OnTextureInputChanged(TextureItem _)
        {
          ApplyAutoResolution();
          CheckResolutionMismatch();
          _texturePreview.Update();
        }

        private void ApplyAutoResolution()
        {
            if(_items.Count == 0) {
                _texturePacker.width = _texturePacker.height = 512;
                _resValue.text = "—";
                return;
            }

            foreach (var item in _items)
            {
                if (item.Input.texture == null) continue;
                int w = Mathf.Clamp(Mathf.ClosestPowerOfTwo(item.Input.texture.width),  ResolutionMin, ResolutionMax);
                int h = Mathf.Clamp(Mathf.ClosestPowerOfTwo(item.Input.texture.height), ResolutionMin, ResolutionMax);
                _texturePacker.width  = w;
                _texturePacker.height = h;
                _resValue.text = $"{w} × {h}";
                return;
            }
            _resValue.text = "—";
        }

        private void CheckResolutionMismatch()
        {
            int refW = -1, refH = -1;
            bool mismatch = false;
            foreach (var item in _items)
            {
                if (item.Input.texture == null) continue;
                if (refW < 0) { refW = item.Input.texture.width; refH = item.Input.texture.height; }
                else if (item.Input.texture.width != refW || item.Input.texture.height != refH)
                { mismatch = true; break; }
            }
            _resWarning.style.display = mismatch ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void GenerateTexture()
        {
            string defaultPath = Application.dataPath;
            if (_texturePacker.texInputs.Count > 0 && _texturePacker.texInputs[0].texture != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(_texturePacker.texInputs[0].texture);
                if (!string.IsNullOrEmpty(assetPath))
                    defaultPath = Path.GetDirectoryName(Path.Combine(Application.dataPath, "..", assetPath));
            }

            string ext = _textureFormat.ToString().ToLower();
            string savePath = EditorUtility.SaveFilePanel("Save Texture", defaultPath, "texture." + ext, ext);
            if (string.IsNullOrEmpty(savePath)) return;

            RenderTexture rt = _texturePacker.Create();

            var output = new Texture2D(rt.width, rt.height, UnityEngine.TextureFormat.RGBA32, false);

            RenderTexture.active = rt;
            output.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            output.Apply();
            RenderTexture.active = null;

            rt.Release();     Object.DestroyImmediate(rt);

            byte[] bytes = _textureFormat switch
            {
                TextureFormat.JPG => output.EncodeToJPG(),
                TextureFormat.EXR => output.EncodeToEXR(),
                TextureFormat.TGA => output.EncodeToTGA(),
                _ => output.EncodeToPNG()
            };
            Object.DestroyImmediate(output);
            File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();

            if (_texturePacker.HasAlphaOutput())
            {
                string dataPath = Application.dataPath.Replace('\\', '/');
                string norm = savePath.Replace('\\', '/');
                if (norm.StartsWith(dataPath))
                {
                    string rel = "Assets" + norm.Substring(dataPath.Length);
                    if (AssetImporter.GetAtPath(rel) is TextureImporter imp)
                    {
                        imp.alphaSource = TextureImporterAlphaSource.FromInput;
                        imp.alphaIsTransparency = true;
                        imp.SaveAndReimport();
                    }
                }
            }
        }
    }
}
