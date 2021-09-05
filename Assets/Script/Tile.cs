using System;
using Object = UnityEngine.Object;

public class Tile
{
    public int x;
    public int z;
    public float weight;//权
    public Object tilePrefab;//对应的方块

    public Tile parent;//父节点（前驱节点）,默认无父节点
    public double F, G, H;//估算距离公式F = G + H

    private int brushNo;//权值绘制相关变量

    public Tile(int x, int z, float weight, Object tilePrefab)
    {
        this.x = x;
        this.z = z;
        this.weight = weight;
        this.tilePrefab = tilePrefab ?? throw new ArgumentNullException(nameof(tilePrefab));//检测空指针报错
        brushNo = 0;
        //F = 0;
        //G = 0;
        //H = 0;
    }

    public Tile()
    {
    }

    public Boolean compareBrushNo(int No)
    {
        if (this.brushNo == No)
        {//此次刷子已经刷过
            return false;
        }
        else
        {//此次刷子没有刷过
            this.brushNo = No;
            return true;
        }
    }

}