using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILine : Graphic
{
    public Vector2 start;
    public Vector2 end;

    public float thickness = 10f;
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x); // Perpendicular vector
        Vector2 offset = normal * (thickness / 2f);

        // Create 4 points of the quad
        Vector2 v0 = start - offset;
        Vector2 v1 = start + offset;
        Vector2 v2 = end + offset;
        Vector2 v3 = end - offset;

        int index = vh.currentVertCount;

        // Add vertices
        vh.AddVert(v0, color, Vector2.zero);
        vh.AddVert(v1, color, Vector2.zero);
        vh.AddVert(v2, color, Vector2.zero);
        vh.AddVert(v3, color, Vector2.zero);

        // Add two triangles
        vh.AddTriangle(index + 0, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index + 0);
    }
}
