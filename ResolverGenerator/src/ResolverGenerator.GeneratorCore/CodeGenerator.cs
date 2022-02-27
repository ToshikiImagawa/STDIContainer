// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace ResolverCompiler
{
    public class CodeGenerator
    {
        private readonly Action<string> _logger;
        private readonly CancellationToken _cancellationToken;

        public CodeGenerator(Action<string> logger, CancellationToken cancellationToken)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        public async Task GenerateFileAsync(
            Compilation compilation,
            string output,
            string resolverName,
            string @namespace
        )
        {
            var sw = Stopwatch.StartNew();
            _logger("Project Compilation Start:" + compilation.AssemblyName);

            var collector = CodeGeneratorTools.CreateTypeCollector(
                compilation,
                _logger
            );

            _logger($"Project Compilation Complete:{sw.Elapsed}");
            sw.Restart();
            _logger("Constructor Collect Start");

            var constructorInfos = collector.Collect();

            _logger($"Constructor Collect Complete:{sw.Elapsed}");
            sw.Restart();
            _logger("Output Generation Start");

            var generatedProgramText = CodeGeneratorTools.GenerateFile(@namespace, resolverName, constructorInfos);
            await CodeGeneratorTools.OutputAsync(
                Path.GetExtension(output) == ".cs" ? output : Path.Combine(output, $"{resolverName}.cs"),
                generatedProgramText,
                _logger,
                _cancellationToken
            );

            _logger($"Output Generation Complete:{sw.Elapsed}");
        }
    }
}