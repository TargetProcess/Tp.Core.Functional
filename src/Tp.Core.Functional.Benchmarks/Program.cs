using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Tp.Core.Functional.Benchmarks.DictionaryExtensionsBenchmarks;
using Tp.Core.Functional.Benchmarks.MaybeBenchmarks;

namespace Tp.Core.Functional.Benchmarks
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			RunBenchmark<DictionaryExtensions_GetValueBenchmark>();
			RunBenchmark<Maybe_GetOrThrowBenchmark>();
			RunBenchmark<Maybe_EqualsBenchmark>();
		}

		private static Summary RunBenchmark<T>()
		{
			return BenchmarkRunner.Run<T>(
				ManualConfig
					.Create(DefaultConfig.Instance)
					.With(new MemoryDiagnoser())
					.With(MarkdownExporter.GitHub));
		}
	}
}
