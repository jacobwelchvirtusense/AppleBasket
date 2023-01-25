/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:21:52 AM
 * 
 * Description: Handles the state of the game between countdowns,
 *              spawning, and ending the game.
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static AppleSpawner;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The current total of points the player has.
    /// </summary>
    private int currentPointTotal = 0;

    private int currentCombo = 0;

    private float comboModifier = 0.25f;

    /// <summary>
    /// The scene instance of the GameController.
    /// </summary>
    private static GameController instance;

    [Tooltip("The text prefab for increments to score")]
    [SerializeField] private GameObject scoreText;

    #region Timer
    [Header("Timer")]
    [Range(0, 300)]
    [Tooltip("The minimum value for the timer in seconds")]
    [SerializeField] private int minTimerAmount = 30;

    [Range(0, 300)]
    [Tooltip("The maximum value for the timer in seconds")]
    [SerializeField] private int maxTimerAmount = 120;

    [Range(0.0f, 1.0f)]
    [Tooltip("The current lerp between the min and max timer amounts")]
    [SerializeField] private float currentTimer = 0.5f;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]
    #endregion

    #region Time Before Start
    [Range(0, 10)]
    [Tooltip("The count down time before starting")]
    [SerializeField] private int timeBeforeStart = 3;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]
    #endregion

    #region Sound
    /// <summary>
    /// The AudioSource for game state events.
    /// </summary>
    private AudioSource audioSource;

    [Header("Sound")]
    #region Countdown Sound
    [Tooltip("The sound made with each change of the countdown")]
    [SerializeField]
    private AudioClip countDownSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the count down sound")]
    [SerializeField]
    private float countDownSoundVolume = 1.0f;
    #endregion
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Initializes components and starts the game.
    /// </summary>
    private void Awake()
    {
        instance = this;

        InitializeComponents();
    }

    /// <summary>
    /// Starts the countdown to begin the game.
    /// </summary>
    private void Start()
    {
        UIManager.InitializeTimer(Mathf.RoundToInt(Mathf.Lerp(minTimerAmount, maxTimerAmount, currentTimer)));
        StartCoroutine(CountdownLoop());
    }

    /// <summary>
    /// Initializes any components needed.
    /// </summary>
    private void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
    }
    #endregion

    #region Updating Score
    /// <summary>
    /// Updates the score the player has.
    /// </summary>
    /// <param name="increment">Increments the player's score by this amount.</param>
    public static void UpdateScore(int increment, Vector2 location)
    {
        instance.UpdateSceneScore(increment, location);
    }

    /// <summary>
    /// Updates the non-static score for the game.
    /// </summary>
    /// <param name="increment">Increments the player's score by this amount.</param>
    public void UpdateSceneScore(int increment, Vector2 location)
    {
        var actualIncrement = ComboModifier(increment);
        currentPointTotal += actualIncrement;

        var text = Instantiate(scoreText, location, Quaternion.identity);
        text.GetComponent<ScoreIncrementText>().InitializeScore(actualIncrement);

        UIManager.UpdateScore(currentPointTotal);
    }

    private int ComboModifier(int increment)
    {
        var amount = increment;

        if(increment > 0) amount += (int)((currentCombo * comboModifier) * increment);

        return amount;
    }

    #region Combo
    public static void IncreaseCombo()
    {
        instance.UpdateSceneCombo();
    }

    public void UpdateSceneCombo()
    {
        currentCombo++;
        UIManager.UpdateCombo(currentCombo);
    }

    public static void ResetCombo()
    {
        instance.ResetSceneCombo();
    }

    public void ResetSceneCombo()
    {
        currentCombo = 0;
        UIManager.UpdateCombo(currentCombo);
    }
    #endregion
    #endregion

    #region Countdown
    /// <summary>
    /// Counts down before starting the game again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountdownLoop()
    {
        int t = timeBeforeStart;

        do
        {
            UIManager.UpdateCountdown(t);

            if(t != 0) yield return new WaitForSeconds(1);
        }
        while (t-- > 0);

        StartGame();
    }

    /// <summary>
    /// Begins the game (spawning of apples and game timer).
    /// </summary>
    private void StartGame()
    {
        StartCoroutine(GameTimer());
        StartSpawningApples();
    }
    #endregion

    #region Timer
    /// <summary>
    /// The timer for the length of the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameTimer()
    {
        float t = Mathf.RoundToInt(Mathf.Lerp(minTimerAmount, maxTimerAmount, currentTimer));

        do
        {
            UIManager.UpdateTimer(t);
            yield return new WaitForEndOfFrame();
            t -= Time.deltaTime;
        }
        while (t > 0);

        EndGame();
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

    #region End Game/Output Data
    /// <summary>
    /// Displays end message, stops apples from spawning, and outputs the game data.
    /// </summary>
    private void EndGame()
    {
        UIManager.DisplayEndMessage();
        StopSpawningApples();
        OutputData();
    }

    /// <summary>
    /// Outputs the data from the players session.
    /// </summary>
    private void OutputData()
    {
        // currentPointTotal;
    }
    #endregion
    #endregion
}
