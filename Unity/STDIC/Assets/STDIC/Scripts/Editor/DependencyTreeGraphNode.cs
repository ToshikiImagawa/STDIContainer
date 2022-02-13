// Copyright (c) 2022 COMCREATE. All rights reserved.

using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace STDICEditor
{
    public class DependencyTreeGraphNode : Node
    {
        public Port InputPort { get; }
        public Port OutputPort { get; }

        public DependencyTreeGraphNode(string title, Vector2 position)
        {
            Title = title;
            SetPosition(new Rect(position, GetPosition().size));
            InputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);

            OutputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                typeof(Port));
            OutputPort.portName = "Out";
            outputContainer.Add(OutputPort);
            capabilities -= Capabilities.Deletable;
            capabilities -= Capabilities.Copiable;
        }

        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }

        public sealed override Rect GetPosition()
        {
            return base.GetPosition();
        }

        private string Title
        {
            set => base.title = value;
        }
    }
}