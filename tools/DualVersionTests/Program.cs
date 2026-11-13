using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security.Cryptography;

// Keep the bootstrap independent of game types so resolution is configured
// before the CLR loads a host-specific game or RitsuLib assembly.
var modPath = Path.GetFullPath(args[0]);
var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(modPath)));
if (hash != args[6]) throw new InvalidOperationException("Mod DLL changed between host runs");
var directories = new[] { args[1], args[3], args[4], args[2], Path.GetDirectoryName(modPath)! };
AssemblyLoadContext.Default.Resolving += (context, name) =>
{
    var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == name.Name);
    if (loaded != null) return loaded;
    foreach (var directory in directories)
    {
        var path = Path.Combine(directory, name.Name + ".dll");
        if (File.Exists(path)) return context.LoadFromAssemblyPath(path);
    }
    return null;
};
Console.WriteLine($"HOST={args[5]}; CLR={RuntimeInformation.FrameworkDescription}; MOD_SHA256={hash}");
MetadataChecks.Run(modPath, args[1], args[2], args[3], args[4], args[7], args[8], args[5]);
var mod = AssemblyLoadContext.Default.LoadFromAssemblyPath(modPath);
var checks = Assembly.GetExecutingAssembly().GetType("RuntimeChecks", throwOnError: true)!;
await (Task)checks.GetMethod("Run")!.Invoke(null, [mod])!;
Console.WriteLine($"PASS HOST={args[5]}; MOD_SHA256={hash}");
