using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Tp.Core.Functional.Benchmarks.DictionaryExtensionsBenchmarks;
using Tp.Core.Functional.Benchmarks.MaybeBenchmarks;
using Tp.Core.Functional.Benchmarks.MaybeEnumerableExtensionsBenchmarks;
using Tp.Core.Functional.Benchmarks.NothingBenchmarks;

namespace Tp.Core.Functional.Benchmarks
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			RunBenchmark<DictionaryExtensions_GetValueBenchmark>();

			RunBenchmark<Maybe_GetOrThrowBenchmark>();
			RunBenchmark<Maybe_EqualsBenchmark>();
			RunBenchmark<Maybe_OperatorEqBenchmark>();
			RunBenchmark<Maybe_OperatorNotEqBenchmark>();
			RunBenchmark<Maybe_SelectManyBenchmark>();
			RunBenchmark<Maybe_AnyBenchmark>();

			RunBenchmark<Nothing_OperatorEqBenchmark>();
			RunBenchmark<Nothing_OperatorNotEqBenchmark>();
			RunBenchmark<Nothing_EqualsObjectBenchmark>();

			RunBenchmark<MaybeEnumerableExtensions_SelectMany_MaybeSourceBenchmark>();
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
