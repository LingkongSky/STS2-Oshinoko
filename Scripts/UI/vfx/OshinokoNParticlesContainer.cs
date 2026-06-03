using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Oshinoko.Scripts.UI.vfx
{
    public partial class OshinokoNParticlesContainer : NParticlesContainer
    {
        public void TriggerRoundStartRefillAnimation()
        {
            var counter = GetParentOrNull<OshinokoNEnergyCounter>();
            counter?.CallDeferred(nameof(OshinokoNEnergyCounter.PlayRoundStartRefillHighlight));
        }
    }

}
