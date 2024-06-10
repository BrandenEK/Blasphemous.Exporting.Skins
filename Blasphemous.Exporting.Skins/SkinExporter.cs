using Blasphemous.ModdingAPI;
using Framework.Managers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Exporting.Skins;

/// <summary>
/// Gathers all player animations and exports them for the skin editor
/// </summary>
public class SkinExporter : BlasMod
{
    internal SkinExporter() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private AnimationInfo[] _animations;

    private bool _isExporting = false;
    private int _currentAnim = 0;
    private float _currentTime = 0;
    private readonly List<Sprite> _currentFrames = new();

    // Testing
    //private string _lastSprite = string.Empty;

    /// <summary>
    /// Loads list of animations to export
    /// </summary>
    protected override void OnInitialize()
    {
        InputHandler.RegisterDefaultKeybindings(new Dictionary<string, KeyCode>()
        {
            { "Export", KeyCode.F9 },
        });

        FileHandler.LoadDataAsJson("animations.json", out _animations);
        Log($"Loaded information for {_animations.Length} animations");
    }

    /// <summary>
    /// Handles export process while it is active
    /// </summary>
    protected override void OnUpdate()
    {
        if (Core.Logic.Penitent == null)
            return;

        // Testing
        //string currSprite = Core.Logic.Penitent.SpriteRenderer.sprite?.name;
        //if (currSprite != _lastSprite)
        //{
        //    Log($"Changing sprite to {currSprite}");
        //    _lastSprite = currSprite;
        //}

        if (_isExporting)
        {
            ProcessExport();
            return;
        }

        if (InputHandler.GetKeyDown("Export"))
            StartExport();
    }

    /// <summary>
    /// Starts the export process
    /// </summary>
    public void StartExport()
    {
        LogWarning("Starting skin export process...");
        Core.Input.SetBlocker("EXPORT", true);
        _isExporting = true;

        _currentAnim = 0; //LungeAttack_Lv3 - MidAirRangeAttack
        _currentTime = 0;
        _currentFrames.Clear();
    }

    /// <summary>
    /// Steps through the current animation until all frames are recorded
    /// </summary>
    private void ProcessExport()
    {
        Core.Logic.Penitent.Animator.Play(_animations[_currentAnim].StateName, 0, _currentTime);
        Sprite sprite = Core.Logic.Penitent.SpriteRenderer.sprite;

        if (sprite != null && !_currentFrames.Contains(sprite))
        {
            Log("Recording new frame: " + sprite?.name);
            _currentFrames.Add(sprite);
        }

        _currentTime += ANIM_STEP;
        if (_currentTime > 1)
        {
            NextExport();
        }
    }

    /// <summary>
    /// Saves the current animation and moves onto the next
    /// </summary>
    private void NextExport()
    {
        string animName = _animations[_currentAnim].DisplayName;
        LogWarning($"Saving {_currentFrames.Count} frames of animation '{animName}'");
        SaveFrames(animName, _currentFrames);

        _currentAnim++;
        _currentTime = 0;
        _currentFrames.Clear();

        if (_currentAnim >= _animations.Length)
            FinishExport();
    }

    /// <summary>
    /// Stops the export process
    /// </summary>
    private void FinishExport()
    {
        LogWarning("Completed skin export process");
        Core.Input.SetBlocker("EXPORT", false);
        _isExporting = false;
    }

    /// <summary>
    /// Exports all frames in an animation to a folder
    /// </summary>
    private void SaveFrames(string name, List<Sprite> frames)
    {
        string folder = Path.Combine(FileHandler.OutputFolder, name);
        Directory.CreateDirectory(folder);

        for (int i = 0; i < frames.Count; i++)
        {
            SaveFrame(Path.Combine(folder, $"{i:00}.png"), frames[i]);
        }
    }

    /// <summary>
    /// Exports a frame to a file
    /// </summary>
    private void SaveFrame(string path, Sprite frame)
    {
        File.WriteAllBytes(path, frame.GetSlicedTexture().EncodeToPNG());
    }

    /// <summary>
    /// Exports all player animation frames
    /// </summary>
    public void OldExport()
    {
        var anims = Resources.FindObjectsOfTypeAll<Animator>().OrderBy(x => x.name);
        LogError($"Loaded animators: {anims.Count()}");

        var controllers = Resources.FindObjectsOfTypeAll<RuntimeAnimatorController>().OrderBy(x => x.name);
        LogError($"Loaded controllers: {controllers.Count()}");

        foreach (Animator anim in anims)
        {
            LogWarning($"{anim.name} ({anim.runtimeAnimatorController?.name})");
        }

        foreach (RuntimeAnimatorController controller in controllers)
        {
            LogWarning($"{controller.name}");
        }
    }

    /// <summary>
    /// Saves a sprite texture into the output folder
    /// </summary>
    private void SaveSprite(Sprite sprite, string name)
    {
        string path = Path.Combine(FileHandler.OutputFolder, $"{name}.png");

        var bytes = sprite.GetSlicedTexture().EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }

    private const float ANIM_STEP = 0.02f;
}
