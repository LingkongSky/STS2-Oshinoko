using Godot;
using MegaCrit.Sts2.addons.mega_text;

namespace Oshinogo.Scripts.UI;

[Tool]
public partial class OshinogoMegaLabel : MegaLabel
{
    private bool _autoSizeEnabled = true;

    private int _minFontSize = 32;

    private int _maxFontSize = 36;
}
