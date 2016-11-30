using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
//要求物件上要有Rigidbody2D及Seeker的子物件(在unity稱作component)，如果物件沒有unity會自動增加這兩種子物件。
public class AI : MonoBehaviour {

    public Transform Target;
    //目標物的transform子物件
    public Rigidbody2D RD;
    //本身的剛體子物件
    private Seeker seeker;
    //本身的seeker子物件
    private Path path;
    //利用pathfinding搜尋的路徑會存在這個變數中
    public float AISpeed = 8f;
    //AI的移動速度
    public float LimitDistance = 3f;
    //距離最大上限
    private int NextPoint = 0;
    //AI現在的目標
    public float UpdateRate = 0.1f;
    //更新路徑參數頻率，數字越小時更新越快
    public ForceMode2D FM;
    //Addforce所需使用之力量模式
    private float LastTime = -9999;
    //為一判斷時間所用到之變數，初始值為-9999使其必定能夠一開始就執行
    [HideInInspector]
    //[HideInInspector]以下宣告的變數無法在InInspector直接修改，但可由其他函數取得

    //unity一開始會優先選擇執行的函式為awake()，此處start()為第二順位執行
	// Use this for initialization
	void Start () 
    {
        seeker = GetComponent<Seeker>();
        RD = GetComponent<Rigidbody2D>();
        //變數設置使變數取得子物件
        RD.drag = 0.5f;
        //設定空氣阻力
        RD.gravityScale = 0.01f;
        //設定重力大小
	}
    
	// Update is called once per frame
    //物理效果必使用fixedupdate函式，固定每次執行週期，不會因電腦效能而有差異造成不正常的顯示
	void FixedUpdate () //此函式會一直重複執行
    {
        if (Time.time - LastTime > UpdateRate && seeker.IsDone())
        {
            LastTime = Time.time + Random.value * UpdateRate * 0.5f;
            seeker.StartPath(transform.position, Target.position, PathCorrect);
            //seeker 的函數，開始計算路徑，並且傳回路徑到PathCorrect
        }
        //Time.time為遊戲執行的時間，Random.value相當於Random.NextDouble
        //Time.time - LastTime = { Time.time(末) - Time.time (初) }- Random.value * UpdateRate * 0.5f
        //故 Time.time(末) - Time.time (初) > UpdateRate * ( 1 + Random.value * 0.5f)
        // 意即 要過了時間為 更新速率 * (1~1.5) 秒 才會再次更新路徑(使AI不會緊緊跟隨)
        if(Target == null)
        {
            return;
        }
        if(path == null)
        {
            return;
        }
        //排除target和path都是空的例外狀況，跳出啥都不做
        if(NextPoint > path.vectorPath.Count)
        {
            return;
        }
        if(NextPoint == path.vectorPath.Count)
        {
            ++NextPoint;
            return;
        }
        //NextPoint大於等於現在計算的點則跳出，不繼續執行(以免跑出計算點之外)
        
        Vector2 direction = (path.vectorPath[NextPoint] - transform.position).normalized;
        // AI要前進的方向
        //path.vectorPath[CurrentPath]為現在應該前進的方向，transform.position為自己的位置
        direction *= AISpeed;
        //乘上AI速度
        RD.AddForce(direction*2,FM);
        //施加力以及力的模式
        float closedistance = Vector2.Distance(Target.position, transform.position);
        if (closedistance < LimitDistance)
        {
            ++NextPoint;
            return;
        }
        //判斷當自己和目標位置差，若夠接近則增加NextPoint標籤，並跳出
        //使得目標要夠接近才會促使AI靠近，避免整場AI一直跟隨
	}

    public void PathCorrect(Path path)
    {
        if (!path.error)
        {
            this.path = path;
            NextPoint = 0;
            //若傳回來之path沒有錯誤，就可塞進path變數中，並且重置下個目標點之標籤為0，來走新路徑
        }
    }
}
