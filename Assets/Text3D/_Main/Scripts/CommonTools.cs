using UnityEngine;

public static class CommonTools
{
    /// <summary>
    /// Generates a random Vector3 with values between -1 and 1 for each component
    /// </summary>
    /// <returns>A random Vector3</returns>
    public static Vector3 GetRandomVector3()
    {
        return new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );
    }
    
    /// <summary>
    /// Generates a random Vector3 with values between min and max for each component
    /// </summary>
    /// <param name="min">Minimum value for each component</param>
    /// <param name="max">Maximum value for each component</param>
    /// <returns>A random Vector3</returns>
    public static Vector3 GetRandomVector3(float min, float max)
    {
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max)
        );
    }
}
