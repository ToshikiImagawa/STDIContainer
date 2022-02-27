// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using JetBrains.Annotations;
using STDIC.Internal;

namespace STDICEditor.Data
{
    internal struct RegistrationData
    {
        public RegistrationData(
            [NotNull] Type instanceType,
            ScopeType scopeType,
            [NotNull] string[] dependentIds,
            [NotNull] Type[] contractTypes,
            [NotNull] string parentId
        )
        {
            Id = instanceType.FullName ?? string.Empty;
            DependentIds = dependentIds;
            ParentId = parentId;
            ContractTypes = contractTypes;
            ScopeType = scopeType;
            InstanceType = instanceType;
        }

        [NotNull] public string Id { get; }
        [NotNull] public string ParentId { get; }
        [NotNull] public string[] DependentIds { get; }
        [NotNull] public Type[] ContractTypes { get; }
        [NotNull] public Type InstanceType { get; }
        public ScopeType ScopeType { get; }
    }
}