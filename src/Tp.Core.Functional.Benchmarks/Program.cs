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
			RunBenchmark<DictionaryExtensions_GetValue>();

			RunBenchmark<Maybe_GetOrThrow>();
			RunBenchmark<Maybe_Equals>();
			RunBenchmark<Maybe_OperatorEq>();
			RunBenchmark<Maybe_OperatorNotEq>();
			RunBenchmark<Maybe_SelectMany>();
			RunBenchmark<Maybe_Any>();

			RunBenchmark<Nothing_OperatorEq>();
			RunBenchmark<Nothing_OperatorNotEq>();
			RunBenchmark<Nothing_EqualsObject>();

			RunBenchmark<MaybeEnumerableExtensions_SelectMany_MaybeAsSource>();
			RunBenchmark<MaybeEnumerableExtensions_SelectMany_EnumerableAsSource>();
			RunBenchmark<MaybeEnumerableExtensions_ToEnumerable>();
			RunBenchmark<MaybeEnumerableExtensions_FirstOrNothing_WithoutPredicate>();
			RunBenchmark<MaybeEnumerableExtensions_SingleOrNothing_WithPredicate>();
			RunBenchmark<MaybeEnumerableExtensions_SingleOrNothing_WithoutPredicate>();
			RunBenchmark<MaybeEnumerableExtensions_Bind>();
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
