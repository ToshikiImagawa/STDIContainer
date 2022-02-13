// Copyright (c) 2022 COMCREATE. All rights reserved.

using ResolverCompiler.CodeAnalysis;

#pragma warning disable SA1649

namespace ResolverCompiler.Generator
{
    public partial class ResolverTemplate
    {
        public ResolverTemplate(
            string ns,
            string resolverName,
            IConstructorInfo[] constructorInfos
        )
        {
            Namespace = ns;
            ResolverName = resolverName;
            ConstructorInfos = constructorInfos;
        }

        public string Namespace { get; }
        public string ResolverName { get; }
        public IConstructorInfo[] ConstructorInfos { get; }
    }

    public partial class ConstructorTemplate
    {
        public ConstructorTemplate(
            string ns,
            IConstructorInfo[] constructorInfos
        )
        {
            Namespace = ns;
            ConstructorInfos = constructorInfos;
        }

        public string Namespace { get; }
        public IConstructorInfo[] ConstructorInfos { get; }
    }
}