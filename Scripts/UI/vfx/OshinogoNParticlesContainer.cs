using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Oshinogo.Scripts.UI.vfx
{
    public partial class OshinogoNParticlesContainer : NParticlesContainer
    {
        public void TriggerRoundStartRefillAnimation()
        {
            var counter = GetParentOrNull<OshinogoNEnergyCounter>();
            counter?.CallDeferred(nameof(OshinogoNEnergyCounter.PlayRoundStartRefillHighlight));
        }
    }

}
