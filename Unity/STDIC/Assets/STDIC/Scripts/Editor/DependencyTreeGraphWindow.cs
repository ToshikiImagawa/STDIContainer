// Copyright (c) 2022 COMCREATE. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace STDICEditor
{
    public class DependencyTreeGraphWindow : EditorWindow
    {
        private DependencyTreeGraphView _graph;
        private DependencyTreeGraphView Graph => _graph ??= new DependencyTreeGraphView();

        [MenuItem("STDIC/Open Dependency tree graph window")]
        public static void Open()
        {
            var graphEditor = CreateInstance<DependencyTreeGraphWindow>();
            graphEditor.Show();
            graphEditor.titleContent = new GUIContent(nameof(DependencyTreeGraphWindow));
            graphEditor.Init();
        }

        private void Init()
        {
            rootVisualElement.Add(Graph);
        }
    }
}