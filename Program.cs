using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MyBenchmarks
{
    public class TryGetHashCode
    {
        private readonly List<object> mRootedObjects = new();
        private readonly ConditionalWeakTable<object, object> mWeakTable = new();
        private object mAnObjectInTheTable = null!;


        [Params(1, 100, 1000, 10000)]
        public int NumberOfObjects;

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < NumberOfObjects; i++)
            {
                var obj = new object();
                mRootedObjects.Add(obj);
                mWeakTable.Add(obj, new object());
                mAnObjectInTheTable = obj;
            }
        }

        [Benchmark]
        public bool TryGetNonExistentValue()
        {
            return mWeakTable.TryGetValue(new object(), out object _);
        }

        [Benchmark]
        public bool TryGetExistingValue()
        {
            return mWeakTable.TryGetValue(mAnObjectInTheTable, out object _);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TryGetHashCode>(null!, args);
        }
    }
}
