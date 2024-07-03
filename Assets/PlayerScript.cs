using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    GameObject Pl;
    GameObject Shot;
    GameObject Body;
    GameObject Motor;
    GameObject MCamera;

    public static new AudioSource[] audio;
    public static int NumOfEnemies = 12;

    float yokosp = 0.1f;
    float tatesp = 0.01f;
    float motsp = -0.8f;
    float shotwait = 0.0f;//リセット値はvoid shoot
    float maxy = 11.0f;
    float miny = 1.0f;
    Tweener shaket;

    // Start is called before the first frame update
    void Start()
    {
        Pl = GameObject.Find("Player");
        Shot = Resources.Load("Shot")as GameObject;
        Body = GameObject.Find("Body");
        Motor = GameObject.Find("Motor");
        MCamera = GameObject.Find("Main Camera");

        audio = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("Body"))return;

        shotwait -= Time.deltaTime;
        if (shotwait < 0) shotwait = 0.0f;

        if (Input.GetKey(KeyCode.A)) transform.RotateAround(Vector3.zero, Vector3.up, yokosp);
        if (Input.GetKey(KeyCode.D)) transform.RotateAround(Vector3.zero, Vector3.up, -yokosp);
        if (Input.GetKey(KeyCode.W)&&Pl.transform.position.y<maxy) transform.Translate(new Vector3(0, 0, -tatesp));
        if (Input.GetKey(KeyCode.S)&&Pl.transform.position.y>miny) transform.Translate(new Vector3(0, 0, tatesp));
        if (Input.GetKey(KeyCode.Space) && shotwait <= 0) shoot();

        Vector3 ve = Motor.transform.localEulerAngles;
        Motor.transform.localEulerAngles = new Vector3(ve.x, ve.y + motsp, ve.z);

        
    }
    void shoot()
    {
        shotwait = 0.2f;
        Vector3 ang = transform.eulerAngles;
        Instantiate(Shot, transform.position, Quaternion.Euler(ang));
    }
    void shake()
    {
        float dur = 0.6f;
        float str = 0.2f;
        int vib = 30;
        int ran = 90;

        shaket = MCamera.transform.DOShakePosition(dur, str, vib, ran);

    }
    public static void clear()
    {
        GameObject.Find("Message").GetComponent<Text>().text=
            "CONGRATULATIONS!\nCLEAR TIME: " + Time.time + "s";
    }
    public static void gameover()
    {
        GameObject.Find("Message").GetComponent<Text>().text = "GAME OVER";
    }
}
