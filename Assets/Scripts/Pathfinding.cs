using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Pathfinding
    {
        public static List<Grid.Tile> GetShortestPath(List<Grid.Tile> tiles, Grid.Tile start, List<Grid.Tile> targets, List<Grid.Tile> dangers)
        {
            List<WeightedTile> openTiles = new();
            List<WeightedTile> closedTiles = new();

            openTiles.Add(
                new WeightedTile
                {
                    Tile = start,
                    Parent = null,
                    StartDistance = 0,
                    EndDistance = Mathf.FloorToInt(targets.Min(t =>
                        Vector2.Distance(new Vector2(t.x, t.y), new Vector2(start.x, start.y)))),
                    DangerDistance = Mathf.FloorToInt(dangers.Min(t =>
                        Vector2.Distance(new Vector2(t.x, t.y), new Vector2(start.x, start.y))))
                }
            );

            for (int i = 0; i < 1000; i++)
            {
                WeightedTile current = openTiles.OrderBy(t => t.Weight).FirstOrDefault();
                openTiles.Remove(current);
                closedTiles.Add(current);

                if (targets.Contains(current.Tile))
                {
                    // shortest Path found
                    List<Grid.Tile> bestPath = new();

                    while (true)
                    {
                        bestPath.Add(current.Tile);
                        if (current.Parent == null) break;
                        current = current.Parent;
                    }
                    
                    return bestPath;
                }

                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        Grid.Tile neighbour = Grid.Instance.TryGetTile(new Vector2Int(current.Tile.x - x, current.Tile.y - y));
                        if (neighbour == null || neighbour.occupied || closedTiles.Count(t => t.Tile == neighbour) > 0)
                        {
                            continue;
                        }
                        
                        
                    }
                }
                
            }
            
            return new List<Grid.Tile>();
        }

        private class WeightedTile
        {
            public Grid.Tile Tile;
            public WeightedTile Parent;

            public int StartDistance;
            public int EndDistance;
            public int DangerDistance;
                
            public int Weight => StartDistance + EndDistance + DangerDistance;
        }
    }
}