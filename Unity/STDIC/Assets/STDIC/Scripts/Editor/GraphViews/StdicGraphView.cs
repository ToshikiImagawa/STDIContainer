// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using STDIC;
using STDIC.Internal;
using STDICEditor.Data;
using STDICEditor.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace STDICEditor.GraphViews
{
    internal abstract class StdicGraphView : GraphView, IObserver<DependencyTreeGraphHelper.ContainerData>
    {
        private static readonly Vector2 Span = new Vector2(50, 20);

        [NotNull] protected Dictionary<string, DIContainerData> ContainerDataMap { get; }

        protected StdicGraphView()
        {
            ContainerDataMap = new Dictionary<string, DIContainerData>();
            foreach (var data in DependencyTreeGraphHelper.Instance.Values)
            {
                ContainerDataMap[data.ID] = new DIContainerData(
                    data.ID,
                    data.Label,
                    data.ParentId,
                    CreateRegistrationDataMap(
                        data.ID,
                        data.ParentId,
                        data.Registrations
                    )
                );
            }

            var disposable = DependencyTreeGraphHelper.Instance.Subscribe(this);
            RegisterCallback<GeometryChangedEvent>(_ => { UpdateElement(); });
            RegisterCallback<DetachFromPanelEvent>(_ => { disposable.Dispose(); });
        }

        protected abstract void UpdateElement();
        public abstract void UpdateView();

        void IObserver<DependencyTreeGraphHelper.ContainerData>.OnCompleted()
        {
        }

        void IObserver<DependencyTreeGraphHelper.ContainerData>.OnError(Exception error)
        {
        }

        void IObserver<DependencyTreeGraphHelper.ContainerData>.OnNext(DependencyTreeGraphHelper.ContainerData value)
        {
            ContainerDataMap[value.ID] = new DIContainerData(
                value.ID,
                value.Label,
                value.ParentId,
                CreateRegistrationDataMap(
                    value.ID,
                    value.ParentId,
                    value.Registrations
                )
            );
            UpdateElement();
        }

        protected static int GetNodesHash([NotNull] IEnumerable<GraphNode> nodeList)
        {
            var result = 0;
            foreach (var node in nodeList)
            {
                result = (result * 397) ^ node.GetHashCode();
                result = (result * 397) ^ GetNodesHash(node.ChildrenNode);
            }

            return result;
        }

        protected static Vector2 UpdatePosition(GraphNode node, Vector2 startPos)
        {
            node.SetPosition(startPos);
            var childrenStart = new Vector2(startPos.x + node.Size.x + Span.x, startPos.y);
            var nextStart = new Vector2(startPos.x, startPos.y + node.Size.y + Span.y);
            childrenStart = node.ChildrenNode.Aggregate(
                childrenStart,
                (currentPos, nodeChild) => UpdatePosition(nodeChild, currentPos)
            );
            return new Vector2(startPos.x, Mathf.Max(nextStart.y, childrenStart.y));
        }

        private Dictionary<string, RegistrationData> CreateRegistrationDataMap(
            string containerId,
            string parentId,
            IRegistration[] registrations
        )
        {
            RegistrationData GetOrCreateRegistrationData(
                Dictionary<string, RegistrationData> registrationDataMap,
                IRegistration registration
            )
            {
                var id = registration.InstanceType.FullName ?? string.Empty;
                return registrationDataMap.ContainsKey(id)
                    ? registrationDataMap[id]
                    : CreateRegistrationData(registrationDataMap, registration);
            }

            RegistrationData CreateRegistrationData(
                Dictionary<string, RegistrationData> registrationDataMap,
                IRegistration registration
            )
            {
                var data = new RegistrationData(
                    registration.InstanceType,
                    registration.ScopeType,
                    registration.DependentTypes
                        .Select(t =>
                        {
                            var hitRegistration = registrations.FirstOrDefault(r => r.ContractTypes.Contains(t));
                            if (hitRegistration != null)
                                return GetOrCreateRegistrationData(registrationDataMap, hitRegistration).Id;
                            var id = parentId;
                            while (!string.IsNullOrEmpty(id))
                            {
                                var containerData = ContainerDataMap[id];
                                if (containerData.TryGetRegistrationData(t, out var hitData))
                                {
                                    return hitData.Id;
                                }

                                id = containerData.ParentId;
                            }

                            return null;
                        })
                        .Where(r => r != null)
                        .ToArray(),
                    registration.ContractTypes.ToArray(),
                    containerId
                );
                registrationDataMap[data.Id] = data;
                return data;
            }

            var map = registrations.ToDictionary(r => r.InstanceType.FullName, r => r);
            var registrationDataMap = new Dictionary<string, RegistrationData>();
            foreach (var id in map.Keys.Where(id => !registrationDataMap.ContainsKey(id)))
            {
                GetOrCreateRegistrationData(registrationDataMap, map[id]);
            }

            return registrationDataMap;
        }
    }
}