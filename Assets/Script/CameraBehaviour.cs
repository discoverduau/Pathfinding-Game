using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public TileManagement map;
    public GameObject Player;
    public bool PlayerTracking = false;//是否追踪玩家
    public float moveTime = 0.8f;//相机移动所用时间

    private float startTime;//开始移动的时间

    public void initialize()
    { //移动摄像机的位置
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float startTime = Time.time;
        Vector3 oldPosition = gameObject.transform.position;
        Vector3 newPosition = new Vector3(map.rows / 2, map.rows + 5, -2);//一个比较好的相机位置计算方法
        //（PS：我也不知道自己咋想出来这组计算方法的，不过看样子从2行2列到500行500列，镜头视角都挺合适的 = w =）
        //记住相机Rotation ( 70 , 0 , 0 )

        while (Time.time - startTime < moveTime)
        {
            gameObject.transform.position = Vector3.Lerp(oldPosition, newPosition, (Time.time - startTime) / moveTime);
            yield return null;
        }
        gameObject.transform.position = newPosition;
        yield break;
    }

    void Update()
    {
        if (PlayerTracking)//如果开启
            gameObject.transform.LookAt(Player.transform);//相机跟踪玩家
    }

}
