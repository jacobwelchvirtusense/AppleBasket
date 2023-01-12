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

public class UIManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The instance of the UI manager in the scene.
    /// </summary>
    private static UIManager instance;

    // UI objects
    private static TextMeshProUGUI countDown;
    private static TextMeshProUGUI score;
    private static TextMeshProUGUI timerUI;
    private static TextMeshProUGUI endMessage;
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
        foreach (Transform t in transform)
        {
            switch (t.name)
            {
                case "Count Down":
                    countDown = t.gameObject.GetComponent<TextMeshProUGUI>();
                    UpdateCountdown(0);
                    break;
                case "Score":
                    score = t.gameObject.GetComponent<TextMeshProUGUI>();
                    break;
                case "Timer":
                    timerUI = t.gameObject.GetComponent<TextMeshProUGUI>();
                    UpdateTimer(0);
                    break;
                case "End Message":
                    endMessage = t.gameObject.GetComponent<TextMeshProUGUI>();
                    endMessage.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region UI Updates
    /// <summary>
    /// Sets the new count in the countdown.
    /// </summary>
    /// <param name="newCount">The current count.</param>
    public static void UpdateCountdown(int newCount)
    {
        if (InstanceDoesntExist() || IsntValid(countDown)) return;

        // Updates the countdown UI 
        countDown.text = newCount.ToString();
        countDown.gameObject.SetActive(newCount != 0);
    }

    /// <summary>
    /// Updates the dipslay of the current score.
    /// </summary>
    /// <param name="newScore">The current score the player has.</param>
    public static void UpdateScore(int newScore)
    {
        if (InstanceDoesntExist() || IsntValid(score)) return;

        // Updates the score UI 
        score.text = newScore.ToString();
    }

    /// <summary>
    /// Updates the timer to its current time.
    /// </summary>
    /// <param name="newTime">The current time left of the timer.</param>
    public static void UpdateTimer(int newTime)
    {
        if (InstanceDoesntExist() || IsntValid(timerUI)) return;

        // Updates the timer UI 
        timerUI.text = "Time left: " + newTime.ToString();
        timerUI.gameObject.SetActive(newTime != 0);
    }

    /// <summary>
    /// Displays the message that should appear at the end of the game.
    /// </summary>
    public static void DisplayEndMessage()
    {
        if (InstanceDoesntExist() || IsntValid(timerUI)) return;

        endMessage.gameObject.SetActive(true);
    }
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
