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
using System.Linq;
using UnityEngine;
using static InspectorValues;
using static AppleSpawner;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The current total of points the player has.
    /// </summary>
    private int currentPointTotal = 0;

    private int currentCombo = 0;

    // Output Data
    private int highestComboReached = 0;
    private int goodApples = 0;
    private int badApples = 0;
    private int goodApplesMissed = 0;

    /// <summary>
    /// The scene instance of the GameController.
    /// </summary>
    public static GameController instance;

    [Tooltip("The text prefab for increments to score")]
    [SerializeField] private GameObject scoreText;

    [Tooltip("The rate of points to increase by per combo")]
    [SerializeField] private float comboModifier = 0.25f;

    #region Timer
    [Header("Timer")]
    [Tooltip("The minimum value for the timer in seconds")]
    [SerializeField] private int[] timers = new int[] { 30, 60, 120 };

    private int currentTimer = 0;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]
    #endregion

    #region Time Before
    [Range(0.0f, 5.0f)]
    [Tooltip("The count down time before starting")]
    [SerializeField] private float timeBeforeEnd = 1.0f;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]
    #endregion

    #region Infinite
    [Header("Infinite")]
    [Tooltip("The rate of points to increase by per combo")]
    [SerializeField] private float speedModIncreaseRate = 0.25f;
    [HideInInspector] public float speedMod = 1f;
    [Tooltip("The rate of points to increase by per combo")]
    [SerializeField] private int allowedMisses = 3;

    public static float InfiniteSpeedMod { get => instance.speedMod; }
    #endregion

    #region Sound
    /// <summary>
    /// The AudioSource for game state events.
    /// </summary>
    private AudioSource audioSource;

    [Header("Sound")]
    #region End Sound
    [Tooltip("The sound made when the game ends")]
    [SerializeField]
    private AudioClip endSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the end sound")]
    [SerializeField]
    private float endSoundVolume = 1.0f;
    #endregion

    [Header("Pickup Sounds")]
    #region Pickup Sounds
    #region Good Pickup Sound
    [Tooltip("The sound made when picking up a good apple")]
    [SerializeField]
    private AudioClip goodPickupSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the good pickup sound")]
    [SerializeField]
    private float goodPickupSoundVolume = 1.0f;
    #endregion

    #region Countdown Sound
    [Tooltip("The sound made when picking up a bad apple")]
    [SerializeField]
    private AudioClip badPickupSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the good pickup sound")]
    [SerializeField]
    private float badPickupSoundVolume = 1.0f;
    #endregion
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
    /// Initializes any components needed.
    /// </summary>
    private void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGameCountdown()
    {
        //var timer = isInfinite ? -1 : Mathf.RoundToInt(Mathf.Lerp(minTimerAmount, maxTimerAmount, currentTimer));

        UIManager.InitializeTimer(GetTimerAmount());
        StartCoroutine(CountdownLoop());
    }
    #endregion

    public static void UpdateTimer(int newTimerSlot)
    {
        instance.currentTimer = newTimerSlot;
    }

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


        // Spawns text at score location
        var text = Instantiate(scoreText, location, Quaternion.identity);
        text.GetComponent<ScoreIncrementText>().InitializeScore(actualIncrement);
        UIManager.UpdateScore(currentPointTotal);

        if(increment > 0)
        {
            PlaySound(goodPickupSound, goodPickupSoundVolume);
            goodApples++;
        }
        else
        {
            PlaySound(badPickupSound, badPickupSoundVolume);
            badApples++;
        }
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

        if(highestComboReached < currentCombo)
        {
            highestComboReached = currentCombo;
        }

        UIManager.UpdateCombo(currentCombo);
    }

    public static void MissedGoodApple()
    {
        instance.UpdateGoodApplesMissed();
    }

    public void UpdateGoodApplesMissed()
    {
        ++goodApplesMissed;
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
        yield return Countdown.CountdownLoop();

        StartGame();
    }

    /// <summary>
    /// Begins the game (spawning of apples and game timer).
    /// </summary>
    private void StartGame()
    {
        MusicHandler.StartMusic();

        if (IsInfinite())
        {
            StartCoroutine(InfiniteRoutine());
        }
        else
        {
            StartCoroutine(GameTimer());
        }

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
        float t = GetTimerAmountHelper();

        do
        {
            UIManager.UpdateTimer(t);
            yield return new WaitForEndOfFrame();
            t -= Time.deltaTime;
        }
        while (t > 0);

        yield return EndGame();
    }

    public static int GetTimerAmount()
    {
        return instance.GetTimerAmountHelper();
    }

    private int GetTimerAmountHelper()
    {
        if (currentTimer == 3) return -1;

        return timers[currentTimer];
        //return Mathf.RoundToInt(Mathf.Lerp(minTimerAmount, maxTimerAmount, currentTimer));
    }
    #endregion

    #region Infinite
    private IEnumerator InfiniteRoutine()
    {
        while (badApples+goodApplesMissed < allowedMisses)
        {
            yield return new WaitForFixedUpdate();

            speedMod += Time.fixedDeltaTime * speedModIncreaseRate;
        }

        yield return EndGame();
    }

    public static bool IsInfinite()
    {
        return instance.GetTimerAmountHelper() == -1;
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
    private IEnumerator EndGame()
    {
        StopSpawningApples();

        // Waits for game to fully complete
        yield return WaitForApplesToDrop();
        yield return new WaitForSeconds(timeBeforeEnd);

        // Displays all data
        UIManager.DisplayEndMessage();
        DisplayGameData();
        OutputData();
    }

    private IEnumerator WaitForApplesToDrop()
    {
        var goodApples = GameObject.FindGameObjectsWithTag("Good").ToList();
        var badApples = GameObject.FindGameObjectsWithTag("Bad").ToList();

        while (goodApples.Count != 0 || badApples.Count != 0)
        {
            goodApples.RemoveAll(item => item == null);
            badApples.RemoveAll(item => item == null);


            yield return new WaitForFixedUpdate();
        }
    }

    private void DisplayGameData()
    {
        UIManager.UpdateAppleCount(goodApples, badApples);
        UIManager.UpdateApplesMissedCount(goodApplesMissed);
        UIManager.UpdateHighestCombo(highestComboReached);
        UIManager.UpdateScore(currentPointTotal);
    }

    /// <summary>
    /// Outputs the data from the players session.
    /// </summary>
    private void OutputData()
    {
        // currentPointTotal;
        // goodApples
        // badAPples
        // highestComboReached
        // pointTotal
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
    #endregion
}
