// Copyright (c) 2022 COMCREATE. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ResolverCompiler
{
    internal static class RoslynExtensions
    {
        /// <summary>
        /// Compilationより取得できる全NamedTypeSymbol
        /// </summary>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public static IEnumerable<INamedTypeSymbol> GetNamedTypeSymbols(this Compilation compilation)
        {
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semModel = compilation.GetSemanticModel(syntaxTree);

                foreach (var item in syntaxTree.GetRoot()
                             .DescendantNodes()
                             .Select(x => semModel.GetDeclaredSymbol(x)))
                {
                    if (item is INamedTypeSymbol namedType)
                    {
                        yield return namedType;
                    }
                }
            }
        }

        /// <summary>
        /// 全てのメソッドを取得する
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static IEnumerable<IMethodSymbol> GetAllConstructors(this INamedTypeSymbol symbol)
        {
            return symbol.Constructors
                .Where(s => s.MethodKind == MethodKind.Constructor)
                .ToArray();
        }

        /// <summary>
        /// 指定したAttributeが含まれているか
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="attributeNamedTypeSymbol"></param>
        /// <returns></returns>
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeNamedTypeSymbol)
        {
            return symbol.GetAttributes()
                .Select(x => x.AttributeClass)
                .Any(x2 => x2!.IsApproximatelyEqual(attributeNamedTypeSymbol));
        }

        /// <summary>
        /// {名前空間.クラス名}を取得
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetNamespaceAndName(this ITypeSymbol symbol)
        {
            return symbol.ContainingNamespace?.IsNamespace ?? false
                ? $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}"
                : symbol.Name;
        }

        private static bool IsApproximatelyEqual(this ISymbol left, ISymbol right)
        {
            if (left is IErrorTypeSymbol || right is IErrorTypeSymbol)
            {
                return left.ToDisplayString() == right.ToDisplayString();
            }

            return SymbolEqualityComparer.Default.Equals(left, right);
        }
    }
}