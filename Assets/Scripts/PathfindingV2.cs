using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PathfindingV2 : MonoBehaviour
{
    private readonly float rootOf2 = Mathf.Sqrt(2);
    
    private readonly List<WeightedTile> openTiles = new();
    private readonly List<WeightedTile> closedTiles = new();
    private readonly List<Grid.Tile> completedPath = new();
    
    [SerializeField] private Color pathColor = new(0.09f, 0.63f, 0.52f);
    [SerializeField] private Color defaultColor = new(0.95f, 0.61f, 0.07f);
    [SerializeField] private Color dangerColor = new(0.75f, 0.22f, 0.17f);
    [SerializeField] private Color relationshipColor = new(0.17f, 0.24f, 0.31f);

    [SerializeField] private float dangerSearchRadius = 2.5f;
    [SerializeField] private float dangerCostMultiplier = 2.5f;
    
    private class WeightedTile
    {
        public Grid.Tile tile;
        public WeightedTile parent = null;

        public float travelCost = float.PositiveInfinity;           // Best calculated path to the tile
        public float heuristicCost = float.PositiveInfinity;        // Estimated cost to the goal
        public float dangerCost = 0f;                               // Danger cost of the tile
                
        public float Weight => travelCost + heuristicCost + dangerCost;
    }
    
    public List<Grid.Tile> GetPath(Grid.Tile start, List<Grid.Tile> targets)
    {
        openTiles.Add(new WeightedTile
        {
            tile = start,
            travelCost = 0,
            heuristicCost = targets.Min(t => Vector2.Distance(t.Coord, start.Coord)),
            dangerCost = GetDangerCost(Grid.Instance.WorldPos(start))
        });

        while (openTiles.Count > 0)
        {
            float minWeight = openTiles.Min(t => t.Weight);
            WeightedTile current = openTiles.First(t => Mathf.Approximately(t.Weight, minWeight));
            
            openTiles.Remove(current);
            closedTiles.Add(current);
            
            if (current.tile.finishTile)
            {
                while (true)
                {
                    completedPath.Add(current.tile);
                    if (current.parent == null) break;
                    current = current.parent;
                }

                completedPath.Reverse();
                
                return completedPath;
            }
            
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (y == 0 & x == 0) continue;
                        
                    Grid.Tile neighbour = Grid.Instance.TryGetTile(new Vector2Int(current.tile.x + x, current.tile.y + y));
                    if (neighbour == null || neighbour.occupied || closedTiles.Exists(t => t.tile == neighbour))
                    {
                        continue;
                    }

                    float distance = x == 0 || y == 0 ? 1f : rootOf2;

                    WeightedTile wNeighbour = openTiles.Exists(t => t.tile == neighbour) ?
                        openTiles.First(t => t.tile == neighbour)
                        : new WeightedTile
                        {
                            parent = current,
                            tile = neighbour,
                            heuristicCost = targets.Min(t => Vector2.Distance(t.Coord, neighbour.Coord)),
                            dangerCost = GetDangerCost(Grid.Instance.WorldPos(neighbour))
                        };

                    if (wNeighbour.Weight > wNeighbour.heuristicCost + wNeighbour.dangerCost + current.travelCost + distance  || !openTiles.Contains(wNeighbour))
                    {
                        wNeighbour.travelCost = current.travelCost + distance;
                        wNeighbour.parent = current;
                        wNeighbour.heuristicCost = targets.Min(t => Vector2.Distance(t.Coord, neighbour.Coord));
                        
                        openTiles.Add(wNeighbour);
                    }
                }
            }
        }
        
        return new List<Grid.Tile>();
    }

    private float GetDangerCost(Vector3 worldPos)
    {
        Collider[] context = Physics.OverlapSphere(worldPos, dangerSearchRadius);
        List<GameObject> zombies = new List<GameObject>();
        foreach (Collider c in context)
        {
            if (c.transform.CompareTag("Zombie"))
            {
                zombies.Add(c.gameObject);
            }
        }

        if (zombies.Count == 0) return 0f;
        
        float closesDistance = zombies.Min(z => Vector2.Distance(z.transform.position, worldPos));

        return (dangerSearchRadius / closesDistance) * dangerCostMultiplier;
    }

    private void OnDrawGizmos()
    {
        if (openTiles.Count > 0)
        {
            foreach (WeightedTile weightedTile in openTiles)
            {
                Gizmos.color = Color.Lerp(defaultColor, dangerColor, weightedTile.dangerCost / dangerSearchRadius * dangerCostMultiplier);
                Gizmos.DrawCube(Grid.Instance.WorldPos(weightedTile.tile), Vector3.one * 0.125f);

                if (weightedTile.parent == null) continue;
                
                Gizmos.color = relationshipColor;
                Gizmos.DrawLine(Grid.Instance.WorldPos(weightedTile.parent.tile), Grid.Instance.WorldPos(weightedTile.tile));
            }
        }
        
        if (closedTiles.Count > 0)
        {
            foreach (WeightedTile weightedTile in closedTiles)
            {
                Gizmos.color = Color.Lerp(defaultColor, dangerColor, weightedTile.dangerCost / dangerSearchRadius * dangerCostMultiplier);
                Gizmos.DrawCube(Grid.Instance.WorldPos(weightedTile.tile), Vector3.one * 0.125f);

                if (weightedTile.parent == null) continue;
                
                Gizmos.color = relationshipColor;
                Gizmos.DrawLine(Grid.Instance.WorldPos(weightedTile.parent.tile), Grid.Instance.WorldPos(weightedTile.tile));
            }
        }

        if (completedPath.Count > 0)
        {
            foreach (Grid.Tile tile in completedPath)
            {
                Gizmos.color = pathColor;
                Gizmos.DrawCube(Grid.Instance.WorldPos(tile), Vector3.one * 0.25f);
            }
        }
    }
}