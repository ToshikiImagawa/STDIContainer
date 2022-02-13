// Copyright (c) 2022 COMCREATE. All rights reserved.

#pragma warning disable SA1649
namespace ResolverCompiler.CodeAnalysis
{
    public interface IConstructorInfo
    {
        string FullName { get; }
        string ConstructorName { get; }
        IParameterTypeInfo[] ParameterTypeInfos { get; }
    }

    public interface IParameterTypeInfo
    {
        string FullName { get; }
    }
}
#pragma warning restore SA1649