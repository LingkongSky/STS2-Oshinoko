using System.Reflection;

namespace Oshinoko.Scripts.Compatibility;

internal static class CreatureDamageCompat
{
    private static readonly MethodInfo GroupDamageMethod = typeof(CreatureCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(method => method.Name == nameof(CreatureCmd.Damage)
            && method.GetParameters() is var parameters
            && parameters.Length is 7 or 8
            && parameters[1].ParameterType == typeof(IEnumerable<Creature>)
            && parameters[2].ParameterType == typeof(decimal));

    internal static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature>? targets,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        // 0.113 adds an optional ModifyDamageHookType parameter. Let reflection
        // supply its declared default instead of binding the DLL to that overload.
        object?[] arguments = GroupDamageMethod.GetParameters().Length == 7
            ? [choiceContext, targets, amount, props, dealer, cardSource, cardPlay]
            : [choiceContext, targets, amount, props, dealer, cardSource, cardPlay, Type.Missing];
        return (Task<IEnumerable<DamageResult>>)GroupDamageMethod.Invoke(null, arguments)!;
    }
}
