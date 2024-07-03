using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommon : MonoBehaviour
{
    public int maxHP;
    public string EnemyType;
    public float delay;//Flexだとsc_yの意味 Lookmeはそのまま


    public int HP;
    delegate void myfunc();
    myfunc setting;
    myfunc updating;

    //Radiation, Lookme
    GameObject Shot;
    GameObject EnemyShot;
    float shotwait;
    float shotwaiting;

    //Flex
    float[] sc_y = { 3.0f, 6.0f };//min, max
    float flexsp = 0.002f;
    bool isexpand;

    //Lookme
    GameObject pl;
    GameObject bd;
    float shotstart;//一発目のtime
    float rapidwaiting = 0.1f;//連続で撃つ間隔
    int rapid = 3;//連続で撃ってくる数
    int bullet = 0;//残弾数

    // Start is called before the first frame update
    void Start()
    {
        this.tag = "Enemy";
        HP = maxHP;
        pl = GameObject.Find("Player");
        bd = GameObject.Find("Body");
        Shot = Resources.Load("Shot") as GameObject;
        EnemyShot = Resources.Load("EnemyShot") as GameObject;

        switch (EnemyType)
        {
            case "Flex":
                setting = setFlex;
                updating = updFlex;
                break;
            case "Radiation":
                setting=setRadiation;
                updating = updRadiation;
                break;
            case "Lookme":
                setting = setLookme;
                updating = updLookme;
                break;
            default:
                break;
        }
        if (setting!=null) setting();
    }

    // Update is called once per frame
    void Update()
    {
        if(updating!=null)updating();
    }


    public void damaged(GameObject cl)
    {
    }

    //Flex
    void setFlex(){
        var sc = transform.localScale;
        sc.y = sc_y[0] + delay;
        transform.localScale = sc;
        isexpand = true;
    }
    void updFlex()
    {
        var sc = transform.localScale;

        if (sc.y <= sc_y[0]) isexpand = true;
        if (sc.y >= sc_y[1]) isexpand = false;

        if (isexpand) sc.y += flexsp;
        else sc.y -= flexsp;

        transform.localScale = sc;
    }

    //Radiation
    void setRadiation()
    {
        shotwait = 1.5f;
        shotwaiting = 0.0f;
    }
    void updRadiation()
    {
        if (!GameObject.Find("Body")) return;

        shotwaiting -= Time.deltaTime;
        if (shotwaiting < 0)
        {
            Vector3 ang = transform.eulerAngles;
            ang.x += 90;
            for (int i = 0; i < 16; i++)
            {
                Instantiate(EnemyShot, transform.position, Quaternion.Euler(ang));
                ang.y += 22.5f;
            }

            shotwaiting = shotwait;
        }
    }

    //Lookme
    void setLookme()
    {
        shotwait = 1.0f;
        shotwaiting = delay;
    }
    void updLookme()
    {
        if (!bd) return;
        var pldrc = pl.transform.position;
        pldrc.y = gameObject.transform.position.y;
        transform.LookAt(pldrc);

        shotwaiting -= Time.deltaTime;
        if (shotwaiting < 0)
        {
            bullet = rapid;
            Lookme_shoot();
            shotstart = Time.time;
            shotwaiting = shotwait;
        }
        if (bullet > 0 && Time.time - shotstart > rapidwaiting * (rapid - bullet))
        {
            Lookme_shoot();
        }
    }
    void Lookme_shoot()
    {
        bullet--;
        Vector3 ang = transform.eulerAngles;
        ang.x += 90;
        Instantiate(EnemyShot, transform.position, Quaternion.Euler(ang));
    }
}
