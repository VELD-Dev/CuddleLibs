using BepInEx;
using HarmonyLib;
using Nautilus.OutcropsHelper.Patchers;
using Nautilus.OutcropsHelper.Utility;

namespace Nautilus.OutcropsHelper;

[BepInPlugin("com.snmodding.nautilus.outcropshelper", "Nautilus Outcrops Extension", "1.0.0")]
internal class Initializer : BaseUnityPlugin
{
    private void Awake()
    {
        InternalLogger.Initialize(Logger);
        var harmony = new Harmony("com.snmodding.nautilus.outcropshelper");

        InternalLogger.Info("Using Nautilus.OutcropsHelper");
        BreakableResourcePatcher.Patch(harmony);
    }
}
