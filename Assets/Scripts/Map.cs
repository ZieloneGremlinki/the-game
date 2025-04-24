using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    public GameObject player;
    public MapNode point;
    public UILine line;
    public float nPaths = 2;
    public int rows = 1;
    public int columns = 1;

    private GameObject plr;
    
    private RectTransform _tr;
    private Bounds _bounds;

    private Transform _prevPoint = null;

    struct Node
    {
        public Vector2 mapPos;
        public int gridPos;
        public MapNode mapNode;
    }
    
    private void Awake()
    {
        plr = Instantiate(player, transform);
        plr.SetActive(false);
        _bounds = GetMapArea();
        Debug.Log($"Bounds: {_bounds} | {_bounds.min} {_bounds.max}");
        
        float width = _bounds.max.x - _bounds.min.x, height = _bounds.max.y - _bounds.min.y;
        float spacingX = width / columns, spacingY = height / rows;
        
        Vector2[] grid = new Vector2[rows*columns];
        for (int r = 0; r < rows; r++)
        {
           
            for (int c = 0; c < columns; c++)
            {
                int idx = r * columns + c;
                Vector2 pos = new Vector2(_bounds.min.x + spacingX * c + spacingX / 2f, _bounds.min.y + spacingY * r + spacingY / 2f);
                grid[idx] = pos;
            }
        }

        List<List<Node>> paths = new List<List<Node>>();
        List<Node> rnodes = new List<Node>();
        List<Vector2> nodes = new List<Vector2>();
        
        for (int i = 0; i < nPaths; i++)
        {
            List<Node> path = new List<Node>();
            int prevNode = -1;
            for (int r = 0; r < rows; r++)
            {
                if (r + 1 >= rows) break;
                int node = 0;
                if (r == 0)
                {
                    node = Random.Range(0, columns);
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
                prevNode = neighboursRawIdx[selection];
                Node n = new Node();
                GameObject line = AddLine(grid[idx], grid[neighbours[selection]], Color.white);
                
                if (!nodes.Contains(grid[idx]))
                {
                    nodes.Add(grid[idx]);
                    MapNode m = AddPoint(grid[idx]);
                    m.AddEvent(() =>
                    {
                        plr.transform.localPosition = grid[neighbours[selection]];
                        m.Interactable = false;
                        Destroy(line);
                    });
                    n.gridPos = idx;
                    n.mapPos = grid[idx];
                    n.mapNode = m;
                    path.Add(n);
                    rnodes.Add(n);
                }
                else
                {
                    MapNode m = rnodes.Find(x => x.gridPos == idx).mapNode;
                    m.AddEvent(() =>
                    {
                        plr.transform.localPosition = grid[neighbours[selection]];
                        m.Interactable = false;
                        Destroy(line);
                    });
                }
                if (!nodes.Contains(grid[neighbours[selection]]))
                {
                    nodes.Add(grid[neighbours[selection]]);
                    MapNode m = AddPoint(grid[neighbours[selection]]);
                    n.gridPos = neighbours[selection];
                    n.mapPos = grid[neighbours[selection]];
                    n.mapNode = m;
                    path.Add(n);
                }
            }
            paths.Add(path);
        }
        
        int chosenPath = Random.Range(0, paths.Count);
        Node startNode = paths[chosenPath][0];
        startNode.mapNode.NodeType = NodeType.StartNode;
        plr.transform.localPosition = startNode.mapPos;
        plr.SetActive(true);
    }

    private MapNode AddPoint(Vector2 pos)
    {
        MapNode obj = Instantiate(point, transform);
        obj.transform.localPosition = pos;
        return obj;
    }
    
    private GameObject AddLine(Vector2 start, Vector2 end, Color color)
    {
        UILine obj = Instantiate(line, transform);
        obj.color = color;
        obj.start = start;
        obj.end = end;
        return obj.gameObject;
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
