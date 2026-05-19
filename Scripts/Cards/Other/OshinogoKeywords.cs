using STS2RitsuLib.Content;


namespace Oshinogo.Scripts.Cards.Other;


[RegisterOwnedCardKeyword(nameof(Shine), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.AfterCardDescription)]
public class OshinogoKeywords
{
    public static readonly string Shine = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Shine));
}
