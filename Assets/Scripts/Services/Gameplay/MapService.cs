using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapService : MonoBehaviour
{
    public float GetMapHeight(mmo_shared.Vector2 point)
    {
        return Terrain.activeTerrain.SampleHeight(new Vector3(point.X, 0, point.Y));
    }

    public float GetMapHeight(float x, float y)
    {
        return GetMapHeight(new mmo_shared.Vector2(x, y));
    }

}
