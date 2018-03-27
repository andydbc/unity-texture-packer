using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ChannelPackerInput
{
    Texture2D _texture = null;
    public Texture2D Texture() { return _texture; }

    bool _fold = true;

    private List<bool> _channelsIn = new List<bool>();
    private List<int> _channelOutIndices = new List<int>();

    public ChannelPackerInput()
    {
        for(int i = 0; i < 4; ++i)
        {
            _channelsIn.Add(false);
            _channelOutIndices.Add(0);
        }
    }

    public void Draw(ChannelPackerWindow win)
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        EditorGUILayout.BeginHorizontal();
        var r = EditorGUILayout.GetControlRect();
        var rFold = r;
        rFold.width = 20;

        _fold = EditorGUI.Foldout(rFold, _fold, "Input");

        var gearStyle = new GUIStyle("Icon.Options");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(gearStyle.normal.background, new GUIStyle("IconButton")))
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove"), false, data =>
            {
                var channelInput = data as ChannelPackerInput;
                win.RemoveInput(channelInput);

            }, this);

            menu.ShowAsContext();
        }

        using (new EditorGUI.IndentLevelScope(1))
        {
            EditorGUILayout.EndHorizontal();
            if (_fold)
            {

                GUILayout.BeginHorizontal(ChannelPackerStyles.Heading);

                GUILayout.BeginVertical(ChannelPackerStyles.Heading);

                GUILayout.Label("Channels Selection:");

                string[] channels = new string[] { "Red", "Green", "Blue", "Alpha" };

                for (int i = 0; i < 4; ++i)
                {
                    GUILayout.BeginHorizontal();
                   
                    _channelsIn[i] = GUILayout.Toggle(_channelsIn[i], new GUIContent(" " + channels[i]), GUILayout.Width(60));

                    GUILayout.Label(">");
                    _channelOutIndices[i] = EditorGUILayout.Popup((int)_channelOutIndices[i], channels, GUILayout.Width(80));

                    GUILayout.EndHorizontal();
                }
               
                GUILayout.EndVertical();

                _texture = EditorGUILayout.ObjectField(_texture, typeof(Texture2D), false, GUILayout.Width(90), GUILayout.Height(80)) as Texture2D;

                GUILayout.EndHorizontal();

            }
        }
    }

    public Vector4 In()
    {
        Vector4 r = Vector4.zero;
    
        r.x = _channelsIn[0] ? 1.0f : 0.0f;
        r.y = _channelsIn[1] ? 1.0f : 0.0f;
        r.z = _channelsIn[2] ? 1.0f : 0.0f;
        r.w = _channelsIn[3] ? 1.0f : 0.0f;
        
        return r;
    }

    public Matrix4x4 Out()
    {
        Matrix4x4 m = Matrix4x4.zero;

        for(int i = 0; i < 4; ++i)
        {
            Vector4 inputChannel = Vector4.zero;
            inputChannel[_channelOutIndices[i]] = 1;
            m.SetRow(i, inputChannel);
        }

        return m;
    }
}
