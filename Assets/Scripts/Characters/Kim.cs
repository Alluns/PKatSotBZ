using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Kim : CharacterController
{
    [SerializeField] float ContextRadius;

    private List<Grid.Tile> path = new();
    
    public override void StartCharacter()
    {
        base.StartCharacter();

        Grid.Tile start = Grid.Instance.GetClosest(transform.position);
        path = Pathfinding.GetPath(Grid.Instance.tiles, start, Grid.Instance.GetFinishTile());

        myWalkBuffer.AddRange(path);
    }

    public override void UpdateCharacter()
    {
        base.UpdateCharacter();

        Zombie closest = GetClosest(GetContextByTag("Zombie"))?.GetComponent<Zombie>();
    }

    Vector3 GetEndPoint()
    {
        return Grid.Instance.WorldPos(Grid.Instance.GetFinishTile());
    }

    GameObject[] GetContextByTag(string aTag)
    {
        Collider[] context = Physics.OverlapSphere(transform.position, ContextRadius);
        List<GameObject> returnContext = new List<GameObject>();
        foreach (Collider c in context)
        {
            if (c.transform.CompareTag(aTag))
            {
                returnContext.Add(c.gameObject);
            }
        }
        return returnContext.ToArray();
    }

    GameObject GetClosest(GameObject[] aContext)
    {
        float dist = float.MaxValue;
        GameObject Closest = null;
        foreach (GameObject z in aContext)
        {
            float curDist = Vector3.Distance(transform.position, z.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                Closest = z;
            }
        }
        return Closest;
    }

    private void OnDrawGizmos()
    {
        if (path.Count == 0) return;

        foreach (Grid.Tile tile in path)
        {
            Gizmos.color = Color.chartreuse;
            Gizmos.DrawCube(Grid.Instance.WorldPos(tile), Vector3.one * 0.3f);
        }
    }
}
