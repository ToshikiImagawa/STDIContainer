// Copyright (c) 2022 COMCREATE. All rights reserved.

using System.Collections.Generic;
using JetBrains.Annotations;
using STDICEditor.Data;
using STDICEditor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace STDICEditor.Node
{
    internal class RegistrationNode : GraphNode
    {
        private readonly List<GraphNode> _childrenList = new List<GraphNode>();
        public RegistrationData Data { get; }
        public override string Id { get; }

        public override Port InputPort { get; }
        public override Port OutputPort { get; }
        public override IEnumerable<GraphNode> ChildrenNode => _childrenList.ToArray();

        private RegistrationNode(
            RegistrationData registration,
            RegistrationNode parentNode
        ) : this(registration)
        {
            parentNode?._childrenList.Add(this);
            var inputPort = CreateInputPort();
            var outputPort = CreateOutputPort();
            inputPort.portColor = ColorUtil.GetHtmlStringColor("#009425");
            outputPort.portColor = ColorUtil.GetHtmlStringColor("#009425");
            InputPort = inputPort;
            OutputPort = outputPort;
        }

        private RegistrationNode(
            RegistrationData registration,
            DIContainerData diContainerData,
            IEnumerable<RegistrationNode> dependentNodes
        ) : this(registration)
        {
            _childrenList.AddRange(dependentNodes);
            var inputPort = CreateInputPort("ContractType");
            var outputPort = CreateOutputPort("DependentType");
            inputPort.portColor = ColorUtil.GetHtmlStringColor("#009425");
            outputPort.portColor = ColorUtil.GetHtmlStringColor("#009425");
            InputPort = inputPort;
            OutputPort = outputPort;
            var containerLabel = new Label
            {
                text = "DIContainer"
            };
            if (!string.IsNullOrEmpty(diContainerData.Label)) containerLabel.text += $" ({diContainerData.Label})";
            containerLabel.style.Margin(6);
            containerLabel.style.PaddingH(2);
            containerLabel.style.fontSize = 12;
            containerLabel.style.color = Color.white;
            extensionContainer.Add(containerLabel);
            extensionContainer.style.backgroundColor = ColorUtil.GetHtmlStringStyleColor("#d98200");
            RefreshExpandedState();
        }

        private RegistrationNode(
            RegistrationData registration
        )
        {
            Id = registration.Id;
            Data = registration;
            Title = registration.Id;
            capabilities = Capabilities.Selectable;
            titleContainer.style.backgroundColor = ColorUtil.GetHtmlStringStyleColor("#009425");
            this.TitleLabel().style.color = Color.white;
        }

        public static RegistrationNode CreateForStdicGraphView(
            RegistrationData registration,
            [CanBeNull] RegistrationNode parentNode = null
        )
        {
            return new RegistrationNode(
                registration,
                parentNode
            );
        }

        public static RegistrationNode CreateForRegistrationGraphView(
            RegistrationData registration,
            DIContainerData diContainerData,
            [NotNull] IEnumerable<RegistrationNode> dependentNodes
        )
        {
            return new RegistrationNode(
                registration,
                diContainerData,
                dependentNodes
            );
        }
    }
}