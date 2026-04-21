using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TexPacker
{
    public class TextureItem
    {
        public TextureInput Input { get; }
        public VisualElement Root { get; }

        private static readonly List<string> ChannelChoices = new List<string> { "R", "G", "B", "A" };

        public TextureItem(TextureInput input, Action<TextureItem> onRemove, Action<TextureItem> onChanged = null)
        {
            Input = input;
            Root = Build(onRemove, onChanged);
        }

        private VisualElement Build(Action<TextureItem> onRemove, Action<TextureItem> onChanged)
        {
            var root = new VisualElement();
            root.AddToClassList("texture-item");

            var foldout = new Foldout { text = "Input", value = true };

            var removeBtn = new Button(() => onRemove(this)) { text = "×" };
            removeBtn.AddToClassList("remove-btn");
            foldout.Q<Toggle>()?.Add(removeBtn);

            // Texture source — full width
            var textureField = new ObjectField
            {
                objectType = typeof(Texture2D),
                allowSceneObjects = false,
                value = Input.texture
            };
            textureField.AddToClassList("item-texture-field");
            textureField.RegisterValueChangedCallback(e =>
            {
                Input.texture = e.newValue as Texture2D;
                onChanged?.Invoke(this);
            });
            foldout.Add(textureField);

            // Header row — each element uses the SAME CSS class as the content column
            // below it so widths are guaranteed to align.
            var headerRow = new VisualElement();
            headerRow.AddToClassList("channel-row");
            headerRow.Add(Spacer("channel-enable"));
            headerRow.Add(Spacer("channel-mode"));
            headerRow.Add(ColHeader("From", "channel-source"));
            headerRow.Add(Spacer("channel-arrow"));
            headerRow.Add(ColHeader("To",   "channel-output"));
            headerRow.Add(ColHeader("Invert",  "channel-invert"));
            foldout.Add(headerRow);

            // One row per slot (R / G / B / A)
            for (int i = 0; i < 4; i++)
            {
                var channel = (TextureChannel)i;
                var channelInput = Input.GetChannelInput(channel);

                var row = new VisualElement();
                row.AddToClassList("channel-row");
                if (!channelInput.enabled) row.AddToClassList("ch-disabled");

                // Enable checkbox — clicking it enables/disables this slot
                var enableToggle = new Toggle { value = channelInput.enabled };
                enableToggle.AddToClassList("channel-enable");
                row.Add(enableToggle);

                // Mode toggle — switches source between texture sample and constant value
                var modeBtn = new Button { text = ModeLabel(channelInput.sourceMode) };
                modeBtn.AddToClassList("channel-mode");
                modeBtn.SetEnabled(channelInput.enabled);
                row.Add(modeBtn);

                // Source column — holds either the source-channel popup or a constant slider
                var sourceContainer = new VisualElement();
                sourceContainer.AddToClassList("channel-source");
                sourceContainer.SetEnabled(channelInput.enabled);

                var sourcePopup = new PopupField<string>(ChannelChoices, (int)channelInput.sourceChannel);
                sourcePopup.RegisterValueChangedCallback(e =>
                {
                    channelInput.sourceChannel = (TextureChannel)ChannelChoices.IndexOf(e.newValue);
                    onChanged?.Invoke(this);
                });

                var constSlider = new Slider(0f, 1f) { value = channelInput.constantValue, showInputField = true };
                constSlider.RegisterValueChangedCallback(e =>
                {
                    channelInput.constantValue = e.newValue;
                    onChanged?.Invoke(this);
                });

                void RefreshSource()
                {
                    sourceContainer.Clear();
                    sourceContainer.Add(channelInput.sourceMode == ChannelSourceMode.Constant
                        ? (VisualElement)constSlider
                        : sourcePopup);
                    modeBtn.text = ModeLabel(channelInput.sourceMode);
                }
                RefreshSource();
                row.Add(sourceContainer);

                modeBtn.clicked += () =>
                {
                    channelInput.sourceMode = channelInput.sourceMode == ChannelSourceMode.Constant
                        ? ChannelSourceMode.FromTexture
                        : ChannelSourceMode.Constant;
                    RefreshSource();
                    onChanged?.Invoke(this);
                };

                var arrow = new Label("▶");
                arrow.AddToClassList("channel-arrow");
                row.Add(arrow);

                // Output channel selector
                var outputField = new PopupField<string>(ChannelChoices, (int)channelInput.output);
                outputField.AddToClassList("channel-output");
                outputField.SetEnabled(channelInput.enabled);
                outputField.RegisterValueChangedCallback(e =>
                {
                    channelInput.output = (TextureChannel)ChannelChoices.IndexOf(e.newValue);
                    onChanged?.Invoke(this);
                });
                row.Add(outputField);

                // Invert toggle
                var invertToggle = new Toggle { value = channelInput.invert };
                invertToggle.AddToClassList("channel-invert");
                invertToggle.SetEnabled(channelInput.enabled);
                invertToggle.RegisterValueChangedCallback(e => {
                    channelInput.invert = e.newValue;
                    onChanged?.Invoke(this);
                });
                row.Add(invertToggle);

                enableToggle.RegisterValueChangedCallback(e =>
                {
                    channelInput.enabled = e.newValue;
                    if (e.newValue) row.RemoveFromClassList("ch-disabled");
                    else            row.AddToClassList("ch-disabled");
                    modeBtn.SetEnabled(e.newValue);
                    sourceContainer.SetEnabled(e.newValue);
                    outputField.SetEnabled(e.newValue);
                    invertToggle.SetEnabled(e.newValue);
                    onChanged?.Invoke(this);
                });

                foldout.Add(row);
            }

            root.Add(foldout);
            return root;
        }

        // Invisible spacer that occupies the same flex slot as its column class.
        private static VisualElement Spacer(string columnClass)
        {
            var e = new VisualElement();
            e.AddToClassList(columnClass);
            return e;
        }

        // Dimmed label that occupies the same flex slot as its column class.
        private static Label ColHeader(string text, string columnClass)
        {
            var l = new Label(text);
            l.AddToClassList(columnClass);
            l.AddToClassList("col-header");
            return l;
        }

        private static string ModeLabel(ChannelSourceMode m) => m == ChannelSourceMode.Constant ? "#" : "C";
    }
}
