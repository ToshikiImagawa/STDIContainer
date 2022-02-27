// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using STDICEditor.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace STDICEditor
{
    public class StdicGraphWindow : EditorWindow
    {
        private static StdicGraphWindow GraphWindow { get; set; }
        private Func<StdicGraphView> GraphView { get; set; }

        [MenuItem("STDIC/Open Dependency tree graph window")]
        public static void Open()
        {
            GetWindow<StdicGraphWindow>();
            GraphWindow.titleContent = new GUIContent(nameof(StdicGraphWindow));
        }

        private void Reload()
        {
            rootVisualElement.Clear();
            var visualElement = new StdicVisualElement
            {
                name = nameof(StdicVisualElement),
            };
            visualElement.Initialize();
            rootVisualElement.Add(visualElement);
            GraphView = () => visualElement.GraphView;
            Repaint();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnEnable()
        {
            GraphWindow = this;
            Undo.undoRedoPerformed += Reload;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Reload;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnDestroy()
        {
            GraphWindow = null;
        }

        private void Update()
        {
            GraphView?.Invoke()?.UpdateView();
        }
    }
}