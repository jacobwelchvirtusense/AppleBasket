/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 2/17/2023 4:37:45 PM
 * 
 * Description: Handles the funcitonality of the buttons on the end screen.
*********************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class EndScreenButtonsManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The audiosource for settings events.
    /// </summary>
    private UnityEngine.AudioSource audioSource;

    [Tooltip("The sound made when clicking a button")]
    [SerializeField] private AudioClip clickSound;

    private Button[] buttons;
    private List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();
    private int currentButtonSlot;
    #endregion

    #region Functions
    /// <summary>
    /// Gets components and sets their initial states.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent<UnityEngine.AudioSource>();
        buttons = GetComponentsInChildren<Button>();

        foreach(Button button in buttons) 
        {
            buttonTexts.Add(button.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>());
        }

        UpdateSelectedSettingSlot(0);
    }

    #region Input
    /// <summary>
    /// Gets keyboard inputs for testing purposes.
    /// </summary>
    private void Update()
    {
        KeyboardInput();
    }

    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdateSelectedSettingSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateSelectedSettingSlot(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClickSlot();
        }
    }
    #endregion

    /// <summary>
    /// Updates the setting slot that is currently hovered over.
    /// </summary>
    /// <param name="mod"></param>
    private void UpdateSelectedSettingSlot(int mod)
    {
        buttonTexts[currentButtonSlot].fontStyle = FontStyles.Normal;

        currentButtonSlot = (currentButtonSlot + mod) % buttons.Length;

        if (currentButtonSlot < 0)
        {
            currentButtonSlot = buttons.Length - 1;
        }

        buttonTexts[currentButtonSlot].fontStyle = FontStyles.Underline;
        PlayChangeSound();
    }

    /// <summary>
    /// Performs the click event of the currently selected settings slot.
    /// </summary>
    private void ClickSlot()
    {
        buttons[currentButtonSlot].onClick.Invoke();
        PlayChangeSound();
    }

    /// <summary>
    /// Plays the sound whenever a setting is clicked or hovered over.
    /// </summary>
    public void PlayChangeSound()
    {
        if (audioSource == null || !gameObject.activeInHierarchy) return;

        audioSource.PlayOneShot(clickSound);
    }
    #endregion
}
