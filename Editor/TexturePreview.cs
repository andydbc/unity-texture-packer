using UnityEngine;
using UnityEngine.UIElements;

namespace TexPacker
{
    public class TexturePreview
    {
        public VisualElement Root { get; }

        private Vector4 _channelMask = Vector4.one;
        private Button _rgbaBtn;
        private readonly Button[] _channelButtons = new Button[4];
        private int _renderResolution = 512;
        private Image _previewImage;
        private RenderTexture _previewRT;
        private Texture2D _checkerTex;
        private readonly TexturePacker _texturePacker;

        public TexturePreview(TexturePacker texturePacker)
        {
            _texturePacker = texturePacker;
            Root = Build();
        }

        private VisualElement Build()
        {
            var section = new VisualElement();
            section.AddToClassList("section");

            var header = new Label("Preview");
            header.AddToClassList("section-header");
            section.Add(header);

            var imageWrapper = new VisualElement();
            imageWrapper.AddToClassList("preview-wrapper");

            _previewImage = new Image { scaleMode = ScaleMode.ScaleToFit };
            _previewImage.AddToClassList("preview-image");

            _checkerTex = CreateCheckerboard();
            _previewImage.style.backgroundImage = new StyleBackground(_checkerTex);
            _previewImage.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            _previewImage.style.backgroundSize   = new BackgroundSize(
                new Length(16, LengthUnit.Pixel), new Length(16, LengthUnit.Pixel));

            imageWrapper.Add(_previewImage);
            section.Add(imageWrapper);

            imageWrapper.RegisterCallback<GeometryChangedEvent>(e =>
            {
                float w = e.newRect.width;
                if (w < 1f) return;
                _previewImage.style.height = w * Aspect();
            });

            var toolbar = new VisualElement();
            toolbar.AddToClassList("preview-toolbar");

            _rgbaBtn = new Button(() =>
            {
                _channelMask = Vector4.one;
                RefreshToolbar();
                Update();
            }) { text = "RGBA" };
            _rgbaBtn.AddToClassList("channel-btn");
            toolbar.Add(_rgbaBtn);

            string[] labels = { "R", "G", "B", "A" };
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                _channelButtons[i] = new Button(() =>
                {
                    _channelMask = Vector4.zero;
                    _channelMask[idx] = 1f;
                    RefreshToolbar();
                    Update();
                }) { text = labels[i] };
                _channelButtons[i].AddToClassList("channel-btn");
                toolbar.Add(_channelButtons[i]);
            }

            RefreshToolbar();
            section.Add(toolbar);

            section.RegisterCallback<DetachFromPanelEvent>(_ => Release());
            
            return section;
        }

        private void RefreshToolbar()
        {
            bool allOn = _channelMask.x > 0.5f && _channelMask.y > 0.5f
                      && _channelMask.z > 0.5f && _channelMask.w > 0.5f;
            _rgbaBtn.EnableInClassList("channel-btn--on", allOn);
            for (int i = 0; i < 4; i++)
                _channelButtons[i].EnableInClassList("channel-btn--on", !allOn && _channelMask[i] > 0.5f);
        }

        private float Aspect()
        {
            int w = _texturePacker.width;
            int h = _texturePacker.height;
            return (w > 0 && h > 0) ? (float)h / w : 1f;
        }

        public void Update()
        {
            float aspect = Aspect();
            int rtW = _renderResolution;
            int rtH = Mathf.Max(1, Mathf.RoundToInt(_renderResolution * aspect));

            if (_previewRT == null || _previewRT.width != rtW || _previewRT.height != rtH)
            {
                ReleaseRT();
                _previewRT = new RenderTexture(rtW, rtH, 0, RenderTextureFormat.ARGB32);
                _previewRT.Create();
            }

            float displayW = _previewImage.resolvedStyle.width;
            if (displayW > 1f)
                _previewImage.style.height = displayW * aspect;

            _texturePacker.RenderTo(_previewRT, _channelMask);
            _previewImage.image = _previewRT;
        }

        private static Texture2D CreateCheckerboard()
        {
            const int cell = 8;
            var tex = new Texture2D(cell * 2, cell * 2,
                UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.wrapMode  = TextureWrapMode.Repeat;
            var light = new Color(0.75f, 0.75f, 0.75f, 1f);
            var dark  = new Color(0.50f, 0.50f, 0.50f, 1f);
            for (int y = 0; y < cell * 2; y++)
                for (int x = 0; x < cell * 2; x++)
                    tex.SetPixel(x, y, ((x / cell + y / cell) % 2 == 0) ? light : dark);
            tex.Apply();
            return tex;
        }

        private void ReleaseRT()
        {
            if (_previewRT == null) return;
            _previewImage.image = null;
            _previewRT.Release();
            Object.DestroyImmediate(_previewRT);
            _previewRT = null;
        }

        private void Release()
        {
            ReleaseRT();
            if (_checkerTex != null) { Object.DestroyImmediate(_checkerTex); _checkerTex = null; }
        }
    }
}
