/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:27:21 AM
 * 
 * Description: Handles the spawning of apples over variable rates.
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static InspectorValues;

public class AppleSpawner : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The scene instance of the apple spawner.
    /// </summary>
    private static AppleSpawner appleSpawner;

    [Range(0.0f, 20.0f)]
    [Tooltip("The max dist from the center to spawn apples")]
    [SerializeField] private float maxSpawnDist = 8.0f;

    #region Spawn Rates
    [Header("Spawn rates")]
    [Range(0.0f, 1.0f)]
    [Tooltip("The rate that good apples should be spawning")]
    [SerializeField] private float goodAppleRate = 0.8f;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Tooltip("The type of random generation for the time between apple spawns")]
    [SerializeField] private CustomRandom.GenerationType timeBetweenSpawnsGenerationType = CustomRandom.GenerationType.RANDOM;

    [Range(0.0f, 10.0f)]
    [Tooltip("The minimum time in seconds between apple spawns")]
    [SerializeField] private float minTimeBetweenSpawns = 0.4f;

    [Range(0.0f, 10.0f)]
    [Tooltip("The maximum time in seconds between apple spawns")]
    [SerializeField] private float maxTimeBetweenSpawns = 1.0f;
    #endregion

    #region Apples
    [Header("Apples")]
    [Tooltip("The prefab of the normal good apple")]
    [SerializeField] private GameObject goodApple;

    [Tooltip("The prefab of the normal bad apple")]
    [SerializeField] private GameObject badApple;
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Gets the scene instance of the apple spawner.
    /// </summary>
    private void Awake()
    {
        appleSpawner = this;
    }
    #endregion

    #region Spawn Routine
    /// <summary>
    /// Calls to start spawning apples
    /// </summary>
    public static void StartSpawningApples()
    {
        if (appleSpawner == null) return;

        appleSpawner.StartCoroutine(appleSpawner.DropApplesRoutine());
    }

    /// <summary>
    /// Calls to stop spawning apples
    /// </summary>
    public static void StopSpawningApples()
    {
        appleSpawner.StopAllCoroutines();
    }

    /// <summary>
    /// The routine for dropping apples over variable amounts of time.
    /// </summary>
    /// <returns></returns>
    public IEnumerator DropApplesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CustomRandom.RandomGeneration(minTimeBetweenSpawns, maxTimeBetweenSpawns, timeBetweenSpawnsGenerationType));
            SpawnApple();
        }
    }
    #endregion

    #region Spawning Apple
    /// <summary>
    /// Selects the apple to be spawned.
    /// </summary>
    /// <returns>The apple prefab to instantiate.</returns>
    private GameObject SelectApple()
    {
        if (Random.Range(0.0f, 1.0f) < goodAppleRate) return goodApple;
        else return badApple;

    }

    /// <summary>
    /// Spawns the apple in the scene at a random X value.
    /// </summary>
    private void SpawnApple()
    {
        var pos = transform.position;
        pos.x += CustomRandom.RandomGeneration(-maxSpawnDist, maxSpawnDist);

        Instantiate(SelectApple(), pos, Quaternion.identity);
    }
    #endregion
    #endregion
}
