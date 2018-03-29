using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TexturePackerWindow : EditorWindow
{
    string _viewTitle = "Texture Channel Packer";
    Vector2 _viewScrollPos;
    
    List<string> _errorList = new List<string>();

    List<TexturePackerInput> _textureInputs = new List<TexturePackerInput>();
    public List<TexturePackerInput> Inputs() { return _textureInputs; }

    Material _material = null;

    static int TEXTURE_MIN_RESOLUTION = 64;
    static int TEXTURE_MAX_RESOLUTION = 8192;
    List<int> _textureResolutions = new List<int>();
    List<string> _textureResolutionsNames = new List<string>();

    int _resolution = 2048;
    public int Resolution() { return _resolution; }

    enum TextureFormat
    {
        PNG,
        EXR
    }

    TextureFormat _textureFormat = TextureFormat.PNG;

    [MenuItem("Window/Texture Channel Packer")]
    static void ShowWindow()
    {
        TexturePackerWindow window = GetWindow<TexturePackerWindow>();

        Vector2 minSize = new Vector2(300, 450);
        window.minSize = minSize;

#if UNITY_5_3_OR_NEWER
        window.titleContent = new GUIContent(window._viewTitle);
#else
		window.title = window._viewTitle;
#endif
    }

    void Initialize()
    {
        _textureInputs.Clear();
        _textureResolutions.Clear();
        _textureResolutionsNames.Clear();

        for (int i = TEXTURE_MIN_RESOLUTION; i <= TEXTURE_MAX_RESOLUTION; i *= 2)
        {
            _textureResolutions.Add(i);
            _textureResolutionsNames.Add(i.ToString());
        }

        if (_material == null)
        {
            var shader = Shader.Find("Hidden/TexturePacker");
            _material = new Material(shader);
        }
    }

    void OnEnable()
    {
        Initialize();
    }

    void OnGUI()
    {
        _viewScrollPos = EditorGUILayout.BeginScrollView(_viewScrollPos, false, false);

        GUILayout.Label(_viewTitle, TexturePackerStyles.Heading);

        GUILayout.BeginVertical(TexturePackerStyles.Section);

        DrawInputSelection();

        DrawPreview();

        DrawOptions();

        OnValidityCheckGUI();

        using (new EditorGUI.DisabledScope(!ValidityCheck()))
        {
            if (GUILayout.Button("Generate Texture", TexturePackerStyles.Button))
            {
                string savePath = string.Empty;
                switch (_textureFormat)
                {
                    case TextureFormat.PNG:
                        savePath = EditorUtility.SaveFilePanel("Save", Application.dataPath, "texture.png", "PNG");
                        if(savePath.Length!=0)
                        {
                            Texture2D result = TexturePackerUtils.GenerateTexture(_resolution, _resolution, _material);
                            File.WriteAllBytes(savePath, result.EncodeToPNG());
                            AssetDatabase.Refresh();
                        }
                        break;
                    case TextureFormat.EXR:
                        savePath = EditorUtility.SaveFilePanel("Save", Application.dataPath, "texture.exr", "EXR");
                        if (savePath.Length != 0)
                        {
                            Texture2D result = TexturePackerUtils.GenerateTexture(_resolution, _resolution, _material);
                            File.WriteAllBytes(savePath, result.EncodeToEXR());
                            AssetDatabase.Refresh();
                        }
                        break;
                }
            }
        }

        GUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    public void OnValidityCheckGUI()
    {
        if (!ValidityCheck())
        {
            for (int i = 0; i < _errorList.Count; i++)
            {
                EditorGUILayout.HelpBox(_errorList[i], MessageType.Warning);
            }
        }
    }
    
    public bool ValidityCheck()
    {
        _errorList.Clear();

        var ok = true;

        if (_textureInputs.Count == 0)
        {
            ok = false;
            _errorList.Add("No input, please add a texture input.");
        }

        return ok;
    }

    void DrawInputSelection()
    {
        GUILayout.Label("Inputs", TexturePackerStyles.Heading);
        foreach (TexturePackerInput input in _textureInputs)
            input.Draw(this);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+"))
        {
            var newInput = new TexturePackerInput();
            _textureInputs.Add(newInput);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    void DrawPreview()
    {
        GUILayout.Label("Preview", TexturePackerStyles.Heading);

        GUILayout.BeginVertical(TexturePackerStyles.Section);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Vector2 previewSize = new Vector2(256, 256);
        GUILayout.Label("", TexturePackerStyles.MidBox, GUILayout.Width(previewSize.x), GUILayout.Height(previewSize.y));
        Rect previewRect = GUILayoutUtility.GetLastRect();
        Rect alphaRect = new Rect(previewRect.x + 5, previewRect.y + 5, previewRect.width - 10, previewRect.height - 10);

        for (int i = 0; i < 4; ++i)
            SendInputProperties(i);

        Texture2D preview = TexturePackerUtils.GenerateTexture((int)previewSize.x, (int)previewSize.y, _material);
        EditorGUI.DrawPreviewTexture(alphaRect, preview);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void DrawOptions()
    {
        GUILayout.Label("Options", TexturePackerStyles.Heading);
        GUILayout.BeginVertical(TexturePackerStyles.Section);
        _textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("> Format:", _textureFormat);
        _resolution = EditorGUILayout.IntPopup("> Resolution:", _resolution, _textureResolutionsNames.ToArray(), _textureResolutions.ToArray());
        GUILayout.EndVertical();
    }

    public void RemoveInput(TexturePackerInput input)
    {
        for (int i = 0; i < _textureInputs.Count; ++i)
            ClearInputProperties(i);

        _textureInputs.Remove(input);
    }

    public string GetInputPropertyName(int i, string param)
    {
        return string.Format("_Input0{0}{1}", i, param);
    }

    public void ClearInputProperties(int i)
    {
        _material.SetTexture(GetInputPropertyName(i, "Tex"), Texture2D.blackTexture);
        _material.SetVector(GetInputPropertyName(i, "In"), Vector4.zero);
    }

    public void SendInputProperties(int i)
    {
        if (_textureInputs.Count <= i)
            return;

        var input = _textureInputs[i];
        
        _material.SetTexture(GetInputPropertyName(i, "Tex"), input.Texture());
        _material.SetVector(GetInputPropertyName(i, "In"), input.In());
        _material.SetMatrix(GetInputPropertyName(i, "Out"), input.Out());
    }

}
