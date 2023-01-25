/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:25:04 AM
 * 
 * Description: Handles the functionality of all
 *              UI assets.
*********************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The instance of the UI manager in the scene.
    /// </summary>
    private static UIManager instance;

    private static int timerStartingAmount = 0;

    // UI objects
    [SerializeField] private TextMeshProUGUI countDown;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI timerUI;
    [SerializeField] private TextMeshProUGUI combo;
    [SerializeField] private TextMeshProUGUI endMessage;

    private static TextMeshProUGUI CountDown;
    private static TextMeshProUGUI Score;
    private static TextMeshProUGUI TimerUI;
    private static TextMeshProUGUI Combo;
    private static TextMeshProUGUI EndMessage;

    // Images
    [SerializeField] private Image timerBar1;
    [SerializeField] private Image timerBar2;

    private static Image TimerBar1;
    private static Image TimerBar2;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Initializes all aspects of the UI manager.
    /// </summary>
    private void Awake()
    {
        instance = this;
        GetUIReferences();
    }

    /// <summary>
    /// Gets references to all of the UI objects.
    /// </summary>
    private void GetUIReferences()
    {
        #region Countdown
        CountDown = countDown;
        UpdateCountdown(0);
        #endregion

        #region Score
        Score = score;
        #endregion

        #region Timer
        TimerUI = timerUI;
        TimerBar1 = timerBar1;
        TimerBar2 = timerBar2;
        #endregion

        #region End Message
        EndMessage = endMessage;
        EndMessage.gameObject.SetActive(false);
        #endregion

        #region Combo
        Combo = combo;
        UpdateCombo(0);
        #endregion
    }
    #endregion

    #region UI Updates
    /// <summary>
    /// Sets the new count in the countdown.
    /// </summary>
    /// <param name="newCount">The current count.</param>
    public static void UpdateCountdown(int newCount)
    {
        if (InstanceDoesntExist() || IsntValid(CountDown)) return;

        // Updates the countdown UI 
        //CountDown.text = newCount.ToString();

        CountDown.gameObject.SetActive(newCount != 0);
    }

    /// <summary>
    /// Updates the dipslay of the current score.
    /// </summary>
    /// <param name="newScore">The current score the player has.</param>
    public static void UpdateScore(int newScore)
    {
        if (InstanceDoesntExist() || IsntValid(Score)) return;

        // Updates the score UI 
        Score.text = newScore.ToString();
    }

    #region Timer
    public static void InitializeTimer(int startingTime)
    {
        timerStartingAmount = startingTime;
        UpdateTimer(startingTime);
    }

    /// <summary>
    /// Updates the timer to its current time.
    /// </summary>
    /// <param name="newTime">The current time left of the timer.</param>
    public static void UpdateTimer(float newTime)
    {
        if (InstanceDoesntExist() || IsntValid(TimerUI)) return;

        var seconds = (int)newTime;
        var minutes = seconds / 60;
        var leftOverSeconds = (seconds - (minutes * 60));
        string secondsDisplayed = "";

        if(leftOverSeconds < 10) secondsDisplayed += "0";
        secondsDisplayed += leftOverSeconds;

        // Updates the timer UI 
        TimerUI.text = minutes.ToString() + ":" + secondsDisplayed;
        TimerUI.gameObject.SetActive(newTime != 0);

        UpdateTimerBars(newTime);
    }

    private static void UpdateTimerBars(float newTime)
    {
        if (IsntValid(TimerBar1) || IsntValid(TimerBar2)) return;

        TimerBar1.fillAmount = newTime / timerStartingAmount;
        TimerBar2.fillAmount = newTime / timerStartingAmount;
    }

    public IEnumerator UpdateTimerBarsRoutine(int newTime)
    {
        var t = (float)newTime;
        var oneLess = newTime - 1;

        do
        {
            t -= Time.deltaTime;

            timerBar1.fillAmount = t / timerStartingAmount;
            timerBar2.fillAmount = t / timerStartingAmount;

            yield return new WaitForEndOfFrame();
        }
        while (t >= oneLess);
    }
    #endregion

    #region Combo
    public static void UpdateCombo(int newCombo)
    {
        if (InstanceDoesntExist() || IsntValid(Combo)) return;

        Combo.text = "x" + newCombo.ToString();
    }
    #endregion

    #region Display End Message
    /// <summary>
    /// Displays the message that should appear at the end of the game.
    /// </summary>
    public static void DisplayEndMessage()
    {
        if (InstanceDoesntExist() || IsntValid(EndMessage)) return;

        EndMessage.gameObject.SetActive(true);
    }
    #endregion
    #endregion

    #region Null Checks
    private static bool IsntValid(Component uiObject)
    {
        return uiObject == null;
    }

    private static bool InstanceDoesntExist()
    {
        return instance == null;
    }
    #endregion
    #endregion
}
