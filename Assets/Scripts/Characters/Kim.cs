using System.Collections.Generic;
using UnityEngine;

public class Kim : CharacterController
{
    [SerializeField] float ContextRadius;

    public override void StartCharacter()
    {
        base.StartCharacter();

        for (int i = 0; i < 5; i++)
        {
            myWalkBuffer.Add(Grid.Instance.TryGetTile(new Vector2Int(myCurrentTile.x + i, myCurrentTile.y)));
        }
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
}
