using UnityEngine;

public class Utils
{
    public static Vector2 GetPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x), y = Random.Range(bounds.min.y, bounds.max.y);
        Debug.Log($"Point: {x},{y}");
        return new Vector2(x,y);
    }
}