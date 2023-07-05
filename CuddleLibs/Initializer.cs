namespace CuddleLibs;

[BepInPlugin(LibInfo.LIB_GUID, LibInfo.LIB_NAME, LibInfo.LIB_VERS)]
internal class Initializer : BaseUnityPlugin
{
    private void Awake()
    {
        InternalLogger.Initialize(Logger);
        var harmony = new Harmony(LibInfo.LIB_GUID);

        InternalLogger.Info($"Thanks for using {LibInfo.LIB_DISPLAYNAME} {LibInfo.LIB_VERS}!");
        BreakableResourcePatcher.Patch(harmony);
        InternalLogger.Debug($"{LibInfo.LIB_DISPLAYNAME} {LibInfo.LIB_VERS} intialized.");
    }
}
