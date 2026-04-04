using BaseLib.Config;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;


namespace Oshinogo.Scripts;


[ModInitializer("Init")]
public class Entry
{
    // 初始化函数
    public static void Init()
    {
        // 打patch（即修改游戏代码的功能）用
        // 传入参数随意，只要不和其他人撞车即可
        var harmony = new Harmony("sts2.lingkong.Oshinogo");
        harmony.PatchAll();
        // 使得tscn可以加载自定义脚本


        // 注册配置界面
        ModConfig modConfig = new ModConfig();
        ModConfigRegistry.Register("Oshinogo", modConfig);

        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        Log.Debug("Mod initialized!");
    }
}
