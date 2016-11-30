using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (PolygonCollider2D))]
[RequireComponent (typeof (EdgeCollider2D))]
//要求物件上要有Rigidbody2D及Collider2D的子物件(在unity稱作component)，如果物件沒有unity會自動增加這兩種子物件。
public class move : MonoBehaviour {

    public float moveSpeed = 2.5f;
    //移動速度
    private PolygonCollider2D[] other;
    //"其他東西的"碰撞器子物件
    private Rigidbody2D Gliding ;
    private EdgeCollider2D Trigger;
    //"物件本身的"剛體以及碰撞子物件
    [HideInInspector]
    //[HideInInspector]以下宣告的變數無法在InInspector直接修改，但可由其他函數取得
    public float JumpHeight = 250f;
    //跳躍高度，希望可由其他函數取得
    public int collision;
    //判斷物件是否在地上。
    
    //unity執行一開始會優先選擇執行的函式為awake()，不論這個函式的位置，執行一次
    void Awake()
    {
        Gliding = GetComponent<Rigidbody2D>();
        Trigger = GetComponent<EdgeCollider2D>();
        //設定變數取得自己的子物件
        Trigger.isTrigger = true;
        int count = GameObject.FindGameObjectsWithTag("ground").Length;
        other = new PolygonCollider2D[count];
        for(int i = 0; i<other.Length;++i)
        {
            other[i] = GameObject.FindGameObjectsWithTag("ground")[i].GetComponent<PolygonCollider2D>();
            //設定具有標籤"ground"的物件其子物件collider2D為other
        }
    }
    //物理效果必使用fixedupdate函式，固定每次執行週期，不會因電腦效能而有差異造成不正常的顯示
    void FixedUpdate()
    {
        collision = 0;
        //初始值為0，使其為無窮迴圈效果。
        foreach (PolygonCollider2D i in other)
        {
            if (Trigger.IsTouching(i) == true)
            {
                ++collision;
            }
        }
        //物件本身有和標籤為ground物體接觸則collision +1
        if (Input.GetKey(KeyCode.Space) && collision > 0)
        {
            Gliding.velocity = new Vector2(0, JumpHeight) * Time.fixedDeltaTime;
        }
        //條件為按下空白鍵 加上物體和具有other的物件有接觸才能跳躍
        //限制物體不能在空中連續跳
        //按下空白鍵時給予向上之向量，並以每單位時間跑過禎量計算
        
        float distance = moveSpeed * Time.fixedDeltaTime * Input.GetAxis("Horizontal");
        //偵測到輸入平移鍵盤指令或搖桿指令，移動距離 = 速度 * 每一固定時間內跑過禎(frame)量 * 輸入鍵盤訊號(true=1 false=0)
        transform.Translate(Vector2.right * distance);
        //移動物體

        Gliding.drag = 0;
        //初始值為0，使其為無窮迴圈效果。
        if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Gliding.velocity.y < 0f)
        {
            Gliding.drag = 8; //滑翔下降阻力
        }
        //按下shift則空氣阻力變為8f(只有下降時才有，否則不叫做滑翔)
        
    }
}
