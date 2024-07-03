using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject bird;
    Animator animator;
    General general;

    float verticalDegree;//-left right+
    float horizontalDegree;//+up
    [SerializeField] float maxDegree;
    float decayDegree;

    bool isFlapping;
    [SerializeField] bool isInvincible;

    void Start()
    {
        //Timer timer = new Timer();
        bird=GameObject.Find("bird");
        animator=bird.GetComponent<Animator>();
        general=GameObject.Find("General").GetComponent<General>();

        verticalDegree=1.0f;
        horizontalDegree=1.0f;
        maxDegree=5.0f;
        decayDegree=0.2f;
        isFlapping=false;
        isInvincible=false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!general.PlayAble)return;

        if(Mathf.Abs(horizontalDegree)<0)horizontalDegree=0.0f;
        if(Mathf.Abs(verticalDegree)<0)verticalDegree=0.0f;

        horizontalDegree=MakeDecay(horizontalDegree);
        verticalDegree=MakeDecay(verticalDegree);

        if(Input.GetKey(KeyCode.W))horizontalDegree++;
        if (Input.GetKey(KeyCode.S))horizontalDegree--;
        if (Input.GetKey(KeyCode.D))verticalDegree++;
        if (Input.GetKey(KeyCode.A))verticalDegree--;

        if (Input.GetKey(KeyCode.LeftShift))general.Speed=1.0f;
        else general.Speed=-1.0f;
        
        if(horizontalDegree>maxDegree)horizontalDegree=maxDegree;
        if(horizontalDegree<-maxDegree)horizontalDegree=-maxDegree;
        if(verticalDegree>maxDegree)verticalDegree=maxDegree;
        if(verticalDegree<-maxDegree)verticalDegree=-maxDegree;

        AnimeControl();
        Tilt();
    }
    float MakeDecay(float deg){
        if(Mathf.Abs(deg)<1)deg=0.0f;

        if(deg>0)deg-=decayDegree;
        if(deg<0)deg+=decayDegree;
        return deg;
    }
    void Tilt(){
        Vector3 ang=Vector3.zero;
        ang.y=180+verticalDegree;
        ang.x=horizontalDegree;
        bird.transform.localEulerAngles=ang;
    }
    void AnimeControl(){
        bool josho=false;
        if(horizontalDegree>0)josho=true;
        bool accel=false;
        if(general.Speed>general.MinGameSpeed)accel=true;

        if((josho || accel)&& !isFlapping){
            animator.CrossFade("Flapping|Action",0,-1);
            isFlapping=true;
        }else if(!josho && !accel && isFlapping){
            animator.CrossFade("円柱Action",0.1f,-1);
            isFlapping=false;
        }
    }
    public float[] Degree{
        get{
            float[] val={verticalDegree,horizontalDegree};
            return val;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(isInvincible)return;

        if(col.gameObject.tag=="Enemy"){
            verticalDegree=0;
            horizontalDegree=0;
            general.GameOver();
        }
    }
}