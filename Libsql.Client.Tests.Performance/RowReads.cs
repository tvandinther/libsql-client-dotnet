using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.ConsoleArguments.ListBenchmarks;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

namespace Libsql.Client.Tests.Performance;

// [SimpleJob(RuntimeMoniker.Net481)]
// [SimpleJob(RuntimeMoniker.Net70)]
[MemoryDiagnoser]
public class RowReads
{
    [Params(100, 1000, 10000)]
    public int N { get; set; }
    private readonly IDatabaseClient _db = DatabaseClient.Create();
    private readonly Random _random = new Random(42);
    private string[]? _randomStrings;

    [GlobalSetup]
    public void Setup()
    {
        _randomStrings = Enumerable.Range(0, N).Select(_ => GenerateRandomString()).ToArray();
    }

    [Benchmark]
    public List<string> ReadString()
    {
        var results = new List<string>();

        foreach (var randomString in _randomStrings!)
        {
            var result = _db.Execute($"SELECT '{randomString}' as `val`").Result;
            var text = result.Rows.First().First() as Text;

            results.Add(text!.Value);
        }

        return results;
    }

    private string GenerateRandomString()
    {
        var length = _random.Next(1, 257); // Generate a random length between 1 and 256

        // Generate random UTF-8 characters and build a string
        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            // Generate a random character (ASCII range)
            chars[i] = (char)_random.Next('a', 'z' + 1); // You can adjust the range as needed
        }

        return new string(chars);
    }
}