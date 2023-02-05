using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.DotNetCli;
using BenchmarkDotNet.Toolchains.MonoWasm;

namespace MyBenchmarks
{
    public class TryGetHashCode
    {
        private readonly List<object> mRootedObjects = new();
        private readonly ConditionalWeakTable<object, object> mWeakTable = new();
        private object mAnObjectInTheTable = null!;

        // While running these benchmarks, the number of objects has not made a difference.
        [Params(1000)]
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
        private static Job CreateJob(string name, string runtimePath)
        {
            WasmRuntime runtime = new WasmRuntime(msBuildMoniker: "net8.0",
                wasmDataDir: Path.Combine(runtimePath, "src/mono/wasm"),
                moniker: RuntimeMoniker.WasmNet80);
            NetCoreAppSettings netCoreAppSettings = new NetCoreAppSettings(
                targetFrameworkMoniker: "net8.0", runtimeFrameworkVersion: "net8.0", name: "Wasm: " + name,
                customDotNetCliPath: Path.Combine(runtimePath, "artifacts/bin/dotnet-latest/dotnet"));
            IToolchain toolChain = WasmToolchain.From(netCoreAppSettings);

            return Job.MediumRun.WithRuntime(runtime).WithToolchain(toolChain).WithId(name);
        }

        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TryGetHashCode>(DefaultConfig.Instance
                .KeepBenchmarkFiles(true)
                .AddJob(CreateJob("merge-base", @"/tank/externsrc/dotnet/runtime.main").AsBaseline())
                .AddJob(CreateJob("PR", @"/tank/externsrc/dotnet/runtime"))
                );
        }
    }
}
