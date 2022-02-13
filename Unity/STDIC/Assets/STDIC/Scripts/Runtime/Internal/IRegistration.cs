// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;

namespace STDIC.Internal
{
    internal interface IRegistration
    {
        /// <summary>
        /// 約定された型一覧
        /// </summary>
        public IEnumerable<Type> ContractTypes { get; }

        /// <summary>
        /// インスタンスの型
        /// </summary>
        public Type InstanceType { get; }

        /// <summary>
        /// 依存している型一覧
        /// </summary>
        public IEnumerable<Type> DependentTypes { get; }

        /// <summary>
        /// スコープ
        /// </summary>
        public ScopeType ScopeType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public object GetInstance(DiContainer container);
    }
}