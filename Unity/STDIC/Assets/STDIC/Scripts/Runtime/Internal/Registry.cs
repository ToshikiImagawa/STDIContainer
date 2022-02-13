// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace STDIC.Internal
{
    internal class Registry : IRegistry
    {
        private readonly TypeKeyHashTable<IRegistration> _registrationHashTable;

        public Registry(IEnumerable<IRegistration> registrations)
        {
            _registrationHashTable = new TypeKeyHashTable<IRegistration>(
                registrations.SelectMany(registration =>
                    registration.ContractTypes.Select(contractType =>
                        (contractType, registration)
                    )
                )
            );
        }

        public IEnumerable<IRegistration> Registrations => _registrationHashTable.Values;

        public bool TryGetRegistration(Type contractType, out IRegistration registration)
        {
            return _registrationHashTable.TryGetValue(contractType, out registration);
        }

        public bool Contains(Type contractType)
        {
            return _registrationHashTable.TryGetValue(contractType, out _);
        }
    }
}