
namespace Blasphemous.Exporting.Skins;

/// <summary>
/// Provides information about an animation to export
/// </summary>
public class AnimationInfo
{
    /// <summary>
    /// The exported name of this animation
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// The name of the controller that contains the animation
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// The name of the state that contains the animation
    /// </summary>
    public string StateName { get; set; }
}
