/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:22:22 AM
 * 
 * Description: A base functionality for all apples. Includes
 *              scoring, movement, and collision with baskets.
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

[RequireComponent(typeof(Rigidbody2D))]
public class Apple : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The rigidbody of the apple.
    /// </summary>
    private Rigidbody2D rigidbody2d;

    [Range(-10000, 10000)]
    [Tooltip("The points gained for picking up this type of apple")]
    [SerializeField] private int points = 50;

    [Range(-10.0f, 0.0f)]
    [Tooltip("The height below the screen to remove the apple")]
    [SerializeField] private float offscreenHeight = -6.0f;

    #region Speed
    [Header("Speed")]
    [Tooltip("The type of random generation for the fall speed")]
    [SerializeField] private CustomRandom.GenerationType fallSpeedGenerationType = CustomRandom.GenerationType.RANDOM;

    [Range(0.0f, 50.0f)]
    [Tooltip("The minimum speed the apple will fall")]
    [SerializeField] private float minFallSpeed = 1.0f;

    [Range(0.0f, 50.0f)]
    [Tooltip("The maximum speed the apple will fall")]
    [SerializeField] private float maxFallSpeed = 1.0f;

    // Squared values are compared to velocity squared maginitude to reduce sqrt calls
    private float minFallSpeedSquared;
    private float maxFallSpeedSquared;

    /// <summary>
    /// The current terminal velocity of this apple.
    /// </summary>
    private float currentMaxSpeedSquared = 5.0f;
    private float currentMaxSpeed = 5.0f;

    private float speedMod = 0.0f;

    [Range(0.0f, 50.0f)]
    [Tooltip("The rate of speed increase for the apple")]
    [SerializeField] private float accelerationSpeed = 9.8f;

    /// <summary>
    /// Holds true if this apple should be locked in place.
    /// </summary>
    [HideInInspector] public bool IsLockedInPlace = false;
    #endregion

    #region Rotation
    [Header("Rotation")]
    [Tooltip("The type of random generation for the rotation speed")]
    [SerializeField] private CustomRandom.GenerationType angularVelocityGenerationType = CustomRandom.GenerationType.RANDOM;

    [Range(0.0f, 1000.0f)]
    [Tooltip("The minimum angular velocity the apple will fall with")]
    [SerializeField] private float minAngularVelocity = 1.0f;

    [Range(0.0f, 1000.0f)]
    [Tooltip("The maximum angular velocity the apple will fall with")]
    [SerializeField] private float maxAngularVelocity = 1.0f;
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Calls for the initializations of various components and fields.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Gets necessary components of the apple.
    /// </summary>
    private void InitializeComponents()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void InitializeSpeedMod(float speedMod)
    {
        this.speedMod = speedMod;

        InitializeRotation();
        InitializeSpeeds();
    }

    /// <summary>
    /// Initializes the rotation and angular velocity of the apple.
    /// </summary>
    private void InitializeRotation()
    {
        // Sets starting rotation
        var newRot = transform.rotation.eulerAngles;
        newRot.z = CustomRandom.RandomGeneration(0.0f, 360.0f);
        transform.rotation = Quaternion.Euler(newRot);

        // Sets random angular velocity
        rigidbody2d.angularVelocity = CustomRandom.RandomGeneration(minAngularVelocity, maxAngularVelocity, angularVelocityGenerationType) * CustomRandom.RandomNegative();
    }

    /// <summary>
    /// Calculates the squared speed values.
    /// </summary>
    private void InitializeSpeeds()
    {
        minFallSpeedSquared = minFallSpeed * minFallSpeed;
        maxFallSpeedSquared = maxFallSpeed * maxFallSpeed;
        currentMaxSpeedSquared = CustomRandom.RandomGeneration(minFallSpeedSquared, maxFallSpeedSquared, fallSpeedGenerationType) * speedMod * BasketMovement.SpeedGameMod();
        currentMaxSpeed = Mathf.Sqrt(currentMaxSpeedSquared);

        accelerationSpeed *= speedMod;
    }
    #endregion

    /// <summary>
    /// Updates the speed of the apple.
    /// </summary>
    private void FixedUpdate()
    {
        if (IsLockedInPlace)
        {
            return;
        }

        // Removes objects when they fall too low
        if (transform.position.y < offscreenHeight)
        {
            if(points > 0)
            {
                ResetCombo();
                MissedGoodApple();
            }

            Destroy(gameObject);
        }

        // Increases Speed
        else if (rigidbody2d.velocity.sqrMagnitude < currentMaxSpeedSquared)
        {
            rigidbody2d.AddForce(Vector2.down * accelerationSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        // Clamps Speed
        else
        {
            rigidbody2d.velocity = rigidbody2d.velocity.normalized * currentMaxSpeed * InfiniteSpeedMod;
        }
    }

    #region Basket Collisions
    /// <summary>
    /// Checks if the apple has entered the basket.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Basket"))
        {
            EnterBasket();
        }
    }

    /// <summary>
    /// Handles the entry of the apple into the basket.
    /// </summary>
    protected virtual void EnterBasket()
    {
        UpdateScore(points, transform.position);
     
        if(points > 0)
        {
            IncreaseCombo();
        }
        else
        {
            ResetCombo();
        }

        Destroy(gameObject);
    }
    #endregion
    #endregion
}
