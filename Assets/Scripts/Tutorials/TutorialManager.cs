/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 2/20/2023 8:27:06 AM
 * 
 * Description: TODO
*********************************/
using System.Collections;
using static InspectorValues;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
public class TutorialManager : MonoBehaviour
{
    #region Fields
    [Tooltip("These tutorials are played in the order they are set")]
    [SerializeField] TutorialElement[] tutorialElements = new TutorialElement[0];

    [Tooltip("The volume for the music to be while the tutorial is playing")]
    [SerializeField] private float tutorialMusicVolume = 0.05f;

    /// <summary>
    /// The AudioSource for game state events.
    /// </summary>
    private AudioSource audioSource;

    private AudioSource musicSource;

    private float musicStartingVolume;

    public static bool IsPlaying = false;

    private static TutorialManager tutorialManagerSceneInstance;

    private int movementIndex = 0;
    private int timingIndex = 0;
    #endregion

    #region Functions
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        tutorialManagerSceneInstance = this;

        var musicObj = GameObject.FindGameObjectWithTag("Music");

        if (musicObj != null)
        {
            musicSource = musicObj.GetComponent<AudioSource>();
            musicStartingVolume = musicSource.volume;
        }
    }

    public void StartTutorial()
    {
        SetMusicVolume(tutorialMusicVolume);

        StartCoroutine(TutorialLoop());
    }

    public static void StopTutorial()
    {
        tutorialManagerSceneInstance.SetMusicVolume(tutorialManagerSceneInstance.musicStartingVolume);

        tutorialManagerSceneInstance.StopAllCoroutines();
    }

    private void SetMusicVolume(float newVolume)
    {
        if (musicSource != null)
        {
            musicSource.volume = newVolume;
        }
    }

    private IEnumerator TutorialLoop()
    {
        IsPlaying = true;
        PostTutorialMessage.showMessage = true;

        BasketMovement.LockMovement();

        yield return Countdown.CountdownLoop();

        /*
        foreach(var tutorial in tutorialElements)
        {
            if (CheckTutorialBranch(tutorial.TutorialBranchReason))
            {
                yield return PlayTutorial(tutorial);
            }
        }*/

        GameController.instance.PlayAgain();

        IsPlaying = false;
    }

    private bool CheckTutorialBranch(TutorialElement.TutorialBranching branchReason)
    {
        switch (branchReason)
        {
            case TutorialElement.TutorialBranching.MOVEMENTYPE:
                if(movementIndex == SettingsManager.inputType)
                {
                    movementIndex++;
                    return true;
                }
                else
                {
                    movementIndex++;
                    return false;
                }

            case TutorialElement.TutorialBranching.TIMING:
                if((timingIndex == 0 && !GameController.IsInfinite()) || (timingIndex == 1 && GameController.IsInfinite()))
                {
                    timingIndex++;
                    return true;
                }
                else
                {
                    timingIndex++;
                    return false;
                }

            case TutorialElement.TutorialBranching.NONE:
            default:
                return true;
        }
    }

    private IEnumerator PlayTutorial(TutorialElement tutorial)
    {
        #region Before Audio
        if (tutorial.PreTutorialEvent != TutorialElement.PreTutorialAction.NONE) yield return PreTutorialAction(tutorial.PreTutorialEvent);
        TutorialVideoHandler.SetVideo(tutorial.videoClip);
        #endregion

        yield return DialoguePlayer(tutorial.SubtitleText, tutorial.AudioDialogue, tutorial.DialogueVolume);

        #region After Audio
        if (tutorial.DelayAfter != 0) yield return new WaitForSeconds(tutorial.DelayAfter); // Even if 0, calling a WaitForSeconds will run for at least 1 frame
        if (tutorial.PostTutorialEvent != TutorialElement.PostTutorialAction.NONE) yield return PostTutorialAction(tutorial.PostTutorialEvent);
        #endregion
    }

    private IEnumerator PreTutorialAction(TutorialElement.PreTutorialAction action)
    {
        switch (action)
        {
            #region Spawn Apples
            case TutorialElement.PreTutorialAction.SPAWNSIDEAPPLES:
                AppleSpawner.SpawnSideApples();
                yield break;
            case TutorialElement.PreTutorialAction.SPAWNGOODAPPLE:
                BasketMovement.LockMovement();
                AppleSpawner.SpawnGoodApple();
                yield break;
            case TutorialElement.PreTutorialAction.SPAWNBADAPPLE:
                AppleSpawner.SpawnBadApple();
                yield break;
            #endregion

            #region Set Movement Types
            case TutorialElement.PreTutorialAction.SETMOVE:
                BasketMovement.SetMovementType(0);
                yield break;
            case TutorialElement.PreTutorialAction.SETLEAN:
                BasketMovement.SetMovementType(1);
                yield break;
            case TutorialElement.PreTutorialAction.SETHANDS:
                BasketMovement.SetMovementType(2);
                yield break;
            #endregion
            default:
                yield break;
        }
    }

    private IEnumerator PostTutorialAction(TutorialElement.PostTutorialAction action)
    {
        switch (action)
        {
            case TutorialElement.PostTutorialAction.WAITFORAPPLES:
                while (GameObject.FindGameObjectsWithTag("Good").Length != 0 || GameObject.FindGameObjectsWithTag("Bad").Length != 0)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield break;
            default:
                yield break;
        }
    }

    #region Helper Functions
    private IEnumerator DialoguePlayer(string subtitle, AudioClip dialogue, float dialogueVolume)
    {
        TutorialSubtitleHandler.SetSubtitle(subtitle);
        PlaySound(dialogue, dialogueVolume);

        if (dialogue != null) yield return new WaitForSeconds(dialogue.length);
    }
    #endregion

    #region Sound
    /// <summary>
    /// Plays a sound for a specified event (Has null checks built in).
    /// </summary>
    /// <param name="soundClip">The sound clip to be played.</param>
    /// <param name="soundVolume">The volume of the sound to be played.</param>
    private void PlaySound(AudioClip soundClip, float soundVolume)
    {
        if (audioSource == null || soundClip == null) return;

        audioSource.PlayOneShot(soundClip, soundVolume);
    }
    #endregion
    #endregion
}

[System.Serializable]
public class TutorialElement
{
    [Tooltip("The name shown for the tutorial in editor")]
    [field: SerializeField] public string TutorialName { get; private set; }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [field:TextArea]
    [Tooltip("The subtitle text that is displayed for the tutorial")]
    [field:SerializeField] public string SubtitleText { get; private set; }

    [field:Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Tooltip("The tag of the video to play during this tutorial")]
    [field: SerializeField] public VideoClip videoClip { get; private set; }

    [Tooltip("The dialogue that is played for the tutorial")]
    [field: SerializeField] public AudioClip AudioDialogue { get; private set; }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [field: Range(0.0f, 1.0f)]
    [Tooltip("The volume of the dialogue for the tutorial")]
    [field: SerializeField] public float DialogueVolume { get; private set; } = 1.0f;

    [field: Range(0.0f, 5.0f)]
    [Tooltip("The delay after the tutorial has finished before contining to the next tutorial element")]
    [field: SerializeField] public float DelayAfter { get; private set; } = 0.0f;

    #region Extra Loop Features
    #region Branch Tutorial
    /// <summary>
    /// Holds a reason for branching between a set of options.
    /// </summary>
    public enum TutorialBranching { NONE, MOVEMENTYPE, TIMING }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Tooltip("The reason for branching in the tutorial")]
    [field: SerializeField] public TutorialBranching TutorialBranchReason { get; private set; }
    #endregion

    #region Dialogue Interruption
    /// <summary>
    /// Events that interrupt the dialogue from happening.
    /// </summary>
    public enum InterruptDialogue { NONE, GETAPPLES }

    [Tooltip("The interruption type for the tutorial")]
    [field: SerializeField] public InterruptDialogue DialogueInterruption { get; private set; }
    #endregion

    #region Pre Tutorial Action
    /// <summary>
    /// Events that take place before the dialogue has been performed.
    /// </summary>
    public enum PreTutorialAction { NONE, SPAWNSIDEAPPLES, SPAWNGOODAPPLE, SPAWNBADAPPLE, SETLEAN, SETMOVE, SETHANDS }

    [Tooltip("The interruption type for the tutorial")]
    [field: SerializeField] public PreTutorialAction PreTutorialEvent { get; private set; }
    #endregion

    #region Post Tutorial Action
    /// <summary>
    /// Events that take place after the dialogue has been performed.
    /// </summary>
    public enum PostTutorialAction { NONE, WAITFORAPPLES }

    [Tooltip("The interruption type for the tutorial")]
    [field: SerializeField] public PostTutorialAction PostTutorialEvent { get; private set; }
    #endregion
    #endregion
}