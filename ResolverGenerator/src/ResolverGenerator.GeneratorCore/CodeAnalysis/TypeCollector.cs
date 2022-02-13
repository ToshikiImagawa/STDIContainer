// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ResolverCompiler.CodeAnalysis
{
    internal class TypeCollector
    {
        private readonly Action<string> _logger;
        private readonly (INamedTypeSymbol, IMethodSymbol)[] _injectedConstructorSymbols;

        private INamedTypeSymbol InjectAttribute { get; }

        public TypeCollector(
            Compilation compilation,
            Action<string> logger
        )
        {
            InjectAttribute = compilation.GetTypeByMetadataName("STDIC.InjectAttribute") ??
                              throw new InvalidOperationException("failed to get metadata of STDIC.InjectAttribute");
            _logger = logger;
            _injectedConstructorSymbols = (from namedTypeSymbol in compilation
                    .GetNamedTypeSymbols()
                    .Where(x =>
                        x.DeclaredAccessibility == Accessibility.Public ||
                        x.DeclaredAccessibility == Accessibility.Friend)
                    .Where(x =>
                        x.TypeKind == TypeKind.Class ||
                        x.TypeKind == TypeKind.Struct)
                let constructor = namedTypeSymbol
                    .GetAllConstructors()
                    .FirstOrDefault(s => s.HasAttribute(InjectAttribute))
                where constructor != null
                select (namedTypeSymbol, constructor)).ToArray();
        }

        public IConstructorInfo[] Collect()
        {
            return _injectedConstructorSymbols.Select(x =>
            {
                var (classSymbol, constructorSymbol) = x;
                return GetConstructorInfo(classSymbol, constructorSymbol);
            }).ToArray();
        }

        private static IConstructorInfo GetConstructorInfo(
            ITypeSymbol classSymbol,
            IMethodSymbol constructorSymbol
        )
        {
            var fullName = classSymbol.GetNamespaceAndName();
            return new ConstructorInfo(
                fullName,
                ConvertConstructorName(fullName),
                constructorSymbol.Parameters.Select(GetParameterTypeInfo).ToArray()
            );
        }

        private static IParameterTypeInfo GetParameterTypeInfo(IParameterSymbol parameterSymbol)
        {
            return new ParameterTypeInfo(parameterSymbol.Type.GetNamespaceAndName());
        }

        private static string ConvertConstructorName(string fullName)
        {
            return fullName.Replace('.', '_') + "Constructor";
        }

        private class ConstructorInfo : IConstructorInfo
        {
            public ConstructorInfo(
                string fullName,
                string constructorName,
                IParameterTypeInfo[] parameterTypeInfos
            )
            {
                FullName = fullName;
                ConstructorName = constructorName;
                ParameterTypeInfos = parameterTypeInfos;
            }

            public string FullName { get; }
            public string ConstructorName { get; }
            public IParameterTypeInfo[] ParameterTypeInfos { get; }
        }

        private class ParameterTypeInfo : IParameterTypeInfo
        {
            public ParameterTypeInfo(string fullName)
            {
                FullName = fullName;
            }

            public string FullName { get; }
        }
    }
}