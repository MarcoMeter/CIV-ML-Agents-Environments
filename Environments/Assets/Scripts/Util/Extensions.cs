using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Randomly displaced the given <see cref="Vector3"/> by the given range
    /// </summary>
    /// <param name="range">Range by which the vector should be max displaced</param>
    /// <returns>Displaced vector</returns>
    public static Vector3 RandomDisplace(this Vector3 initial, Vector3 range) => new Vector3(
            initial.x + Random.Range(-range.x, range.x),
            initial.y + Random.Range(-range.y, range.y),
            initial.z + Random.Range(-range.z, range.z));

    public static Vector3 RandomDisplace(this Vector3 initial, float range) => new Vector3(
            initial.x + Random.Range(-range, range),
            initial.y + Random.Range(-range, range),
            initial.z + Random.Range(-range, range));

    public static Vector3 RandomDisplace2D(this Vector3 initial, Vector3 range) => new Vector3(
            initial.x + Random.Range(-range.x, range.x),
            0f,
            initial.z + Random.Range(-range.z, range.z));

    public static Vector3 RandomDisplace2D(this Vector3 initial, float range) => new Vector3(
            initial.x + Random.Range(-range, range),
            0f,
            initial.z + Random.Range(-range, range));
}

