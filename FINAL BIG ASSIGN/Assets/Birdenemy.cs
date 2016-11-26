using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof (Rigidbody2D) )]
[RequireComponent(typeof(Seeker))]

public class Birdenemy : MonoBehaviour {
    //可以飛行，在空中對玩家施以攻擊，偵測玩家位置，並且衝過去撞擊玩家
    //衝刺時有1秒的緩衝時間，玩家受到撞擊時(兩撞擊器碰在一起觸發)會受到HP-2的傷害
    private Rigidbody2D ATTACKBIRD;
    private Seeker CK;
    private Path path;
    public float Updateratio = 2f;
    public float speed = 300;
    public Transform target;
    public ForceMode2D FM;
    [HideInInspector]
    public bool pathend = false;
    public float nextwaypointdistance = 3;
    private int currentpathway = 0;
    private bool search = false;

    void start()
    {
        CK = GetComponent<Seeker>();
        ATTACKBIRD = GetComponent<Rigidbody2D>();
        if (target == null)
        {
            if(!search)
            {
                search = true;
                StartCoroutine(searchplayer());
            }
            return;
        }
        CK.StartPath(transform.position,target.position,OnPathComplete);

        StartCoroutine(UpdatePath());
    }
    IEnumerator searchplayer()
    {
        GameObject SResult = GameObject.FindGameObjectWithTag("player");
        if(SResult == null)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(searchplayer());
        }
        else
        {
            search = false;
            target = SResult.transform;
            StartCoroutine(UpdatePath());
        }
    }
    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            //TODO: Insert a player search here.
            yield break;
        }
        CK.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f/Updateratio);
        StartCoroutine(UpdatePath());
    }
    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentpathway = 0;
        }
    }
    void FixedUpdate()
    {
        if (target == null)
        {
            if (!search)
            {
                search = true;
                StartCoroutine(searchplayer());
            }
        }
        if (path == null)
        {
            return;
        }
        if(currentpathway >= path.vectorPath.Count)
        {
            if (pathend)
                return;
            pathend = true;
            return;
        }
        else
        {
            pathend = false;
        }
        //方向
        Vector3 dir = (path.vectorPath[currentpathway]-transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        ATTACKBIRD.AddForce(dir, FM);
        float distclose = Vector3.Distance(path.vectorPath[currentpathway], transform.position);
        if (distclose < nextwaypointdistance)
        {
            ++currentpathway;
            return;
        }
    }
	
}
