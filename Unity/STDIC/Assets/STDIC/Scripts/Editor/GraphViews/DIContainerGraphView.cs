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
    // ReSharper disable once InconsistentNaming
    internal class DIContainerGraphView : StdicGraphView
    {
        private readonly Dictionary<string, DIContainerNode> _createdNodes = new Dictionary<string, DIContainerNode>();
        private readonly List<Edge> _createdEdges = new List<Edge>();
        private readonly Action<RegistrationNode> _onSelectedRegistrationNode;
        private DIContainerNode[] _rootNodes;
        private int _lastPositionHash = 0;

        public DIContainerGraphView(Action<RegistrationNode> onSelectedRegistrationNode)
        {
            _onSelectedRegistrationNode = onSelectedRegistrationNode;
            var disposable = STDIC.DependencyTreeGraphHelper.Instance.Subscribe(this);
            style.flexDirection = FlexDirection.Row;
            style.Flex(1);
            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ClickSelector());
            RegisterCallback<GeometryChangedEvent>(_ => { UpdateElement(); });
            RegisterCallback<DetachFromPanelEvent>(_ => { disposable?.Dispose(); });
            var gridBackground = new GridBackground
            {
                name = nameof(GridBackground),
            };
            Insert(0, gridBackground);
            UpdateElement();
        }

        public sealed override void UpdateView()
        {
            var currentPositionHash = GetNodesHash(_rootNodes);
            if (_lastPositionHash == currentPositionHash) return;
            _lastPositionHash = currentPositionHash;
            _rootNodes.Aggregate(Vector2.zero, (currentPos, node) => UpdatePosition(node, currentPos));
        }

        public sealed override void AddToSelection(ISelectable selectable)
        {
            if (!(selectable is RegistrationNode registrationNode)) return;
            Debug.Log($"[Selected] {registrationNode.Id}");
            _onSelectedRegistrationNode?.Invoke(registrationNode);
        }

        protected sealed override void UpdateElement()
        {
            _rootNodes = GetRootContainerNodes(ContainerDataMap.Keys);
            RemoveOtherNodes(ContainerDataMap.Keys);
            foreach (var node in _rootNodes)
            {
                UpdateConnect(node);
            }

            void UpdateConnect(GraphNode node)
            {
                foreach (var edge in node.OutputPort.connections)
                {
                    RemoveElement(edge);
                }

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

        private DIContainerNode[] GetRootContainerNodes(IEnumerable<string> ids)
        {
            return ids.Select(GetOrCreateContainerNode).Where(node => node.ParentContainerNode == null).ToArray();
        }

        private DIContainerNode GetOrCreateContainerNode(string id)
        {
            return _createdNodes.ContainsKey(id) ? _createdNodes[id] : CreateContainerNode(id);
        }

        private DIContainerNode CreateContainerNode(string id)
        {
            var container = ContainerDataMap[id];
            var registrationNodes = CreateRegistrationNodes(container.RegistrationDataMap);
            var parentId = container.ParentId;
            var parentNode = parentId != null
                ? GetOrCreateContainerNode(parentId)
                : null;

            var node = new DIContainerNode(
                container,
                registrationNodes,
                parentNode
            );
            _createdNodes[id] = node;
            AddElement(node);
            return node;
        }

        private void RemoveOtherNodes(ICollection<string> ids)
        {
            foreach (var id in _createdNodes.Keys)
            {
                if (ids.Contains(id)) continue;
                var node = _createdNodes[id];
                _createdNodes.Remove(id);
                RemoveNode(node);
            }
        }

        private void RemoveNode(GraphNode graphNode)
        {
            if (graphNode is DIContainerNode diContainerNode)
            {
                _createdNodes.Remove(diContainerNode.title);
            }

            foreach (var childNode in graphNode.ChildrenNode)
            {
                RemoveNode(childNode);
            }

            foreach (var edge in graphNode.InputPort.connections)
            {
                RemoveElement(edge);
            }

            RemoveElement(graphNode);
        }

        private RegistrationNode[] CreateRegistrationNodes(Dictionary<string, RegistrationData> dataMap)
        {
            RegistrationNode GetOrCreateRegistrationNode(
                Dictionary<string, RegistrationNode> registrationNodeMop,
                RegistrationData data
            )
            {
                return registrationNodeMop.ContainsKey(data.Id)
                    ? registrationNodeMop[data.Id]
                    : CreateRegistrationNode(registrationNodeMop, data);
            }

            RegistrationNode CreateRegistrationNode(
                Dictionary<string, RegistrationNode> registrationNodeMop,
                RegistrationData data
            )
            {
                var node = RegistrationNode.CreateForStdicGraphView(
                    data,
                    !string.IsNullOrEmpty(data.ParentId) && dataMap.ContainsKey(data.ParentId)
                        ? GetOrCreateRegistrationNode(registrationNodeMop, dataMap[data.ParentId])
                        : null
                );
                AddElement(node);
                return node;
            }

            var registrationNodeMop = new Dictionary<string, RegistrationNode>();
            var registrationNodes = dataMap.Values
                .Select(data => GetOrCreateRegistrationNode(registrationNodeMop, data))
                .ToArray();
            registrationNodeMop.Clear();
            return registrationNodes;
        }
    }
}