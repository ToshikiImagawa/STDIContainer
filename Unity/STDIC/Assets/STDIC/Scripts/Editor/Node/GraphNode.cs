// Copyright (c) 2022 COMCREATE. All rights reserved.

using System.Collections.Generic;
using STDICEditor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace STDICEditor.Node
{
    internal abstract class GraphNode : UnityEditor.Experimental.GraphView.Node
    {
        public abstract string Id { get; }

        public Vector2 Size =>
            GetPosition().size.Let(size => float.IsNaN(size.x) || float.IsNaN(size.y)
                ? new Vector2(float.IsNaN(size.x) ? 200f : size.x, float.IsNaN(size.y) ? 77f : size.y)
                : size
            );

        public abstract Port InputPort { get; }
        public abstract Port OutputPort { get; }
        public abstract IEnumerable<GraphNode> ChildrenNode { get; }

        public void SetPosition(Vector2 newPos)
        {
            SetPosition(new Rect(newPos, GetPosition().size));
        }

        protected Port CreateInputPort(string portName = null)
        {
            var port = Port.Create<Edge>(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Single,
                typeof(Port)
            );
            port.portName = portName ?? "In";
            inputContainer.Add(port);
            return port;
        }

        protected Port CreateOutputPort(string portName = null)
        {
            var port = Port.Create<Edge>(
                Orientation.Horizontal,
                Direction.Output,
                Port.Capacity.Multi,
                typeof(Port)
            );
            port.portName = portName ?? "Out";
            outputContainer.Add(port);
            return port;
        }

        public override int GetHashCode()
        {
            var id = title;
            var size = Size;
            var result = id?.GetHashCode() ?? 0;
            result = (result * 397) ^ size.x.GetHashCode();
            result = (result * 397) ^ size.y.GetHashCode();
            return result;
        }

        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }

        public sealed override Rect GetPosition()
        {
            return base.GetPosition();
        }

        protected string Title
        {
            get => base.title;
            set => base.title = value;
        }
    }
}