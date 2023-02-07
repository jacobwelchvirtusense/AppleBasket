/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 2/1/2023 1:08:22 PM
 * 
 * Description: TODO
*********************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Fields
    private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    #region Saved Data
    private static float timerLerp = 0.5f;
    private static int inputType = 1;
    private static int movementDifficulty = 1;
    private static int gameDifficulty = 1;
    private static bool enableAudio = true;
    private static bool enableTutorial = true;
    #endregion

    #region UI Elements
    #region Sliders
    [Tooltip("The slider for changing the duration of the game")]
    [SerializeField] private Slider timerSlider;

    [Tooltip("The slider for changing the input type of the game")]
    [SerializeField] private Slider inputTypeSlider;

    [Tooltip("The slider for changing the difficulty of the game")]
    [SerializeField] private Slider gameDifficultySlider;

    [Tooltip("The slider for changing the difficulty of the movement in the game")]
    [SerializeField] private Slider movementDifficultySlider;
    #endregion

    #region Toggles
    [Tooltip("The toggle for audio of the game")]
    [SerializeField] private Toggle audioToggle;

    [Tooltip("The toggle for tutorials of the game")]
    [SerializeField] private Toggle tutorialToggle;
    #endregion

    #region Texts
    [Tooltip("The text for showing the current timer of the game")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Tooltip("The text for showing the current difficulty of the game")]
    [SerializeField] private TextMeshProUGUI gameDifficultyText;

    [Tooltip("The text for showing the current movement difficulty of the game")]
    [SerializeField] private TextMeshProUGUI movementDifficultyText;

    [Tooltip("The text for showing the current input type of the game")]
    [SerializeField] private TextMeshProUGUI inputTypeText;
    #endregion
    #endregion
    #endregion

    #region Functions
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeSettings();
    }

    private void GetSettingsFromPipeline()
    {
        #region Initialize Sliders
        inputTypeSlider.value = inputType;
        gameDifficultySlider.value = gameDifficulty;
        movementDifficultySlider.value = movementDifficulty;
        timerSlider.value = timerLerp;

        audioToggle.isOn = true;
        tutorialToggle.isOn = true;
        #endregion
    }

    private void InitializeSettings()
    {
        #region Initialize Sliders
        inputTypeSlider.value = inputType;
        gameDifficultySlider.value = gameDifficulty;
        movementDifficultySlider.value = movementDifficulty;
        timerSlider.value = timerLerp;

        audioToggle.isOn = enableAudio;
        tutorialToggle.isOn = enableTutorial;
        #endregion

        #region Initialize Settings
        UpdateTimer(timerLerp);
        SetGameDifficulty(gameDifficulty);
        SetInputType(inputType);
        SetMovementDifficulty(movementDifficulty);
        EnableAudio(audioToggle.isOn);
        EnableTutorial(tutorialToggle.isOn);
        #endregion
    }

    public void PlayChangeSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void DisableSettingsMenu()
    {
        gameObject.SetActive(false);
    }

    public void UpdateTimer(float timerLerp)
    {
        SettingsManager.timerLerp = timerLerp;
        GameController.UpdateTimer(timerLerp);

        timerText.text = UIManager.GetTimerValue(GameController.GetTimerAmount());

        PlayChangeSound();
    }

    public void SetInputType(float inputType)
    {
        SettingsManager.inputType = (int)inputType;
        BasketMovement.SetMovementType(SettingsManager.inputType);

        inputTypeText.text = ((BasketMovement.MovementType) SettingsManager.inputType).ToString();

        PlayChangeSound();
    }

    public void SetMovementDifficulty(float difficulty)
    {
        movementDifficulty = (int)difficulty;
        BasketMovement.SetMovementDifficulty(movementDifficulty);

        movementDifficultyText.text = ((BasketMovement.MovementDifficulty) movementDifficulty).ToString();

        PlayChangeSound();
    }

    public void SetGameDifficulty(float difficulty)
    {
        gameDifficulty = (int)difficulty;
        AppleSpawner.UpdateGameDifficulty(gameDifficulty);

        gameDifficultyText.text = ((AppleSpawner.AppleSpawnRateDifficulty) gameDifficulty).ToString();

        PlayChangeSound();
    }

    public void EnableAudio(bool shouldEnable)
    {
        enableAudio = shouldEnable;
        AudioListener.volume = shouldEnable ? 1 : 0;

        PlayChangeSound();
    }

    public void EnableTutorial(bool shouldEnable)
    {
        enableTutorial = shouldEnable;
        print("Should Enable Tutorial: " + shouldEnable);

        PlayChangeSound();
    }
    #endregion
}
