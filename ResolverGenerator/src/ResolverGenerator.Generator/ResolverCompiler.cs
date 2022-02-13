// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Hosting;
using ResolverCompiler;

namespace ResolverGenerator.Generator
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ResolverCompiler : ConsoleAppBase
    {
        private const string InputDescription =
                "Input path of analyze MSBuild project file or the directory containing Unity source files.";

        private const string OutputDescription =
            "Output file path(.cs) or directory (multiple generate file).";

        private const string ConditionalDescription =
            "Conditional compiler symbols, split with ','. Ignored if a project file is specified for input.";

        private const string ResolverNameDescription =
            "Set resolver name.";

        private const string NamespaceDescription =
            "Set namespace root name.";

        private static async Task Main(string[] args)
        {
            var instance = MSBuildLocator.RegisterDefaults();
            AssemblyLoadContext.Default.Resolving += (assemblyLoadContext, assemblyName) =>
            {
                var path = Path.Combine(instance.MSBuildPath, assemblyName.Name + ".dll");
                return File.Exists(path) ? assemblyLoadContext.LoadFromAssemblyPath(path) : null;
            };
            await Host.CreateDefaultBuilder()
                .ConfigureLogging(logging => logging.ReplaceToSimpleConsole())
                .RunConsoleAppFrameworkAsync<ResolverCompiler>(args);
        }

#pragma warning disable CS1998
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task RunAsync(
            [Option("i", InputDescription)] string input,
            [Option("o", OutputDescription)] string output,
            [Option("c", ConditionalDescription)] string? conditionalSymbol = null,
            [Option("r", ResolverNameDescription)] string resolverName = "GeneratedResolver",
            [Option("n", NamespaceDescription)] string @namespace = "STDIC"
        )
#pragma warning restore CS1998
        {
            Workspace? workspace = null;
            try
            {
                Compilation compilation;
                if (Directory.Exists(input))
                {
                    var preprocessorSymbols = conditionalSymbol?.Split(',');
                    compilation = await PseudoCompilation.CreateFromDirectoryAsync(
                        input,
                        preprocessorSymbols,
                        Context.CancellationToken
                    );
                }
                else
                {
                    (workspace, compilation) =
                        await OpenMsBuildProjectAsync(input, Context.CancellationToken);
                }

                await new CodeGenerator(Console.WriteLine, Context.CancellationToken)
                    .GenerateFileAsync(
                        compilation,
                        output,
                        resolverName,
                        @namespace
                    )
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await Console.Error.WriteLineAsync("Canceled");
                throw;
            }
            finally
            {
                workspace?.Dispose();
            }
        }

        private static async Task<(Workspace Workspace, Compilation Compilation)> OpenMsBuildProjectAsync(
            string projectPath,
            CancellationToken cancellationToken
        )
        {
            var workspace = MSBuildWorkspace.Create();
            try
            {
                var logger = new ConsoleLogger(Microsoft.Build.Framework.LoggerVerbosity.Quiet);
                var project = await workspace.OpenProjectAsync(projectPath, logger, null, cancellationToken);
                var compilation = await project.GetCompilationAsync(cancellationToken);
                if (compilation is null)
                {
                    throw new InvalidOperationException("The project does not support creating Compilation.");
                }

                return (workspace, compilation);
            }
            catch
            {
                workspace.Dispose();
                throw;
            }
        }
    }
}