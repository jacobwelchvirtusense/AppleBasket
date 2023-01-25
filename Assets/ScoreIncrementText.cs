/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/17/2023 3:33:19 PM
 * 
 * Description: TODO
*********************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class ScoreIncrementText : MonoBehaviour
{
    #region Fields
    [Range(0.0f, 5.0f)]
    [Tooltip("The y position offset to spawn this")]
    [SerializeField] private float spawnYOffset = 2.0f;

    [Range(0.0f, 5.0f)]
    [Tooltip("The length of time that the text lasts for")]
    [SerializeField] private float duration = 2.0f;

    [Range(0.0f, 5.0f)]
    [Tooltip("The speed that the text moves upward")]
    [SerializeField] private float speed = 2.0f;

    private float currentDuration;
    private Color clear;

    private TextMeshPro text;
    #endregion

    #region Functions
    // Start is called before the first frame update
    private void Awake()
    {
        currentDuration = duration;
        clear = Color.white;
        clear.a = 0;

        var pos = transform.position;
        pos.y += spawnYOffset;
        transform.position = pos;

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void InitializeScore(int score)
    {
        string scoreString = "";

        if(score > 0)
        {
            scoreString = "+";
        }

        scoreString += score.ToString();
        text.text = scoreString; 
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        currentDuration -= Time.fixedDeltaTime;

        var pos = transform.position;
        pos.y += speed * Time.fixedDeltaTime;
        transform.position = pos;

        text.color = Color.Lerp(Color.white, clear, Mathf.InverseLerp(duration, 0.0f, currentDuration));

        if(currentDuration < 0.0f)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
