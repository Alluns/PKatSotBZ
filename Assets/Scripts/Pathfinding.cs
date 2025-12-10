using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Pathfinding
    {
        
        public static List<Grid.Tile> GetPath(List<Grid.Tile> tiles, Grid.Tile start, Grid.Tile target)
        {
            List<WeightedTile> openTiles = new();
            List<WeightedTile> closedTiles = new();

            openTiles.Add(
                new WeightedTile
                {
                    Tile = start,
                    Parent = null,
                    tDist = 0,
                    gDist = Vector2.Distance(target.Coord, start.Coord),
                    // dDist = Vector2.Distance(new Vector2(danger.x, danger.y), new Vector2(start.x, start.y))
                }
            );
            
            for (int i = 0; i < 1000; i++)
            {
                WeightedTile current = openTiles.OrderBy(t => t.Weight).First();
                openTiles.Remove(current);
                closedTiles.Add(current);

                if (current.Tile.finishTile)
                {
                    // shortest Path found
                    List<Grid.Tile> bestPath = new();

                    while (true)
                    {
                        bestPath.Add(current.Tile);
                        if (current.Parent == null) break;
                        current = current.Parent;
                    }

                    bestPath.Reverse();
                    
                    return bestPath;
                }

                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (y == 0 & x == 0) continue;
                        
                        Grid.Tile neighbour = Grid.Instance.TryGetTile(new Vector2Int(current.Tile.x + x, current.Tile.y + y));
                        if (neighbour == null || neighbour.occupied || closedTiles.Exists(t => t.Tile == neighbour))
                        {
                            continue;
                        }

                        float distance = x == 0 || y == 0 ? 1f : 1.4142135f;

                        WeightedTile wNeighbour = openTiles.Exists(t => t.Tile == neighbour) ?
                            openTiles.First(t => t.Tile == neighbour)
                            : new WeightedTile
                            {
                                Parent = current,
                                Tile = neighbour,
                                gDist = Vector2.Distance(target.Coord, neighbour.Coord),
                                // dDist = Mathf.FloorToInt(Vector2.Distance(new Vector2(danger.x, danger.y), new Vector2(neighbour.x, neighbour.y)))
                            };

                        if (wNeighbour.Weight < wNeighbour.gDist + wNeighbour.dDist + current.tDist + distance  || !openTiles.Contains(wNeighbour))
                        {
                            wNeighbour.tDist = current.tDist + distance;
                            wNeighbour.Parent = current;
                            wNeighbour.gDist = Vector2.Distance(target.Coord, neighbour.Coord);
                        
                            openTiles.Add(wNeighbour);
                        }
                        
                    }
                }
            }
            
            return new List<Grid.Tile>();
        }

        private class WeightedTile
        {
            public Grid.Tile Tile;
            public WeightedTile Parent = null;

            public float tDist = 0;               // Distance traversed
            public float gDist = 0;               // Distance to the closest goal
            public float dDist = 0;               // Danger distance
                
            public float Weight => tDist + gDist + dDist;
        }
    }
}