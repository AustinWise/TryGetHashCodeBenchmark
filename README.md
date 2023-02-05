
# What is this

For benchmarking these PRs, when running on WASM.

* dotnet/runtime#80059
* dotnet/runtime#81644

# How to build and run

Build WASM runtime using:

```bash
./build.sh -os browser -configuration Release
./dotnet.sh build -p:TargetOS=browser -p:TargetArchitecture=wasm -c Release src/mono/wasm/Wasm.Build.Tests /t:InstallWorkloadUsingArtifacts
```

To run benchmarks, assuming [jsvu](https://github.com/GoogleChromeLabs/jsvu) has been installed for
user `austin` .

```bash
sudo bash
export PATH=/home/austin/.jsvu/bin/:$PATH
dotnet run -c release
```

# TODO

Figure out why the `release` in `dotnet run -c release` has to be lowercase. It appears to be a
problem where the build of apps for the different runtimes are classing with the host process.
Since the child builds use uppercase `Release`, it solves the clash.
