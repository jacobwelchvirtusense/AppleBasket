/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/17/2023 3:09:53 PM
 * 
 * Description: TODO
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

    private float speed = 0.4f;

    private SpriteRenderer spriteRenderer;

    [Tooltip("The cloud images")]
    [SerializeField] private Sprite[] clouds = new Sprite[0];
    #endregion

    #region Functions
    // Start is called before the first frame update
    private void Awake()
    {
        InitializeComponents();
        InitializeCloud();
    }

    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void InitializeCloud()
    {
        speed = CustomRandom.RandomGeneration(minSpeed, maxSpeed);

        spriteRenderer.sprite = clouds[Random.Range(0, clouds.Length)];
    }

    private void ResetPosition()
    {
        var pos = transform.position;
        pos.x = screenBounds;
        transform.position = pos;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdatePostion();
        CheckScreenBounds();
    }

    private void UpdatePostion()
    {
        var pos = transform.position;
        pos.x -= speed * Time.fixedDeltaTime;
        transform.position = pos;
    }

    private void CheckScreenBounds()
    {
        if(transform.position.x < -screenBounds)
        {
            InitializeCloud();
            ResetPosition();
        }
    }
    #endregion
}
