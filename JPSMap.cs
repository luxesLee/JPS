using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSMap
{
    private static JPSMap instance;
    public static JPSMap Instance {
        get {
            if(instance == null) {
                instance = new JPSMap();
            }
            return instance;
        }
    }
    public int Width = 7, Height = 5;
    public JPSPoint[,] map;
    public List<JPSPoint> obstacles = new List<JPSPoint>();

    public JPSMap() {
        Init();
    }

    private void Init() {
        InitMap();
        InitObstacles();
    }

    private void InitMap() {
        map = new JPSPoint[Width, Height];
        for(int i = 0; i < Width; i++) {
            for(int j = 0; j < Height; j++) {
                map[i, j] = new JPSPoint(i, j);
            }
        }
    }

    private void InitObstacles() {
        obstacles.Add(map[4, 2]);
        obstacles.Add(map[4, 1]);
        obstacles.Add(map[4, 0]);
        // obstacles.Add(map[0, 1]);

        foreach(var obstacle in obstacles) {
            obstacle.IsObstacle = true;
            CreatePath(obstacle.position, Color.blue);
        }
    }

    private void CreatePath(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(GameObject.Find("Path").transform);
        map[(int)position.x, (int)position.y].cube = cube;
    }

}
