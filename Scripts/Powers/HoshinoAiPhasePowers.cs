using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Oshinoko.Scripts.Powers;

public abstract class HoshinoAiIconPower : OshinokoCustomPower
{
    public override string? CustomIconPath => "res://Oshinoko/images/powers/ai_energy.png";
    public override string? CustomBigIconPath => "res://Oshinoko/images/powers/ai_energy_big.png";
}

public abstract class HoshinoAiShineIconPower : OshinokoCustomPower
{
    public override string? CustomIconPath => "res://Oshinoko/images/powers/ruby_energy.png";
    public override string? CustomBigIconPath => "res://Oshinoko/images/powers/ruby_energy_big.png";
}

public class HoshinoAiMechanicsPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}

public class HoshinoAiPhase1DrawGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase1PlayGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase1BlockGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase2LightStickGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase3RebirthGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiScrutinyPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (fromHandDraw)
        {
            return true;
        }

        Flash();
        return false;
    }
}

public class HoshinoAiFirstCardExhaustPower : HoshinoAiIconPower
{
    private bool _shouldCreateVoidClone = true;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            _shouldCreateVoidClone = true;
            await Task.CompletedTask;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || Owner.Player == null || !_shouldCreateVoidClone)
        {
            return;
        }

        _shouldCreateVoidClone = false;

        var clone = cardPlay.Card.CreateClone();
        if (clone == null)
        {
            return;
        }

        CardCmd.ApplyKeyword(clone, CardKeyword.Ethereal);
        Flash();
        await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, Owner.Player, CardPilePosition.Top);
    }
}

public class HoshinoAiTwoCardDiscardPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || Owner.Player == null)
        {
            return;
        }

        var handCards = PileType.Hand.GetPile(Owner.Player).Cards.ToList();
        if (handCards.Count == 0)
        {
            return;
        }

        var selected = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(handCards);
        if (selected == null)
        {
            return;
        }

        Flash();
        await CardCmd.Discard(choiceContext, selected);
    }
}
