using System;
using System.Collections.Generic;
using Arbor.ModelBinding.SystemTextJson;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Primitives;

[MemoryDiagnoser]
public class ParsingBenchmarks
{
    private IEnumerable<KeyValuePair<string, StringValues>> _pairs = default!;

    [GlobalSetup]
    public void Setup()
    {
        _pairs = new List<KeyValuePair<string, StringValues>>
        {
            new("Id", "42"),
            new("IsEnabled", "on"),
            new("Name", "Arbor"),
            new("Tags[0]", "alpha"),
            new("Tags[1]", "beta"),
            new("Details.Count", "3")
        };
    }

    [Benchmark(Baseline = true)]
    public object? ParseWithNewtonsoft() =>
        Arbor.ModelBinding.NewtonsoftJson.FormsExtensions.ParseFromPairs(_pairs, typeof(BenchmarkTarget));

    [Benchmark]
    public object? ParseWithSystemTextJson() =>
        FormsExtensions.ParseFromPairs(_pairs, typeof(BenchmarkTarget));

    private class BenchmarkTarget
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
        public DetailsModel Details { get; set; } = new();
    }

    private class DetailsModel
    {
        public int Count { get; set; }
    }
}
