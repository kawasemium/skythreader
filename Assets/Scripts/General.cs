using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//変数は最初小文字、関数やクラスは最初大文字で統一

public class General:MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tex; 
    GameObject bird;
    GameObject player;
    Player playerScript;
    Camera ca;
    float[] cameraScreen;//擬似トンネルの横幅, 高さ, 深さ(far/100)
                         //x, yは中央が0になる
    float createObstacleInterval;
    float createObstacleWaiting;

    [SerializeField] float maxGameSpeed;
    [SerializeField] float minGameSpeed;
    [SerializeField] float speed;
    //float minSpeed;
    int level;//1~5
    int maxLevel;
    float startTime;
    float levelInterval;//levelup(s)
    bool playAble;
    int result;

    void Start()
    {
        bird=GameObject.Find("bird");
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        ca = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraScreen = new float[3];
        createObstacleInterval = 1;//s
        createObstacleWaiting = 0;

        maxGameSpeed=16.0f;
        minGameSpeed=10.0f;
        speed=minGameSpeed;
        level=1;
        maxLevel=5;
        startTime=Time.time;
        levelInterval=20.0f;
        playAble=true;
        result=0;

        CalcCrossSection();
    }
    void Update()
    {
        CalcInterval();

        if(!playAble){
            speed-=0.2f;
            if(speed<0)speed=0;
            return;
        }

        tex.text=
        "Level: "+level+" /"+maxLevel;

        createObstacleWaiting += Time.deltaTime;
        if (createObstacleWaiting >= createObstacleInterval)
        {
            string name;
            int ran=Random.Range(0,10);//0~9
            if(ran<4)name="ObstacleRotate";
            else name="ObstacleNomotion";

            CreateObstacle(name);
            createObstacleWaiting -= createObstacleInterval;
        }

        if(Time.time-startTime>levelInterval*level && level<maxLevel)LevelUp();


        //Delete this
        if(Input.GetKeyDown(KeyCode.Q))LevelUp();
    }
    void LevelUp(){
        level++;
    }
    void CalcCrossSection()
    {
        float fov = ca.fieldOfView;
        cameraScreen[0] = 100*fov/3;
        cameraScreen[1] = 100*fov/3/2;
        cameraScreen[2] = ca.farClipPlane;
    }
    void CalcInterval()
    {
        if(speed>=17){
            Debug.Log("speed >= 17");
            return;
        }
        createObstacleInterval=(17-speed)/15;
        //sp=-15interval+18
    }
    void CreateObstacle(string name)
    {
        GameObject gb = Resources.Load(name) as GameObject;
        float[] mg={100.0f,500.0f};
        float x = Random.Range(-(cameraScreen[0]/2+mg[0]),cameraScreen[0]/2+mg[0]+1);
        float y = Random.Range(-(cameraScreen[1]/2+mg[1]),cameraScreen[1]/2+mg[1]+1);
        float z = cameraScreen[2]+500;
        Vector3 p = new Vector3(x, y, z);
        GameObject pf=Instantiate(gb);
        pf.transform.position = p;
        pf.transform.localScale*=Random.Range(1,level+1);
    }
    public float Speed
    {
        get{
            return speed;
        }
        set
        {

            float s=speed+value;
            if(s>=minGameSpeed && s<=maxGameSpeed){
                speed += value;
            }
            if(Mathf.Abs(maxGameSpeed-speed)<1)speed=maxGameSpeed;
            if(Mathf.Abs(minGameSpeed-speed)<1)speed=minGameSpeed;
        }
    }
    public float MinGameSpeed{
            get{
                return minGameSpeed;
            }
    }

    public bool PlayAble{
        get{
            return playAble;
        }
    }
    public void GameOver(){
        playAble=false;
        var birdRigid=bird.GetComponent<Rigidbody>();
        birdRigid.useGravity=true;
        birdRigid.AddTorque(Vector3.right*80.0f);
        birdRigid.AddTorque(Vector3.back*500.0f);
        result=(int)Time.time-(int)startTime;
        result*=100;

        tex.text=
        "Level: "+level+" /"+maxLevel+"\n"+
        "Score: "+result;
    }
}


public class ObstacleBase : MonoBehaviour
{
    protected GameObject bird;
    protected Player playerScript;
    protected General general;
    protected float flowSpeed;
    protected float backScreen;

    protected float rotateSpeed;
    protected Vector3 angle;

    protected void Start()
    {
        bird=GameObject.Find("bird");
        playerScript=GameObject.Find("Player").GetComponent<Player>();
        general=GameObject.Find("General").GetComponent<General>();
        backScreen = GameObject.Find("Main Camera").transform.position.z;
        flowSpeed = general.Speed;

        rotateSpeed=Random.Range(30,50);
        angle=transform.localEulerAngles;
    }

    protected void Flowing()
    {
        flowSpeed= general.Speed;
        float[] deg=playerScript.Degree;
        float handle=0.8f;

        Vector3 p=transform.position;
        p.x-=deg[0]*handle;
        p.y-=deg[1]*handle;
        p.z-=flowSpeed;
        transform.position=p;

        if (gameObject.transform.position.z < backScreen)
        {
            DestroyMe();
        }
    }
    protected void RandomAngle(){
        Vector3 ang=Vector3.zero;
        ang.x=Random.Range(0,361);
        ang.y=Random.Range(0,361);
        ang.z=Random.Range(0,361);
        transform.eulerAngles=ang;
        angle=ang;
    }

    protected void Rotating()
    {
        angle.x+=rotateSpeed*Time.deltaTime;
        transform.eulerAngles=angle;
    }


    protected void DestroyMe()
    {
        Destroy(gameObject);
    }
}