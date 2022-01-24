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
                    registration.InjectedTypes.Select(injectedType =>
                        (injectedType, registration)
                    )
                )
            );
        }

        public bool TryGetRegistration(Type injectedType, out IRegistration registration)
        {
            return _registrationHashTable.TryGetValue(injectedType, out registration);
        }

        public bool Contains(Type injectedType)
        {
            return _registrationHashTable.TryGetValue(injectedType, out _);
        }
    }
}