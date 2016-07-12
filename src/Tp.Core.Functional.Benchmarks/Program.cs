using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Tp.Core.Functional.Benchmarks
{
	public static class Program
	{
		public static void Main(string[] args)
		{
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
