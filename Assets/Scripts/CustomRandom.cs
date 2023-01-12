/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/11/2023 9:11:24 AM
 * 
 * Description: My own script with custom random generations to
 *              make random generation easier for development 
 *              between projects.
*********************************/
using UnityEngine;

public class CustomRandom : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The types of generation methods for generating random numbers.
    /// RANDOM: gives a random value between the min and max.
    /// NORMAL: gives a random value on a triple sigma normal distribution
    /// </summary>
    public enum GenerationType { RANDOM, NORMALDISTRIBUTION }
    #endregion

    #region Functions
    /// <summary>
    /// Returns 1 or -1 to give a random negative number.
    /// </summary>
    /// <returns></returns>
    public static int RandomNegative()
    {
        return (Random.Range(0, 2) * 2 - 1);
    }

    #region Random Min Max
    /// <summary>
    /// Generates a random value between the min and max given a specific generation method.
    /// </summary>
    /// <param name="min">The minimum value for the random number.</param>
    /// <param name="max">The maximum value for the random number.</param>
    /// <param name="generationType">The type of generation for the random value.</param>
    /// <returns></returns>
    public static float RandomGeneration(float min, float max, GenerationType generationType = GenerationType.RANDOM)
    {
        switch (generationType)
        {
            case GenerationType.NORMALDISTRIBUTION:
                return NormalDistribution(min, max);

            default:
            case GenerationType.RANDOM:
                return RandomLerp(min, max);
        }
    }

    /// <summary>
    /// Randomly lerps between the min and max value.
    /// </summary>
    /// <param name="min">The minimum value to be selected.</param>
    /// <param name="max">The maximum value to be selected.</param>
    /// <returns></returns>
    private static float RandomLerp(float min, float max)
    {
        return Mathf.Lerp(min, max, Random.Range(0.0f, 1.0f));
    }

    /// <summary>
    /// Generates the normal distribution value to be used.
    /// </summary>
    /// <param name="minValue">The min value that can be found.</param>
    /// <param name="maxValue">The max value that can be found.</param>
    /// <returns></returns>
    private static float NormalDistribution(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
    #endregion
    #endregion
}
