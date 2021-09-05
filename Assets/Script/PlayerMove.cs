using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveTime;//每一次移动的基础速度，斜向移动时乘以1.4
    public GameObject YellowRing;//用于指示目标地点的黄圈

    private Tile from;//上一个到达的图块
    private Tile destination;//正在前往的图块

    private Tile[] route;//存储着整个路径的数组
    private int p;//当前在路径中的位置（路径队列的队首指针）[所指的位置就是destination]
    private int n;//路径队列的队尾指针[所指的位置有元素]

    private float startTime;//出发时间
    private float firstHalfTime;//前半段时间
    private float lastHalfTime;//后半段时间

    public void initialize(Tile startPlace)//初始化寻路物体的位置
    {
        route = new Tile[5000];
        from = startPlace;
        destination = startPlace;
        gameObject.transform.SetPositionAndRotation(new Vector3(startPlace.x, 1.5f, startPlace.z), Quaternion.identity);
        stop();
    }

    void Update()
    {
        if (!from.Equals(destination))//看当前位置和目标位置是否相同
        {//如果不相同，说明还在运动过程中
            if (Time.time - startTime < firstHalfTime + lastHalfTime)
            {
                if (Time.time - startTime < firstHalfTime)//正在走前半段
                {
                    float x = Mathf.Lerp(from.x, (from.x + destination.x) / 2f, (Time.time - startTime) / firstHalfTime);
                    float z = Mathf.Lerp(from.z, (from.z + destination.z) / 2f, (Time.time - startTime) / firstHalfTime);
                    gameObject.transform.position = new Vector3(x, 1.5f, z);
                }
                else//正在走后半段
                {
                    float x = Mathf.Lerp((from.x + destination.x) / 2f, destination.x, (Time.time - startTime - firstHalfTime) / lastHalfTime);
                    float z = Mathf.Lerp((from.z + destination.z) / 2f, destination.z, (Time.time - startTime - firstHalfTime) / lastHalfTime);
                    gameObject.transform.position = new Vector3(x, 1.5f, z);
                }
            }
            else//如果已经超出了时间，那么寻路物体想必已经到达了此步的目的地
            {
                this.from = this.destination;
                Debug.Log("到达" + from.x + " " + from.z + "，用时：" + (firstHalfTime + lastHalfTime));
            }
        }
        else if (p < n) //如果路径队列当中还有没有去到的地图块
        {
            p++;
            destination = route[p];//如果没走完，就把队列中下一个图块作为目的地

            firstHalfTime = moveTime * from.weight / 2;//计算前半段路程的用时
            lastHalfTime = moveTime * destination.weight / 2;//计算后半段路程的用时
            if (from.x != destination.x && from.z != destination.z)//如果路径是斜向的
            {
                this.firstHalfTime = firstHalfTime * 1.414f;
                this.lastHalfTime = lastHalfTime * 1.414f;//耗时都乘以1.414
            }
            startTime = Time.time;
        }
    }

    public void navigate(Tile target, Tile[,] map, int rows, int columns)//导航
    {
        Tile currentTile = stop();//停止之前的寻路
        Pathfinding.AStar(currentTile, target, map, rows, columns, ref n, route);
        YellowRing.transform.position = new Vector3(target.x, 0.51f, target.z); //将黄圈放置在寻路终点
        Debug.Log("路径长度" + n);
    }

    public Tile stop()//立即停止本次寻路过程，清空路径队列，并返回当前位置或下一个目标位置
    {
        this.route[0] = this.destination;
        this.n = 0;
        this.p = 0;
        YellowRing.transform.position = new Vector3(destination.x, 0.51f, destination.z);
        return destination;
    }

}
