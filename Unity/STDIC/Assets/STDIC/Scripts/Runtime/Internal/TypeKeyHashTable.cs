// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;

namespace STDIC.Internal
{
    internal sealed class TypeKeyHashTable<TValue> : ManagedHashTable<Type, TValue>
    {
        protected override bool CheckKeyEquals(Type left, Type right)
        {
            return left == right;
        }

        public TypeKeyHashTable()
        {
        }

        public TypeKeyHashTable(IEnumerable<(Type, TValue)> values) : base(values)
        {
        }
    }
}