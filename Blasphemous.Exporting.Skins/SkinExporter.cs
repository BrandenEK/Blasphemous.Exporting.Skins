using Blasphemous.ModdingAPI;

namespace Blasphemous.Exporting.Skins;

/// <summary>
/// Gathers all player animations and exports them for the skin editor
/// </summary>
public class SkinExporter : BlasMod
{
    internal SkinExporter() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        LogError($"{ModInfo.MOD_NAME} has been initialized");
    }
}
