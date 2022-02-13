// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ResolverGenerator.Generator
{
    internal static class PseudoCompilation
    {
        internal static async Task<CSharpCompilation> CreateFromDirectoryAsync(
            string directoryRoot,
            IEnumerable<string>? preprocessorSymbols,
            CancellationToken cancellationToken
        )
        {
            var parseOption = new CSharpParseOptions(
                LanguageVersion.Latest,
                DocumentationMode.Parse,
                SourceCodeKind.Regular,
                preprocessorSymbols?.Where(x => !string.IsNullOrWhiteSpace(x))
            );
            var syntaxTrees = new List<SyntaxTree>();
            var hasInjectAnnotations = false;
            foreach (var file in IterateCsFileWithoutBinObj(directoryRoot))
            {
                using var streamReader = new StreamReader(NormalizeDirectorySeparators(file), Encoding.UTF8);
                var text = await streamReader.ReadToEndAsync();
                var syntax = CSharpSyntaxTree.ParseText(text, parseOption);
                syntaxTrees.Add(syntax);
                if (Path.GetFileNameWithoutExtension(file) != "InjectAttribute") continue;
                var root = await syntax.GetRootAsync(cancellationToken).ConfigureAwait(false);
                if (root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Any(x => x.Identifier.Text == "InjectAttribute"))
                {
                    hasInjectAnnotations = true;
                }
            }

            if (!hasInjectAnnotations)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(DummyInjectAnnotation, parseOption));
            }

            var metadata = GetStandardReferences().Select(x => MetadataReference.CreateFromFile(x)).ToArray();

            var compilation = CSharpCompilation.Create(
                "CodeGenTemp",
                syntaxTrees,
                DistinctReference(metadata),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

            return compilation;
        }

        private static IEnumerable<MetadataReference> DistinctReference(
            IEnumerable<MetadataReference> metadataReferences
        )
        {
            var set = new HashSet<string>();
            foreach (var item in metadataReferences)
            {
                if (item.Display != null && set.Add(Path.GetFileName(item.Display)))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<string> GetStandardReferences()
        {
            var standardMetadataType = new[]
            {
                typeof(object),
                typeof(Attribute),
                typeof(Enumerable),
                typeof(Task<>),
                typeof(IgnoreDataMemberAttribute),
                typeof(System.Collections.Hashtable),
                typeof(System.Collections.Generic.List<>),
                typeof(System.Collections.Generic.HashSet<>),
                typeof(System.Collections.Immutable.IImmutableList<>),
                typeof(System.Linq.ILookup<,>),
                typeof(System.Tuple<>),
                typeof(System.ValueTuple<>),
                typeof(System.Collections.Concurrent.ConcurrentDictionary<,>),
                typeof(System.Collections.ObjectModel.ObservableCollection<>),
            };

            var metadata = standardMetadataType
                .Select(x => x.Assembly.Location)
                .Distinct()
                .ToList();

            var dir = new FileInfo(typeof(object).Assembly.Location).Directory ??
                      throw new NullReferenceException("Assembly location directory not found!");
            {
                var path = Path.Combine(dir.FullName, "netstandard.dll");
                if (File.Exists(path))
                {
                    metadata.Add(path);
                }
            }

            {
                var path = Path.Combine(dir.FullName, "System.Runtime.dll");
                if (File.Exists(path))
                {
                    metadata.Add(path);
                }
            }

            return metadata;
        }

        private static IEnumerable<string> IterateCsFileWithoutBinObj(string root)
        {
            foreach (var item in Directory.EnumerateFiles(root, "*.cs", SearchOption.TopDirectoryOnly))
            {
                yield return item;
            }

            foreach (var dir in Directory.GetDirectories(root, "*", SearchOption.TopDirectoryOnly))
            {
                var dirName = new DirectoryInfo(dir).Name;
                if (dirName == "bin" || dirName == "obj")
                {
                    continue;
                }

                foreach (var item in IterateCsFileWithoutBinObj(dir))
                {
                    yield return item;
                }
            }
        }

        private static string NormalizeDirectorySeparators(string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        private const string DummyInjectAnnotation = @"// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;

namespace STDIC
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectAttribute : Attribute
    {
    }
}";
    }
}