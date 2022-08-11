using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJPS : MonoBehaviour
{
    private Transform cubeParent;
    private JPSPoint[,] map;
    private float mTime = 0.7f;
    private float mTimer = 0f;
    private JPSPoint startPoint;
    private JPSPoint endPoint;
    private List<JPSPoint> path;

    // Start is called before the first frame update
    void Start()
    {
        cubeParent = GameObject.Find("Root").transform;
        map = JPSMap.Instance.map;
        startPoint = map[0, 0];
        InitBackground();
    }

    // Update is called once per frame
    void Update()
    {
        mTimer += Time.deltaTime;
        if(mTimer >= mTime) {
            mTimer = 0;
            Walk();
        }
    }

    private void InitBackground() {
        for(int i = 0; i < JPSMap.Instance.Width; i++) {
            for(int j = 0; j < JPSMap.Instance.Height; j++) {
                CreateGrid(new Vector2(i, j), Color.gray);
            }
        }
    }

    private void CreateGrid(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(cubeParent);
        cube.AddComponent<Cube>().FindPath = FindPath;
    }

    private void Walk() {
        if(path != null && path.Count > 1) {
            startPoint = path[path.Count - 1];
            Color color = startPoint.cube.GetComponent<Renderer>().material.color;
            path.Remove(startPoint);
            Destroy(startPoint.cube);
            startPoint.cube = null;

            startPoint = path[path.Count - 1];
            startPoint.cube.GetComponent<Renderer>().material.color = color;
        }
    }

    public void FindPath(Vector2 target) {
        JPS.Instance.ClearGrid();

        endPoint = map[(int)target.x, (int)target.y];

        path = JPS.Instance.FindPath(startPoint, endPoint);
    }

}
