// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using ResolverCompiler.CodeAnalysis;
using ResolverCompiler.Generator;

namespace ResolverCompiler
{
    internal static class CodeGeneratorTools
    {
        private static readonly Encoding NoBomUtf8 = new UTF8Encoding(false);

        /// <summary>
        /// ファイルを書き出す
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task OutputAsync(
            string path,
            string text,
            Action<string> logger,
            CancellationToken cancellationToken
        )
        {
            const string prefix = "[Out]";
            logger(prefix + path);

            var fi = new FileInfo(path);
            if (fi.Directory is { Exists: false })
            {
                fi.Directory.Create();
            }

            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using var fileStream = File.Create(path);
            using var streamWriter = new StreamWriter(fileStream, NoBomUtf8);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await streamWriter.WriteAsync(NormalizeNewLines(text));
        }

        public static string GenerateFile(
            string ns,
            string resolverName,
            IConstructorInfo[] constructorInfos
        )
        {
            var resolverTemplate = new ResolverTemplate(
                ns,
                resolverName,
                constructorInfos
            );
            var constructorTemplate = new ConstructorTemplate(
                ns,
                constructorInfos
            );
            var sb = new StringBuilder();
            sb.AppendLine("// Copyright (c) 2022 COMCREATE. All rights reserved.");
            sb.AppendLine(resolverTemplate.TransformText());
            sb.AppendLine();
            sb.AppendLine(constructorTemplate.TransformText());
            return sb.ToString();
        }

        public static TypeCollector CreateTypeCollector(
            Compilation compilation,
            Action<string> logger
        )
        {
            return new TypeCollector(compilation, logger);
        }

        /// <summary>
        /// 改行コードをEnvironment.NewLineに変換する
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string NormalizeNewLines(string content)
        {
            // The T4 generated code may be text with mixed line ending types. (CR + CRLF)
            // We need to normalize the line ending type in each Operating Systems. (e.g. Windows=CRLF, Linux/macOS=LF)
            return content.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
        }
    }
}