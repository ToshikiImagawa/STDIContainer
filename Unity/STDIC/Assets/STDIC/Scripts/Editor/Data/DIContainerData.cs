// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace STDICEditor.Data
{
    // ReSharper disable once InconsistentNaming
    internal readonly struct DIContainerData
    {
        public DIContainerData(
            [NotNull] string id,
            [NotNull] string label,
            [CanBeNull] string parentId,
            [NotNull] Dictionary<string, RegistrationData> registrationDataMap)
        {
            Id = id;
            Label = label;
            ParentId = parentId;
            RegistrationDataMap = registrationDataMap;
        }

        [NotNull] public string Id { get; }
        [NotNull] public string Label { get; }
        [CanBeNull] public string ParentId { get; }
        [NotNull] public Dictionary<string, RegistrationData> RegistrationDataMap { get; }

        public bool TryGetRegistrationData(Type dependentType, out RegistrationData registrationData)
        {
            foreach (var data in RegistrationDataMap.Values.Where(data =>
                         data.ContractTypes.Contains(dependentType)))
            {
                registrationData = data;
                return true;
            }

            registrationData = default;
            return false;
        }
    }
}