/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/17/2023 3:09:53 PM
 * 
 * Description: Handles movement of clouds in the environment.
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    #region Fields
    [Range(0.0f, 10.0f)]
    [Tooltip("The minimum speed the cloud move")]
    [SerializeField] private float minSpeed = 0.4f;

    [Range(0.0f, 10.0f)]
    [Tooltip("The maximum speed the cloud moves")]
    [SerializeField] private float maxSpeed = 1.5f;

    [Range(0.0f, 30.0f)]
    [Tooltip("The bounds of the screen that the cloud must stay in")]
    [SerializeField] private float screenBounds = 10.0f;

    [Tooltip("The cloud images")]
    [SerializeField] private Sprite[] clouds = new Sprite[0];

    /// <summary>
    /// The current speed of the cloud.
    /// </summary>
    private float speed = 0.4f;

    /// <summary>
    /// The sprite renderer of the clouds.
    /// </summary>
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Handles all initialization events at the beginning of the scene.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
        InitializeCloud();
    }

    /// <summary>
    /// Gets necessary components.
    /// </summary>
    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Sets the initial speed and sprite of the cloud.
    /// </summary>
    private void InitializeCloud()
    {
        speed = CustomRandom.RandomGeneration(minSpeed, maxSpeed);

        spriteRenderer.sprite = clouds[Random.Range(0, clouds.Length)];
    }
    #endregion

    #region Movement
    /// <summary>
    /// Calls all updates a set amount of times per second.
    /// </summary>
    private void FixedUpdate()
    {
        UpdatePostion();
        CheckScreenBounds();
    }

    /// <summary>
    /// Updates the current position of the cloud to the left.
    /// </summary>
    private void UpdatePostion()
    {
        var pos = transform.position;
        pos.x -= speed * Time.fixedDeltaTime;
        transform.position = pos;
    }

    /// <summary>
    /// Moves the cloud back to the start once it reaches off screen.
    /// </summary>
    private void CheckScreenBounds()
    {
        if(transform.position.x < -screenBounds)
        {
            InitializeCloud();
            ResetPosition();
        }
    }

    /// <summary>
    /// Resets the clouds position back to the right side of the screen.
    /// </summary>
    private void ResetPosition()
    {
        var pos = transform.position;
        pos.x = screenBounds;
        transform.position = pos;
    }
    #endregion
    #endregion
}
