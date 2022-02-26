// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using STDICEditor.Data;
using STDICEditor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace STDICEditor.Node
{
    // ReSharper disable once InconsistentNaming
    internal class DIContainerNode : GraphNode
    {
        private readonly List<GraphNode> _childrenList;
        public readonly DIContainerNode ParentContainerNode;

        public override string Id { get; }
        public string Label { get; }
        public override Port InputPort { get; }
        public override Port OutputPort { get; }
        public override IEnumerable<GraphNode> ChildrenNode => _childrenList.ToArray();

        public DIContainerNode(
            DIContainerData containerData,
            IEnumerable<RegistrationNode> registrationNodes,
            [CanBeNull] DIContainerNode parentContainerNode = null
        ) : this(containerData.Id, containerData.Label)
        {
            _childrenList = new List<GraphNode>(registrationNodes);
            parentContainerNode?._childrenList.Add(this);
            ParentContainerNode = parentContainerNode;
        }

        private DIContainerNode(
            string id,
            string label
        )
        {
            Id = id;
            Label = label;
            Title = "DIContainer";
            if (!string.IsNullOrEmpty(label)) Title += $" ({label})";
            capabilities = 0;
            _childrenList = new List<GraphNode>();
            var inputPort = CreateInputPort();
            var outputPort = CreateOutputPort();
            inputPort.portColor = ColorUtil.GetHtmlStringColor("#d98200");
            outputPort.portColor = ColorUtil.GetHtmlStringColor("#d98200");
            InputPort = inputPort;
            OutputPort = outputPort;
            titleContainer.style.backgroundColor = ColorUtil.GetHtmlStringStyleColor("#d98200");
            this.TitleLabel().style.color = Color.white;
        }
    }
}