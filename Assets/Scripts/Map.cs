using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Map : MonoBehaviour
{
    public GameObject point;
    public UILine line;
    public float paths = 2;
    public int rows = 1;
    public int columns = 1;
    
    private RectTransform _tr;
    private Bounds _bounds;

    private Transform _prevPoint = null;
    
    private void Awake()
    {
        _bounds = GetMapArea();
        Debug.Log($"Bounds: {_bounds} | {_bounds.min} {_bounds.max}");
        
        float width = _bounds.max.x - _bounds.min.x, height = _bounds.max.y - _bounds.min.y;
        float spacingX = width / columns, spacingY = height / rows;
        
        for (int r = 0; r < rows; r++)
        {
           
            for (int c = 0; c < columns; c++)
            {
                int idx = r * columns + c;
                Vector2 pos = new Vector2(_bounds.min.x + spacingX * c + spacingX / 2f, _bounds.min.y + spacingY * r + spacingY / 2f);
                //_grid[idx] = pos;
                GameObject obj = Instantiate(point, transform);
                obj.transform.localPosition = pos;  
            }
        }
        
    }

    private void AddLine(Vector2 start, Vector2 end)
    {
        UILine obj = Instantiate(line, transform);
        obj.start = start;
        obj.end = end;
    }

    private Bounds GetMapArea()
    {
        Bounds bounds = new Bounds();
        _tr = GetComponent<RectTransform>();
        bounds.center = _tr.position;
        
        // Calculate bounds with offset
        RectTransform point = this.point.GetComponent<RectTransform>();
        
        bounds.min = new Vector3(-(_tr.rect.width / 2f), -(_tr.rect.height / 2f), 0);
        bounds.min += new Vector3(point.rect.width, point.rect.height, 0);
        bounds.max = new Vector3(_tr.rect.width / 2f, _tr.rect.height / 2f, 0);
        bounds.max -= new Vector3(point.rect.width, point.rect.height, 0);

        return bounds;
    }
}
