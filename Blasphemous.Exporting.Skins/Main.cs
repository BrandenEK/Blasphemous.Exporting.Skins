using BepInEx;

namespace Blasphemous.Exporting.Skins;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "0.1.0")]
internal class Main : BaseUnityPlugin
{
    public static SkinExporter SkinExporter { get; private set; }

    private void Start()
    {
        SkinExporter = new SkinExporter();
    }
}
