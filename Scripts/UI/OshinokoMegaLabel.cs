using Godot;
using MegaCrit.Sts2.addons.mega_text;

namespace Oshinoko.Scripts.UI;

[Tool]
public partial class OshinokoMegaLabel : MegaLabel
{
	private bool _autoSizeEnabled = true;

	private int _minFontSize = 32;

	private int _maxFontSize = 36;
}
