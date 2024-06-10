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

    private GameObject _testAnim;
    private SpriteRenderer _sr;
    private Animator _anim;

    private bool _isPlaying = false;
    //private bool _skipIncrease = false;
    private float _currPercent = 0;

    private string _lastSprite = string.Empty;

    /// <summary>
    /// Loads list of animations to export
    /// </summary>
    protected override void OnInitialize()
    {
        FileHandler.LoadDataAsJson("animations.json", out _animations);
        Log($"Loaded information for {_animations.Length} animations");
    }

    /// <summary>
    /// Exports all data when loading the main menu
    /// </summary>
    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        //if (newLevel == "MainMenu")
            Export();
    }

    protected override void OnLateUpdate()
    {
        if (Core.Logic.Penitent == null)
            return;

        string currSprite = Core.Logic.Penitent.SpriteRenderer.sprite?.name;
        if (currSprite != _lastSprite)
        {
            //Log($"Changing sprite to {currSprite}");
            _lastSprite = currSprite;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.P))
        {
            _testAnim = new("Test anim");

            Vector3 position = Core.Logic.Penitent.transform.position;
            position.z = -5;
            _testAnim.transform.position = position;

            _sr = _testAnim.AddComponent<SpriteRenderer>();
            _sr.sortingLayerID = Core.Logic.Penitent.SpriteRenderer.sortingLayerID;
            _sr.material = Core.Logic.Penitent.SpriteRenderer.material;
            _sr.sprite = Core.Logic.Penitent.SpriteRenderer.sprite;

            _anim = _testAnim.AddComponent<Animator>();
            _anim.runtimeAnimatorController = Core.Logic.Penitent.Animator.runtimeAnimatorController;

            _isPlaying = true;
            //_skipIncrease = true;
            //float curr = 0;
            //while (curr <= 1)
            //{
            //    anim.Play("LungeAttack_Lv3", 0, curr);
            //    LogWarning(sr.sprite?.name);

            //    curr += ANIM_STEP;
            //}


            //var player = Core.Logic.Penitent;
            //var anim = player.Animator;
            //var controller = anim.runtimeAnimatorController;
            //var clips = controller.animationClips;


            //anim.Play("penitent_dodge_attack_anim", 0);

            //foreach (var clip in clips.OrderBy(c => c.name))
            //{
            //    Log(clip.name);
            //}

            //var dodgeClip = clips.First(c => c.name == "penitent_dodge_attack_anim");
            //dodgeClip.legacy = true;

            //var animator = _testAnim.GetComponent<Animation>();
            //animator.clip = dodgeClip;
            //animator.cullingType = AnimationCullingType.AlwaysAnimate;
            //animator.AddClip(dodgeClip, "dodge");
            //animator.Play();
        }

        if (_isPlaying)
        {
            //LungeAttack_Lv3 - MidAirRangeAttack
            _anim.Play("Idle", 0, _currPercent);
            LogWarning(_sr.sprite?.name);

            //if (_skipIncrease)
            //{
            //    _skipIncrease = false;
            //}
            //else
            //{
            //    _skipIncrease = true;
            //}

            _currPercent += ANIM_STEP;
            if (_currPercent > 1)
            {
                _isPlaying = false;
                _currPercent = 0;
            }
        }

        //if (_testAnim == null)
        //    return;

        //_testAnim.GetComponent<Animator>().enabled = false;

        //if (Input.GetKeyDown(KeyCode.O))
        //    _testAnim.GetComponent<Animator>().enabled = true;
    }

    /// <summary>
    /// Exports all player animation frames
    /// </summary>
    public void Export()
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
