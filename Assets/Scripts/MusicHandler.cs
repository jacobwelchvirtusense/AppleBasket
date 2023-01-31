/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 1/25/2023 1:16:58 PM
 * 
 * Description: TODO
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    #region Fields
    private static AudioSource audioSource;
    #endregion

    #region Functions
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Starts this scenes music.
    /// </summary>
    public static void StartMusic()
    {
        if(audioSource != null) audioSource.Play();
    }

    /// <summary>
    /// Stops this scenes music.
    /// </summary>
    public static void StopMusic()
    {
        if (audioSource != null) audioSource.Stop();
    }
    #endregion
}
