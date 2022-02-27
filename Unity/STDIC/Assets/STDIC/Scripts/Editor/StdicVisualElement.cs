// Copyright (c) 2022 COMCREATE. All rights reserved.

using STDICEditor.Data;
using STDICEditor.GraphViews;
using STDICEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace STDICEditor
{
    public class StdicVisualElement : VisualElement
    {
        internal StdicGraphView GraphView { get; private set; }
        private VisualElement Content { get; set; }

        public void Initialize()
        {
            style.Flex(1);
            var toolbar = new IMGUIContainer(
                () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (GUILayout.Button(nameof(Load), EditorStyles.toolbarButton))
                    {
                        if (Contains(Content))
                        {
                            Remove(Content);
                        }

                        Load();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            );
            Add(toolbar);
            Load();
        }

        private void Load()
        {
            Load(new DIContainerGraphView(selectedRegistrationNode =>
            {
                if (Contains(Content))
                {
                    Remove(Content);
                }

                Load(selectedRegistrationNode.Data);
            })
            {
                name = nameof(DIContainerGraphView),
            });
        }

        private void Load(RegistrationData selectedRegistrationData)
        {
            Load(new RegistrationGraphView(selectedRegistrationData)
            {
                name = nameof(RegistrationGraphView),
            });
        }

        private void Load(StdicGraphView graphView)
        {
            Content = new VisualElement
            {
                name = "Content"
            };
            Content.style.Flex(1);
            Content.Add(graphView);
            GraphView = graphView;
            Add(Content);
        }
    }
}