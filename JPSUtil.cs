using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JPSUtil
{
    public static readonly Vector2 left = Vector2.left;
    public static readonly Vector2 right = Vector2.right;
    public static readonly Vector2 up = Vector2.up;
    public static readonly Vector2 down = Vector2.down;
    public static readonly Vector2 leftUp = new Vector2(-1, 1);
    public static readonly Vector2 leftDown = new Vector2(-1, -1);
    public static readonly Vector2 rightUp = new Vector2(1, 1);
    public static readonly Vector2 rightDown = new Vector2(1, -1);

    public static Dictionary<Vector2, Vector2[]> dictionary = new Dictionary<Vector2, Vector2[]>() {
        { left, new Vector2[] {up, down} },
        { right, new Vector2[] {up, down} },
        { up, new Vector2[] {left, right} },
        { down, new Vector2[] {left, right} }
    };
}
