namespace CuddleLibs;

[BepInPlugin(LibInfo.LIB_GUID, LibInfo.LIB_NAME, LibInfo.LIB_VERS)]
internal class Initializer : BaseUnityPlugin
{
    private void Awake()
    {
        InternalLogger.Initialize(Logger);
        var harmony = new Harmony(LibInfo.LIB_GUID);

        InternalLogger.Info($"Thanks for using {LibInfo.LIB_DISPLAYNAME} {LibInfo.LIB_VERS}! Current logging level: {(InternalLogger.EnableDebugging ? "DEBUG" : "INFO")}.");
        BreakableResourcePatcher.Patch(harmony);
        CreaturePatcher.Patch(harmony);
        JSONUtils.EnsureCustomDataFiles();
        InternalLogger.Debug($"{LibInfo.LIB_DISPLAYNAME} {LibInfo.LIB_VERS} ensured existence of JSON drop datas to file.");
        JSONUtils.LoadCreatureDropsFromFile();
        JSONUtils.LoadCreatureDropsFromFile();
        InternalLogger.Debug($"{LibInfo.LIB_DISPLAYNAME} {LibInfo.LIB_VERS} loaded custom datas from JSON files.");
        InternalLogger.Debug($"{LibInfo.LIB_DISPLAYNAME} {LibInfo.LIB_VERS} intialized.");
    }
}
