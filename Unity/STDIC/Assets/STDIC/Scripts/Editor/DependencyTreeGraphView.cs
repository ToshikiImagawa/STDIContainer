// Copyright (c) 2022 COMCREATE. All rights reserved.

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace STDICEditor
{
    public class DependencyTreeGraphView : GraphView
    {
        public DependencyTreeGraphView()
        {
            style.flexGrow = 1;
            style.flexShrink = 1;

            SetupZoom(
                ContentZoomer.DefaultMinScale,
                ContentZoomer.DefaultMaxScale
            );
            Insert(0, new GridBackground());
            this.AddManipulator(new SelectionDragger());
            UpdateView();
        }

        public void UpdateView()
        {
            Add(new DependencyTreeGraphNode("Sample App", new Vector2(0, 0)));
            Add(new DependencyTreeGraphNode("Sample App 2", new Vector2(150, 0)));
            Add(new DependencyTreeGraphNode("Sample App 3", new Vector2(300, 0)));
        }
    }
}