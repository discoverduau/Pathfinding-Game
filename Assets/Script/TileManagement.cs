using UnityEngine;
using Object = UnityEngine.Object;

public class TileManagement : MonoBehaviour
{
    public GameObject Player;//寻路物体
    public GameObject UI;//UI面板
    public CameraBehaviour MainCamera;//主摄像机脚本
    public Tile[,] tileMatrix;

    [Header("场地生成参数")]
    public int rows;//行数
    public int columns;//列数
    public float initialWeight;//初始权值
    public Object tilePrefab;//Prefab

    [Header("寻路物体初始化位置")]
    public int startX;
    public int startZ;

    [Header("权重画刷设置")]
    public int numOfLevel;// 权值分档数量(大于0)
    public float weightSpan;// 相邻两档之间的权值差
    private float colorChange;// 每次的颜色变化值
    private float maximumWeight;// 能够到达的最大权值

    private int brushNo = 1;//刷子序列号，用于画图控制
    private Ray ray;//ray是屏幕点击射线
    private RaycastHit hit;//hit用于保存射线碰撞事件
    private int old_rows, old_columns;//用于重构地图

    public void reset()
    {
        for (int x = 0; x < old_rows; x++)
        {
            for (int z = 0; z < old_columns; z++)
            {
                Destroy(tileMatrix[x, z].tilePrefab);//删除之前的Tile物体
                tileMatrix[x, z] = null;//删除tile矩阵中的数据
            }
        }
        UI.SetActive(false);//关闭Tab面板

        old_rows = rows;
        old_columns = columns;//保存本次的生成数据，以便下次解构地图

        colorChange = 1f / (numOfLevel - 1);//根据权值总档数求得每次颜色的变化值
        maximumWeight = initialWeight + (numOfLevel - 1) * weightSpan;//求出能够表示的最大权值

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Object newTile = Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity, transform);//生成地图块物体
                tileMatrix[x, z] = new Tile(x, z, initialWeight, newTile);//生成地图矩阵
            }
        }

        Player.GetComponent<PlayerMove>().initialize(tileMatrix[startX, startZ]);//寻路物体初始化
        MainCamera.initialize();//相机位置转移
    }

    void Start()
    {
        old_rows = rows;
        old_columns = columns;//保存本次的生成数据，以便下次解构地图

        tileMatrix = new Tile[300, 300];//最大支持300×300的地图

        colorChange = 1f / (numOfLevel - 1);//根据权值总档数求得每次颜色的变化值
        maximumWeight = initialWeight + (numOfLevel - 1) * weightSpan;//求出能够表示的最大权值

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Object newTile = Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity, transform);//生成地图块物体
                tileMatrix[x, z] = new Tile(x, z, initialWeight, newTile);//生成地图矩阵
            }
        }

        Player.GetComponent<PlayerMove>().initialize(tileMatrix[startX, startZ]);//寻路物体初始化
        MainCamera.initialize();//相机位置转移
    }

    void Update()
    {
        if (!UI.activeSelf)//UI开启的时候无法进行操作
        {
            if (Input.GetMouseButton(0))//鼠标左键点击（寻路或者升高权重）
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);//创建射线
                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))//shift键按下的情况
                    {
                        Tile connectedTile = tileMatrix[(int)hit.transform.localPosition.x, (int)hit.transform.position.z];
                        //根据坐标确定与之关联的Tile结构体（在二维数组中）
                        if (connectedTile.compareBrushNo(brushNo) && connectedTile.weight < maximumWeight)
                        {
                            float newG = (float)(hit.transform.GetComponent<Renderer>().material.color.g - colorChange);//计算新的G值
                            float newB = (float)(hit.transform.GetComponent<Renderer>().material.color.b - colorChange);//计算新的B值
                            hit.transform.GetComponent<Renderer>().material.color = new Color(1f, newG, newB, 0f);//更新颜色
                            connectedTile.weight += weightSpan;//权值增加
                            Debug.Log("权值增加，更新后的权值：" + connectedTile.weight);
                        }
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {//按下鼠标左键但是没有按Shift——指定新的目的地，调用A*开始寻路
                        Tile destinationTile = tileMatrix[(int)hit.transform.localPosition.x, (int)hit.transform.position.z];
                        Player.GetComponent<PlayerMove>().navigate(destinationTile, tileMatrix, rows, columns);//对新的目的地开始寻路
                    }
                }
            }
            else if (Input.GetMouseButton(1))//鼠标右键点击且Shift按下（降低权重）
            {
                if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                {//按下鼠标右键但是没有按Shift——中止寻路过程
                    Tile currentTile = Player.GetComponent<PlayerMove>().stop();//立即终止寻路并打印位置
                    Debug.Log("中止寻路，当前位置：" + currentTile.x + " " + currentTile.z);
                }
                else  //当前处于按住鼠标右键的状态，或者按下鼠标右键但是没按Shift的情况
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);//创建射线
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        {
                            Tile connectedTile = tileMatrix[(int)hit.transform.localPosition.x, (int)hit.transform.position.z];
                            //根据坐标确定与之关联的Tile结构体（在二维数组中）
                            if (connectedTile.compareBrushNo(brushNo) && connectedTile.weight > initialWeight)
                            {
                                float newG = (float)(hit.transform.GetComponent<Renderer>().material.color.g + colorChange);//计算新的G值
                                float newB = (float)(hit.transform.GetComponent<Renderer>().material.color.b + colorChange);//计算新的B值
                                hit.transform.GetComponent<Renderer>().material.color = new Color(1f, newG, newB, 0f);//更新颜色
                                connectedTile.weight -= weightSpan;//权值减小
                                Debug.Log("权值降低，更新后的权值：" + connectedTile.weight);
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            brushNo++;//刷子序列号更新

        if (Input.GetKeyDown(KeyCode.Tab))
            UI.SetActive(!UI.activeSelf);//开启/关闭UI
    }

}
