using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapBorder : Graphic
{
    
    [Min(1)]
    public float nodeSize;
    [Range(1, 5)]
    public int nPaths = 1;
    [Range(1, 15)]
    public int rows = 5;
    [Range(1, 15)]
    public int columns = 10;

    private Vector2[] _grid;
    private List<List<Vector2>> paths = new List<List<Vector2>>();
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        _grid = new Vector2[rows * columns];
        
        // Create 4 points of the quad
        Bounds bounds = new Bounds();
        RectTransform _tr = GetComponent<RectTransform>();
        bounds.center = _tr.position;
        bounds.min = new Vector3(-(_tr.rect.width / 2f), -(_tr.rect.height / 2f), 0);
        bounds.min += new Vector3(nodeSize, nodeSize, 0);
        bounds.max = new Vector3(_tr.rect.width / 2f, _tr.rect.height / 2f, 0);
        bounds.max -= new Vector3(nodeSize, nodeSize, 0);
        
        Vector2 v0 = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 v1 = new Vector2(bounds.min.x, bounds.max.y);
        Vector2 v2 = new Vector2(bounds.max.x, bounds.max.y);
        Vector2 v3 = new Vector2(bounds.max.x, bounds.min.y);

        int index = vh.currentVertCount;
        Debug.Log($"Index: {index}");

        // Add vertices
        vh.AddVert(v0, color, Vector2.zero);
        vh.AddVert(v1, color, Vector2.zero);
        vh.AddVert(v2, color, Vector2.zero);
        vh.AddVert(v3, color, Vector2.zero);

        // Add two triangles
        vh.AddTriangle(index + 0, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index + 0);

        float width = bounds.max.x - bounds.min.x, height = bounds.max.y - bounds.min.y;
        
        float spacingX = width / columns, spacingY = height / rows;

        for (int r = 0; r < rows; r++)
        {
           
            for (int c = 0; c < columns; c++)
            {
                int idx = r * columns + c;
                Vector2 pos = new Vector2(bounds.min.x + spacingX * c + spacingX / 2f, bounds.min.y + spacingY * r + spacingY / 2f);
                _grid[idx] = pos;
            }
        }

       
        for (int i = 0; i < nPaths; i++)
        {
            List<Vector2> path = new List<Vector2>();
            int prevNode = -1;
            for (int r = 0; r < rows; r++)
            {
                if (r + 1 >= rows) break;
                int node = 0;
                if (r == 0)
                {
                    node = Random.Range(0, columns);
                    path.Add(_grid[r * columns + node]);
                }
                if(prevNode != -1)
                {
                    node = prevNode;
                }
                int idx = r * columns + node;
                List<int> neighbours = new List<int>();
                List<int> neighboursRawIdx = new List<int>();
                neighbours.Add((r + 1) * columns + node);
                neighboursRawIdx.Add(node);
                if (node - 1 >= 0)
                {
                    neighbours.Add((r + 1) * columns + node - 1);
                    neighboursRawIdx.Add(node-1);
                }

                if (node + 1 < rows)
                {
                    neighbours.Add((r + 1) * columns + node + 1);
                    neighboursRawIdx.Add(node+1);
                }
                
                int selection = Random.Range(0, neighbours.Count);
                AddLine(vh, _grid[idx], _grid[neighbours[selection]], Color.white);
                prevNode = neighboursRawIdx[selection];
                path.Add(_grid[neighbours[selection]]);
            }
            paths.Add(path);
        }

        foreach (List<Vector2> path in paths)
        {
            foreach (Vector2 p in path)
            {
                AddPoint(vh, p, 25f, Color.red);
            }
        }
    }

    private void AddLine(VertexHelper vh, Vector2 start, Vector2 end, Color color, float thickness = 10f)
    {
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

    private void AddPoint(VertexHelper vh, Vector2 center, float width, Color color)
    {
        int index = vh.currentVertCount;
        
        Vector2 pv0 = new Vector2(center.x - width, center.y - width);
        Vector2 pv1 = new Vector2(center.x - width, center.y + width);
        Vector2 pv2 = new Vector2(center.x + width, center.y + width);
        Vector2 pv3 = new Vector2(center.x + width, center.y - width);
        
        vh.AddVert(pv0, color, Vector2.zero);
        vh.AddVert(pv1, color, Vector2.zero);
        vh.AddVert(pv2, color, Vector2.zero);
        vh.AddVert(pv3, color, Vector2.zero);
        
        vh.AddTriangle(index + 0, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index + 0);
    }
}
