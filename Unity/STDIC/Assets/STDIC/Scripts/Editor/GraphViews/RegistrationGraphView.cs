// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using STDICEditor.Data;
using STDICEditor.Node;
using STDICEditor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace STDICEditor.GraphViews
{
    internal class RegistrationGraphView : StdicGraphView
    {
        private readonly List<GraphNode> _createdNodes = new List<GraphNode>();
        private readonly List<Edge> _createdEdges = new List<Edge>();

        private RegistrationNode _selectedRegistrationNode;
        private RegistrationData _selectedRegistrationData;
        private int _lastPositionHash;

        public RegistrationGraphView(RegistrationData selectedRegistrationData)
        {
            _selectedRegistrationData = selectedRegistrationData;
            style.flexDirection = FlexDirection.Row;
            style.Flex(1);
            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ClickSelector());
            RegisterCallback<GeometryChangedEvent>(_ => { UpdateElement(); });
            var gridBackground = new GridBackground
            {
                name = nameof(GridBackground),
            };
            Insert(0, gridBackground);
            UpdateElement();
        }

        public sealed override void UpdateView()
        {
            var currentPositionHash = GetNodesHash(new[] { _selectedRegistrationNode });
            if (_lastPositionHash == currentPositionHash) return;
            _lastPositionHash = currentPositionHash;
            UpdatePosition(_selectedRegistrationNode, Vector2.zero);
        }

        public override void AddToSelection(ISelectable selectable)
        {
            if (!(selectable is RegistrationNode registrationNode)) return;
            _selectedRegistrationData = registrationNode.Data;
            UpdateElement();
        }

        protected sealed override void UpdateElement()
        {
            var createdEdges = _createdEdges.ToArray();
            _createdEdges.Clear();
            var createdNodes = _createdNodes.ToArray();
            _createdNodes.Clear();
            foreach (var edge in createdEdges)
            {
                RemoveElement(edge);
            }

            foreach (var node in createdNodes)
            {
                RemoveElement(node);
            }

            var dataMap = ContainerDataMap[_selectedRegistrationData.ParentId].RegistrationDataMap;

            _selectedRegistrationNode = CreateRegistrationNode(dataMap[_selectedRegistrationData.Id]);
            UpdateConnect(_selectedRegistrationNode);

            void UpdateConnect(GraphNode node)
            {
                foreach (var childNode in node.ChildrenNode)
                {
                    var edge = new Edge
                    {
                        input = childNode.InputPort,
                        output = node.OutputPort
                    };
                    node.OutputPort.Connect(edge);
                    childNode.InputPort.Connect(edge);
                    AddElement(edge);
                    _createdEdges.Add(edge);
                    UpdateConnect(childNode);
                }
            }
            UpdateView();
        }

        private RegistrationNode CreateRegistrationNode(RegistrationData data)
        {
            var node = RegistrationNode.CreateForRegistrationGraphView(
                data,
                ContainerDataMap[data.ParentId],
                data.DependentIds
                    .Select(id => GetData(id, data))
                    .Select(CreateRegistrationNode)
                    .ToArray()
            );
            _createdNodes.Add(node);
            AddElement(node);
            return node;
        }

        private RegistrationData GetData(
            string id,
            RegistrationData parentData
        )
        {
            var containerData = ContainerDataMap[parentData.ParentId];
            var map = containerData.RegistrationDataMap;
            if (map.ContainsKey(id)) return map[id];
            while (containerData.ParentId != null)
            {
                containerData = ContainerDataMap[containerData.ParentId];
                map = containerData.RegistrationDataMap;
                if (map.ContainsKey(id)) return map[id];
            }

            throw new InvalidOperationException();
        }
    }
}