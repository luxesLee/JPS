using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSPoint
{
    public GameObject cube;
    public Vector2 position;
    public bool IsObstacle;
    public JPSPoint parent;
    public float G, H, F;
    public List<Vector2> directions;

    public JPSPoint(int x, int y) {
        position = new Vector2(x, y);
        IsObstacle = false;
        parent = null;
        cube = null;

        directions = new List<Vector2>();
    }
}
