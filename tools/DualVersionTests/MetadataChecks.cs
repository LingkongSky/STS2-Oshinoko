using Mono.Cecil;

internal static class MetadataChecks
{
    internal static void Run(string modPath, string gameData, string nativeData, string compat, string shared,
        string baselinePath, string oldGameData, string version)
    {
        using var resolver = new DefaultAssemblyResolver();
        foreach (var path in new[] { gameData, compat, shared, nativeData, AppContext.BaseDirectory })
            resolver.AddSearchDirectory(path);
        using var module = ModuleDefinition.ReadModule(modPath, new ReaderParameters { AssemblyResolver = resolver });
        using var host = ModuleDefinition.ReadModule(Path.Combine(gameData, "sts2.dll"));
        var build = (string)host.Assembly.CustomAttributes.Single(a =>
            a.AttributeType.FullName == "System.Reflection.AssemblyInformationalVersionAttribute").ConstructorArguments[0].Value;
        var expectedCommit = version switch { "0.111.0" => "+41cef1ea", "0.113.0" => "+16ac0fe3", _ => throw new InvalidOperationException("Unknown validation host") };
        Require(build.Contains(expectedCommit, StringComparison.Ordinal), $"Incorrect game DLL for {version}: {build}");
        Console.WriteLine($"GAME_BUILD={build}");
        var references = 0;
        foreach (var member in module.GetMemberReferences())
        {
            var scope = member.DeclaringType.GetElementType().Scope.Name;
            if (scope != "sts2" && !scope.StartsWith("STS2-RitsuLib", StringComparison.Ordinal)) continue;
            var resolved = member switch
            {
                MethodReference method => (IMemberDefinition?)method.Resolve(),
                FieldReference field => field.Resolve(),
                _ => null
            };
            Require(resolved != null, $"Missing host API: {member.FullName}");
            references++;
        }
        var overrides = 0;
        foreach (var type in module.GetTypes().Where(t => t.BaseType != null))
        foreach (var method in type.Methods.Where(m => m.IsVirtual && !m.IsNewSlot && m.Overrides.Count == 0))
        {
            var parent = type.BaseType!.Resolve();
            var found = false;
            while (parent != null)
            {
                if (parent.Methods.Any(m => m.IsVirtual && m.Name == method.Name &&
                    m.ReturnType.FullName == method.ReturnType.FullName &&
                    m.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(method.Parameters.Select(p => p.ParameterType.FullName))))
                {
                    found = true;
                    break;
                }
                parent = parent.BaseType?.Resolve();
            }
            Require(found, $"Orphaned override: {method.FullName}");
            overrides++;
        }
        using var oldGame = ModuleDefinition.ReadModule(Path.Combine(oldGameData, "sts2.dll"));
        using var newGame = ModuleDefinition.ReadModule(Path.Combine(nativeData, "sts2.dll"));
        var enums = 0;
        foreach (var reference in module.GetTypeReferences().Where(t => t.Scope.Name == "sts2"))
        {
            var oldType = oldGame.GetType(reference.FullName);
            if (oldType?.IsEnum != true) continue;
            var newType = newGame.GetType(reference.FullName)!;
            foreach (var field in oldType.Fields.Where(f => f.HasConstant))
            {
                var counterpart = newType.Fields.FirstOrDefault(f => f.Name == field.Name && f.HasConstant);
                if (counterpart == null) continue;
                Require(Equals(field.Constant, counterpart.Constant), $"Enum constant changed: {oldType.FullName}.{field.Name}");
                enums++;
            }
        }
        Console.WriteLine($"METADATA: {references} API references; {overrides} overrides; {enums} enum constants verified");
        if (!string.IsNullOrEmpty(baselinePath)) CompareGameplayBodies(module, baselinePath);
    }

    private static void CompareGameplayBodies(ModuleDefinition current, string baselinePath)
    {
        using var baseline = ModuleDefinition.ReadModule(baselinePath);
        var bodies = current.GetTypes().SelectMany(t => t.Methods).Where(m => m.HasBody)
            .ToDictionary(m => m.FullName, StringComparer.Ordinal);
        var compared = 0;
        foreach (var method in baseline.GetTypes().Where(t => t.FullName.StartsWith("Oshinoko.Scripts.", StringComparison.Ordinal))
                     .SelectMany(t => t.Methods).Where(m => m.HasBody))
        {
            var key = method.FullName.Replace("KamikiHikaru::AfterDeath(", "KamikiHikaru::AfterDeathCompat(", StringComparison.Ordinal);
            key = key.Replace("Oshinoko.Scripts.Powers.EncorePower", "Oshinoko.Scripts.Powers.OshinokoEncorePower", StringComparison.Ordinal);
            Require(bodies.TryGetValue(key, out var candidate), $"Gameplay method removed: {key}");
            Require(Body(method) == Body(candidate!), $"Gameplay body changed beyond the API bridge: {key}");
            compared++;
        }
        Console.WriteLine($"BASELINE: {compared} existing gameplay method bodies unchanged after normalizing compatibility calls and the Encore model rename");
    }

    private static string Body(MethodDefinition method)
    {
        var locals = string.Join(";", method.Body.Variables.Select(v => v.VariableType.FullName));
        var instructions = string.Join("\n", method.Body.Instructions.Select(i => $"{i.OpCode} {i.Operand}"));
        instructions = instructions.Replace("Oshinoko.Scripts.Compatibility.CreatureDamageCompat::Damage(",
            "MegaCrit.Sts2.Core.Commands.CreatureCmd::Damage(", StringComparison.Ordinal);
        var handlers = string.Join(";", method.Body.ExceptionHandlers.Select(h =>
            $"{h.HandlerType}:{h.CatchType}:{h.TryStart?.Offset}:{h.TryEnd?.Offset}:{h.HandlerStart?.Offset}:{h.HandlerEnd?.Offset}:{h.FilterStart?.Offset}"));
        return $"{locals}\n{instructions}\n{handlers}".Replace("Oshinoko.Scripts.Powers.OshinokoEncorePower",
            "Oshinoko.Scripts.Powers.EncorePower", StringComparison.Ordinal);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
