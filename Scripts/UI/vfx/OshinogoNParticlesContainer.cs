using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Oshinogo.Scripts.UI.vfx
{
	public partial class OshinogoNParticlesContainer : NParticlesContainer
	{
	}

	[HarmonyPatch(typeof(NParticlesContainer), "Restart")]
	public static class Patch_NParticlesContainer_Restart
	{
		static bool Prefix(NParticlesContainer __instance)
		{
			// 判断是不是你的子类
			if (__instance is Oshinogo.Scripts.UI.vfx.OshinogoNParticlesContainer custom)
			{
				return false; // 阻止原方法执行
			}

			return true; // 其他情况继续原逻辑
		}
	}
}
