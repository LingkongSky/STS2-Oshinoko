using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Oshinogo.Scripts.Patchs
{
    // 能量表盘动画Patch
    public class NParticlesContainerPatch
    {

        [HarmonyPatch(typeof(NParticlesContainer), "Restart")]
        public static class Patch_NParticlesContainer_Restart
        {
            static bool Prefix(NParticlesContainer __instance)
            {
                if (__instance is Oshinogo.Scripts.UI.vfx.OshinogoNParticlesContainer custom)
                {
                    custom.TriggerRoundStartRefillAnimation();
                    return false;
                }

                return true;
            }
        }
    }
}
