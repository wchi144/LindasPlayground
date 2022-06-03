/*
 * using code from https://blog.ndepend.com/improve-c-code-performance-with-spant/
*/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

[Orderer(SummaryOrderPolicy.FastestToSlowest)] // Order the result
[RyuJitX64Job, LegacyJitX86Job] // Run with x64 and x86 runtimes
[MemoryDiagnoser] // Analyse the memory usage
public class SplitStringNoHeapAllocation
{
    //private string _input;
    private const int numberOfElements = 7;
    private const string intput = "1, 20, 35, 130, 3, 10, 160";
    

    [Params(10, 1000, 10000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        //_input = Enumerable.Range(0, Size).Select(i => (byte)i).ToArray();
        //_input = "1, 20, 35, 130, 3, 10, 160";
    }

    // Code to benchmark
    [Benchmark(Baseline = true)]
    public int GetStringSumWithSpan() => GetStringSumWithSpan(intput);

    [Benchmark]
    public int GetStringSum() => GetStringSum(intput);

    public int GetStringSum(string input)
    {
        var numbers = input.Split(",").Select(x => int.Parse(x));
        var sum = 0;
        foreach (var number in numbers)
        {
            sum += number;
        }
        return sum;
    }

    public int GetStringSumWithSpan(string input)
    {
        var spanOfNumber = SplitUsingSpan(input);
        var sum = 0;
        foreach (var span in spanOfNumber)
        {
            sum += span;
        }
        return sum;
    }

    public int[] SplitUsingSpan(string input)
    {
        var result = new int[numberOfElements];
        ReadOnlySpan<char> spanChar = input.AsSpan();
        var nextCommaIndex = 0;
        var insertValueAtIndex = 0;
        var isLastLoop = false;

        while (!isLastLoop)
        {
            var indexStart = nextCommaIndex;
            nextCommaIndex = input.IndexOf(',', indexStart);

            isLastLoop = nextCommaIndex == -1;
            if (isLastLoop)
            {
                nextCommaIndex = input.Length;
            }
            ReadOnlySpan<char> slice = spanChar.Slice(indexStart, nextCommaIndex - indexStart);
            var value = int.Parse(slice);
            result[insertValueAtIndex] = value;
            insertValueAtIndex++;
            nextCommaIndex++;
        }
        return result;
    }
}
