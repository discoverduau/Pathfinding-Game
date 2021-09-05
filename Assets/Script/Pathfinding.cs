using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    private static Tile FindInList(int x, int z, List<Tile> list)//判断点是否在list中
    {
        foreach (Tile t in list)
        {
            if (t.x == x && t.z == z)
                return t;
        }
        return null;
    }

    private static double GetG(Tile v, Tile[,] map)//考虑上下左右，上左，上右，下左，下右的距离
    {
        int i = (v.parent.x - v.x) * (v.parent.z - v.z);
        if (v.parent != null && i >= -1 && i <= 1)
            return v.parent.G + Mathf.Abs(map[v.x, v.z].weight);
        else
            return 0;
    }

    private static double EstimateDistance(Tile s, Tile e)//画框法计算距离
    {
        if (Mathf.Abs(s.x - e.x) > Mathf.Abs(s.z - e.z))//如果x方向的距离大于z方向的距离
            return Mathf.Abs(s.z - e.z) * 1.414f + Mathf.Abs(s.x - e.x) - Mathf.Abs(s.z - e.z);
        else if (Mathf.Abs(s.x - e.x) == Mathf.Abs(s.z - e.z))//如果x方向的距离和z方向的距离相等
            return Mathf.Abs(s.x - e.x) * 1.414f;
        else//如果x方向的距离小于z方向的距离
            return Mathf.Abs(s.x - e.x) * 1.414f + Mathf.Abs(s.z - e.z) - Mathf.Abs(s.x - e.x);
    }

    private static Tile GetMinVertexFromOpenList(List<Tile> OpenList)//从OpenList中查找F值最小的顶点
    {
        double min = OpenList[0].F;
        Tile MinVertex = OpenList[0];
        for (int i = 1; i < OpenList.Count(); i++)
        {
            if (OpenList[i].F < min)
            {
                min = OpenList[i].F;
                MinVertex = OpenList[i];
            }
        }
        return MinVertex;
    }

    public static void AStar(Tile StartV, Tile EndV, Tile[,] map, int rows, int columns, ref int count, Tile[] route)
    {
        StartV.F = 0;
        StartV.G = 0;
        StartV.H = 0;
        StartV.parent = null;

        int vexIndex = 0;//指示openList的当前元素个数
        List<Tile> open = new List<Tile>();
        open.Add(StartV);//初始化openList
        List<Tile> close = new List<Tile>();

        int[,] direction = new int[8, 2] //表示方向的二元组
        {
            { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, //正向
            { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } //斜向
        };
        while (open.Count() > 0)//当openList不为空时
        {
            Tile MinVertex = GetMinVertexFromOpenList(open);//遍历openList,查找F值最小的顶点
            if (MinVertex.x == EndV.x && MinVertex.z == EndV.z)//当前找到的F值最小顶点为终点时结束
            {
                EndV.parent = MinVertex.parent;
                break;//路径求算完成
            }
            close.Add(MinVertex);
            open.Remove(MinVertex);//将F值最小的节点移入closeList中，并将其作为当前要处理的节点
            for (int i = 0; i < 8; i++)//开始移动点，并排除超出地图外的点
            {
                int nextX = MinVertex.x + direction[i, 0];
                int nextZ = MinVertex.z + direction[i, 1];
                if (nextX >= 0 && nextX < rows && nextZ >= 0 && nextZ < columns)//限定点在地图中
                {
                    if (FindInList(nextX, nextZ, close) == null)
                    {//不在closeList中
                        if (FindInList(nextX, nextZ, open) == null)//不在openList中，则加入openList，并计算其F,G,H
                        {
                            Tile vex = map[nextX, nextZ];
                            vex.parent = MinVertex;
                            vex.G = GetG(vex, map);
                            vex.H = EstimateDistance(vex, EndV);//用画框法估算距离
                            vex.F = vex.G + vex.H;
                            open.Add(vex);
                            vexIndex++;
                        }
                        else  //该顶点已经加入过openList
                        {
                            //从openList中获取该节点
                            Tile vex = FindInList(nextX, nextZ, open);
                            if (MinVertex.G + map[vex.x, vex.z].weight < vex.G)
                            {
                                vex.parent = MinVertex;
                                vex.G = MinVertex.G + map[vex.x, vex.z].weight;
                                vex.F = vex.G + vex.H;
                            }

                        }
                    }
                }
            }
        }

        Tile v = EndV;
        for (count = 0; v.parent != null; count++)
        {
            route[count] = v;
            v = v.parent;
        }
        for (int n = 0, m = count; n < m; n++, m--)
        {//倒序逆置
            v = route[n];
            route[n] = route[m];
            route[m] = v;
        }

    }

}