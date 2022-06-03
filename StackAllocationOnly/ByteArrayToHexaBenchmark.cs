using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Text;

[Orderer(SummaryOrderPolicy.FastestToSlowest)] // Order the result
[RyuJitX64Job, LegacyJitX86Job] // Run with x64 and x86 runtimes
[MemoryDiagnoser] // Analyse the memory usage
public class ByteArrayToHexaBenchmark
{
    // Initialize the byte array for each run
    private byte[] _array;

    [Params(10, 1000, 10000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _array = Enumerable.Range(0, Size).Select(i => (byte)i).ToArray();
    }

    // Code to benchmark
    [Benchmark(Baseline = true)]
    public string ToHexWithStringBuilder() => ToHexWithStringBuilder(_array);

    [Benchmark]
    public string ToHexWithBitConverter() => ToHexWithBitConverter(_array);

    public string ToHexWithStringBuilder(byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
            hex.Append(b.ToString("X2"));
        return hex.ToString();
    }

    public string ToHexWithBitConverter(byte[] bytes)
    {
        var hex = BitConverter.ToString(bytes);
        return hex.Replace("-", "");
    }
}