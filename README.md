# JPS

JPS（Jump Point Search）算法核心在于寻找规划路径中具有对称性的路径，避免大量无用节点。


劣性节点，从父节点不经过当前节点而直接抵达所需要的代价，小于等于，从父节点抵达当前节点，再抵达劣性节点所需的代价。

自然节点，大于即为自然节点。

## Look Ahead Rule

直行

对角


## 流程

```
Loop
    if queue is empty:
        return false;
        break;
    Remove the node n with the lowest f from the priority queue;
    Mark node n as expanded;
    if the node n is the goal state:
        return true;
        break;
    For all unexpanded neighbours m of node n:
        if g(m) == infinite:
            g(m) = g(n) + Cost_{mn}
            Push node m into the queue
        if g(m) > g(n) + Cost_{mn}
            g(m) = g(n) + Cost_{mn}
    end
End Loop
```


## 评价

JPS在复杂环境下的效果远超A*，因为JPS可以减少加入openList中节点的数目，仅对关键节点进行探索。

但是若是在无障碍物的非复杂环境下，JPS的效率可能低于A*。例如，当起点的左侧地图无障碍物，而起点右侧的终点有障碍物阻挡，JPS会先向左侧探索，当抵达边缘时才会向右探索。

