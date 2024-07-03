/*========================================

    static class Config
        ゲームバランスの設定を担当します。

    static class Records
        操作説明を表示したことがあるかどうかを保持します。
        (Rキーでリスタートした時は操作説明が表示されない)

    public class General
        空のゲームオブジェクトに持たせたクラスで、
        ゲームに必要な大方の処理がこちらにあります。
        スピード調整・加速、スコア計算・表示、
        レベル管理、障害物の生成・生成範囲の調節、
        ゲームオーバー処理などを行っています。

    public class ObstacleBase
        全ての障害物に共通で持たせたい機能をまとめたクラスです。
        これだけでは動かないため、継承先で動きを指定して動かします。
        障害物自身がプレイヤーの背後へ動いていくことで
        疑似的にプレイヤーの前進を表現しています。

========================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


//ゲームバランスの設定
static class Config{
    static float turnSpeed=0.8f;//プレイヤーの曲がりやすさ ~1.0
    static float accelBy=0.5f;//加速の度合い
    static float maxGameSpeed=70.0f;//加速時の進む速度
    static float minGameSpeed=24.0f;//普段の進む速度
    static float maxSpeedBonusRate=5.0f;//最高速時のスコアボーナス倍率
    static float maxObstacleInterval=0.22f;//障害物が生成される間隔
    static float minObstacleInterval=0.18f;
    static float maxObstacleScale=4.1f;//最も大きい障害物のスケール
    static int maxLevel=5;//最大レベル
    static float levelInterval=15.0f;//レベルが上がるまでの秒数

    public static float TurnSpeed
    {
        get{
            return turnSpeed;
        }
    }
    public static float AccelBy
    {
        get{
            return accelBy;
        }
    }
    public static float MaxGameSpeed
    {
        get{
            return maxGameSpeed;
        }
    }
    public static float MinGameSpeed
    {
        get{
            return minGameSpeed;
        }
    }
    public static float MaxSpeedBonusRate
    {
        get{
            return maxSpeedBonusRate;
        }
    }
    public static float MaxObstacleInterval
    {
        get{
            return maxObstacleInterval;
        }
    }
    public static float MinObstacleInterval
    {
        get{
            return minObstacleInterval;
        }
    }
    public static float MaxObstacleScale
    {
        get{
            return maxObstacleScale;
        }
    }
    public static int MaxLevel
    {
        get{
            return maxLevel;
        }
    }
    public static float LevelInterval
    {
        get{
            return levelInterval;
        }
    }
}
static class Records{
    public static bool first=false;
}


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

    [SerializeField] float speed;
    int level;
    int score;
    float sceneTime;
    bool playAble;
    bool tutorial;

    void Start()
    {
        bird=GameObject.Find("bird");
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        ca = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraScreen = new float[3];
        createObstacleInterval = 1;//s
        createObstacleWaiting = 0;

        speed=Config.MinGameSpeed;
        level=1;
        score=0;
        sceneTime=0;
        CalcCrossSection();

        if(Records.first){
            playAble=true;
            return;
        }
        tutorial=true;
        tex.text="W/A/S/D to move\nSpace to accelarate(+bonus)\n\nOK? [Space]";
        Records.first=false;

    }
    void Update()
    {
        //チュートリアル中
        if(tutorial){
            if(Input.GetKey(KeyCode.Space)){
                tutorial=false;
                playAble=true;
            }
            return;
        }
        
        //障害物生成間隔の調整
        float mitv=Config.MinObstacleInterval,Mitv=Config.MaxObstacleInterval;
        int Mle=Config.MaxLevel;
        createObstacleInterval=mitv+(Mitv-mitv)/Mle*(Mle-level);
        float baisoku=speed/Config.MinGameSpeed;
        createObstacleInterval*=(1/baisoku);

        //ゲームオーバー時処理
        if (!playAble){
            if(Input.GetKey(KeyCode.R))SceneManager.LoadScene("Scene1");

            if(speed==0)return;
        }

        //障害物生成
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
        
        //やられてから完全に止まるまで
        if(!playAble){
            if(speed>Config.MinGameSpeed)speed*=0.75f;
            else speed-=0.2f;
            if(speed<0)speed=0;
            return;
        }

        //レベルアップ
        sceneTime+=Time.deltaTime;
        if(sceneTime>Config.LevelInterval*level){
            LevelUp();
        }

        //加速
        if (Input.GetKey(KeyCode.Space))Speed=Config.AccelBy;
        else Speed=-1.0f;

        //テキスト更新
        string text="Level: "+level+" /"+Config.MaxLevel+"\nScore: ";
        int plus=(int)(Time.deltaTime*100);
        float min=Config.MinGameSpeed;
        if(speed>min){
            float rate=(speed-min)/(Config.MaxGameSpeed-min);
            float mag=(Config.MaxSpeedBonusRate-1.0f)*rate+1.0f;
            mag=(int)(mag*10);
            mag=(float)(mag/10);
            plus=(int)(plus*mag);
            score+=plus;
            text+=score+" x"+mag;
        }else{
            score+=plus;
            text+=score;
        }

        tex.text=text;

    }
    void LevelUp(){
        if(level==Config.MaxLevel)return;
        level++;
    }
    void CalcCrossSection()
    {
        float fov = ca.fieldOfView;
        cameraScreen[0] = 100*fov/3;
        cameraScreen[1] = 100*fov/3/2;
        cameraScreen[2] = ca.farClipPlane;
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

        float ms=Config.MaxObstacleScale;
        float mins=(float)(level-1)/(float)Config.MaxLevel*ms;
        if(mins==1)mins=1;
        float s=(Random.value*((ms-mins)*((float)level/(float)Config.MaxLevel)))+mins;
        //Debug.Log(s);
        pf.transform.localScale*=s;
    }
    
    public float Speed
    {
        get{
            return speed;
        }
        set
        {
            float max=Config.MaxGameSpeed;
            float min=Config.MinGameSpeed;

            float s=speed+value;
            if(s>max)s=max;
            if(s<min)s=min;

            speed=s;
        }
    }

    public bool PlayAble{
        get{
            return playAble;
        }
    }
    public void GameOver(){
        playAble=false;
        Records.first=true;
        var birdRigid=bird.GetComponent<Rigidbody>();
        birdRigid.useGravity=true;
        birdRigid.AddTorque(Vector3.right*80.0f);
        birdRigid.AddTorque(Vector3.back*500.0f);
        tex.text="Level: "+level+" /"+Config.MaxLevel+"\nScore: "+score+"\n\n[R] Try Again?";
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

        rotateSpeed=Random.Range(40,75);
        angle=transform.localEulerAngles;
    }

    protected void Flowing()
    {
        flowSpeed= general.Speed;
        float[] deg=playerScript.Degree;

        Vector3 p=transform.position;
        p.x-=deg[0]*Config.TurnSpeed;
        p.y-=deg[1]*Config.TurnSpeed;
        p.z-=flowSpeed;
        transform.position=p;

        if (gameObject.transform.position.z < backScreen)
        {
            Destroy(gameObject);
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
}