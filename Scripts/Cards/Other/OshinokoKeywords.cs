using STS2RitsuLib.Content;


namespace Oshinoko.Scripts.Cards.Other;


[RegisterOwnedCardKeyword(nameof(Shine), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.AfterCardDescription)]
public class OshinokoKeywords
{
    public static readonly string Shine = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Shine));
}
