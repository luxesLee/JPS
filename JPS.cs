using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPS
{
    private static JPS instance;
    public static JPS Instance {
        get {
            if(instance == null) {
                instance = new JPS();
            }
            return instance;
        }
    }

    private JPSPoint[,] map;
    private JPSPoint startPoint;
    private JPSPoint endPoint;
    private List<JPSPoint> openList, closeList;



    public JPS() {
        map = JPSMap.Instance.map;

    }

    public List<JPSPoint> FindPath(JPSPoint start, JPSPoint end) {
        if(start == end) {
            return null;
        }

        startPoint = start;
        endPoint = end;

        openList = new List<JPSPoint>();
        closeList = new List<JPSPoint>();

        startPoint.directions.Add(JPSUtil.left);
        startPoint.directions.Add(JPSUtil.right);
        startPoint.directions.Add(JPSUtil.up);
        startPoint.directions.Add(JPSUtil.down);
        startPoint.directions.Add(JPSUtil.leftDown);
        startPoint.directions.Add(JPSUtil.leftUp);
        startPoint.directions.Add(JPSUtil.rightDown);
        startPoint.directions.Add(JPSUtil.rightUp);

        openList.Add(startPoint);

        while(openList.Count > 0) {
            // 从集合中选出F最小的跳点
            JPSPoint curPoint = GetTheMinfPoint(openList);

            if(curPoint == end) {
                break;
            }

            openList.Remove(curPoint);
            closeList.Add(curPoint);

            
            List<JPSPoint> jumpPoints = new List<JPSPoint>();

            foreach(var direction in curPoint.directions) {
                if(direction.x * direction.y == 0) {
                    JPSPoint point = GetStraights(curPoint, direction);
                    if(point != null)
                        jumpPoints.Add(point);
                }
                else {
                    JPSPoint point = GetDiagonals(curPoint, direction);
                    if(point != null)
                        jumpPoints.Add(point);
                }
            }

            foreach(JPSPoint point in jumpPoints) {
                Debug.Log(point.position);
                if(openList.Contains(point)) {
                    float new_G = CalcG(point, curPoint);
                    if(new_G < point.G) {
                        point.G = new_G;
                        point.F = point.G + point.H;
                        point.parent = curPoint;
                    }
                }
                else {
                    openList.Add(point);
                    point.parent = curPoint;
                    CalcF(point, end);
                }
            }

        }

        // 没找到
        if(!openList.Contains(end)) {
            Debug.Log("目标不可达");
            return null;
        }

        return GetPath(start, end);
    }

    private List<JPSPoint> GetPath(JPSPoint start, JPSPoint end) {
        List<JPSPoint> path = new List<JPSPoint>();

        JPSPoint curPoint = end;
        while(true) {
            path.Add(curPoint);

            Color color = Color.white;
            if(curPoint == start) color = Color.green;
            else if(curPoint == end) color = Color.red;

            // 因为这里全都是跳点，所以显示路径会有问题
            CreatePath(curPoint.position, color);

            if(curPoint == start) break;
            curPoint = curPoint.parent;
        }        
        return path;
    }

    public void CreatePath(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(GameObject.Find("Path").transform);
        GetPointFromMap(position).cube = cube;
    }

    public void ClearGrid() {
        for(int i = 0; i < JPSMap.Instance.Width; i++) {
            for(int j = 0; j < JPSMap.Instance.Height; j++) {
                if(!map[i, j].IsObstacle && map[i, j].cube != null) {
                    GameObject.Destroy(map[i, j].cube);
                    map[i, j].parent = null;
                    map[i, j].cube = null;
                }
            }
        }
    }

    private JPSPoint GetTheMinfPoint(List<JPSPoint> openList) {
        float val = float.MaxValue;
        JPSPoint ans = null;
        foreach(JPSPoint point in openList) {
            if(point.F < val) {
                val = point.F;
                ans = point;
            }
        }
        return ans;
    }

    /// <summary>
    /// 检查curPoint沿直线方向是否有跳点
    /// </summary>
    /// <param name="curPoint"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private JPSPoint GetStraights(JPSPoint curPoint, Vector2 direction) {

        JPSPoint point = GetPointFromMap(curPoint.position + direction);
        while(canMove(point)) {
            
            // 该point为endPoint或存在强迫邻居则为跳点
            if(point == endPoint) {
                return point;
            }

            List<Vector2> dirs = ForceNeighborsInStraights(point, direction);
            if(dirs.Count > 0) {
                dirs.Add(direction);
                point.directions.AddRange(dirs);
                return point;
            }

            // closeList.Add(point);

            // 否则沿着该方向递归
            point = GetPointFromMap(point.position + direction);
        }
        
        return null;
    }

    /// <summary>
    /// 该点在此直线方向上是否有强迫邻居
    /// </summary>
    /// <param name="point"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private List<Vector2> ForceNeighborsInStraights(JPSPoint point, Vector2 direction) {
        List<Vector2> dirs = new List<Vector2>();
        foreach(var d in JPSUtil.dictionary[direction]) {
            JPSPoint block = GetPointFromMap(point.position + d);
            JPSPoint FN = GetPointFromMap(point.position + d + direction);
            if(!canMove(block) && canMove(FN)) {
                dirs.Add(d + direction);
            }
        }
        return dirs;
    }

    /// <summary>
    /// 检查对角移动是否有跳点
    /// </summary>
    /// <param name="curPoint"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private JPSPoint GetDiagonals(JPSPoint curPoint, Vector2 direction) {
        // 获取对角移动的两个可能阻碍方格
        // closeList.Add(curPoint);

        Vector2 pos1 = new Vector2(curPoint.position.x + direction.x, curPoint.position.y);
        Vector2 pos2 = new Vector2(curPoint.position.x, curPoint.position.y + direction.y);
        JPSPoint point1 = GetPointFromMap(pos1), point2 = GetPointFromMap(pos2);
        
        // 获取对角移动的下一点
        JPSPoint nextPoint = GetPointFromMap(curPoint.position + direction);
        if(!canMove(nextPoint)) return null;

        // if(closeList.Contains(nextPoint)) return null;

        if(canMove(point1)) {
            if(canMove(point2)) {    // 对角移动两个分量方向无阻碍
                
                if(nextPoint == endPoint) return nextPoint;

                // 对角移动的两个分量方向上存在跳点，则该点也是跳点
                List<Vector2> dirs = DiagonalExplore(nextPoint, direction);
                if((dirs.Count > 0)) {
                    nextPoint.directions.AddRange(dirs);
                    nextPoint.directions.Add(direction);
                    return nextPoint;
                }

                // 递归下一对角点
                return GetDiagonals(nextPoint, direction);

            }
            else {  // 对角移动point2方向有阻碍
                
                if(nextPoint == endPoint) return nextPoint;

                // 若对角分量方向存在跳点，或对角方向存在强迫邻居
                List<Vector2> dirs = ForceNeighborsInDiagonal(nextPoint, point2, direction, JPSUtil.up);
                dirs.AddRange(DiagonalExplore(nextPoint, direction));
                if(dirs.Count > 0) {
                    dirs.Add(direction);
                    nextPoint.directions.AddRange(dirs);
                    return nextPoint;
                }

                return GetDiagonals(nextPoint, direction);
            }
        }
        else {
            if(canMove(point2)) {    // 对角移动point1方向有阻碍

                if(nextPoint == endPoint) return nextPoint;

                List<Vector2> dirs = ForceNeighborsInDiagonal(nextPoint, point1, direction, JPSUtil.right);
                dirs.AddRange(DiagonalExplore(nextPoint, direction));
                if(dirs.Count > 0) {
                    dirs.Add(direction);
                    nextPoint.directions.AddRange(dirs);
                    return nextPoint;
                }

                return GetDiagonals(nextPoint, direction);
            }
            else {
                
                return null;
            }
        }
    }

    
    private List<Vector2> ForceNeighborsInDiagonal(JPSPoint point, JPSPoint obstaclePoint, Vector2 direction, Vector2 mask){
        List<Vector2> dirs = new List<Vector2>();
        JPSPoint fNpoint = GetPointFromMap(obstaclePoint.position + direction * mask);
        if(canMove(fNpoint)) {
            dirs.Add(fNpoint.position - point.position);
        }
        return dirs;
    }

    /// <summary>
    /// 沿对角移动的两个分量方向进行检测是否存在跳点
    /// </summary>
    /// <param name="point"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private List<Vector2> DiagonalExplore(JPSPoint point, Vector2 direction) {
        List<Vector2> dirs = new List<Vector2>();
        JPSPoint p1 = GetStraights(point, new Vector2(direction.x, 0));
        JPSPoint p2 = GetStraights(point, new Vector2(0, direction.y));
        
        if(p1 != null) dirs.Add(new Vector2(direction.x, 0));
        if(p2 != null) dirs.Add(new Vector2(0, direction.y));
        return dirs;
    }


    private bool canMove(JPSPoint point) {
        return point != null && !point.IsObstacle;
    }

    private JPSPoint GetPointFromMap(Vector2 pos) {
        if(!IsPointValid(pos)) return null;
        return map[(int)pos.x, (int)pos.y];
    }

    private bool IsPointValid(Vector2 pos) {
        return (pos.x >= 0 && pos.x < JPSMap.Instance.Width && pos.y >= 0 && pos.y < JPSMap.Instance.Height);
    }

    #region Calc
    // G用绝对距离
    // 上一跳点到当前节点
    private float CalcG(JPSPoint prevPoint, JPSPoint curPoint) {
        return Vector3.Distance(prevPoint.position, curPoint.position) + curPoint.G;
    }

    // H用曼哈顿距离计算
    // 当前节点到终点
    private float CalcH(JPSPoint curPoint, JPSPoint endPoint) {
        return Mathf.Abs(endPoint.position.x - curPoint.position.x) + Mathf.Abs(endPoint.position.y - curPoint.position.y);
    }

    private void CalcF(JPSPoint curPoint, JPSPoint endPoint) {
        curPoint.H = CalcH(curPoint, endPoint);
        if(curPoint.parent == null) {
            curPoint.G = 0;
        }
        else {
            curPoint.G = CalcG(curPoint, curPoint.parent);
        }
        curPoint.F = curPoint.H + curPoint.G;
    }

    #endregion

}
