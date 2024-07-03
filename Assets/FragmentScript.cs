using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentScript : MonoBehaviour
{
    float maxsc=3.0f;
    float minsc=1.0f;
    float maxforce = 300.0f;//絶対値指定
    float maxtorque = 300.0f;

    Vector3 force;
    Vector3 torque;
    float sc;
    bool setok = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!setok) return;
        transform.localScale *= 0.998f;
        if (transform.localScale.x < 0.001f)
        {
            Destroy(gameObject);
        }

    }
    public void setting(GameObject gb)
    {
        Material mtr = gb.GetComponent<Renderer>().material;
        GetComponent<Renderer>().material = mtr;
        var rb = GetComponent<Rigidbody>();

        //初速度、サイズのばらつき
        sc = Random.value * (maxsc-minsc)+minsc ;
        force = new Vector3(Random.value, Random.value, Random.value);
        torque = new Vector3(Random.value, Random.value, Random.value);
        force*= (Random.value*2-1) * maxforce;
        torque *= (Random.value * 2 - 1) * maxtorque;

        transform.localScale *=sc;
        rb.AddForce(force);
        rb.AddTorque(torque);

        setok = true;
    }
    public static void crash(GameObject posgb,GameObject mtrgb)
    {
        for (int i = 0; i < 15; i++)
        {
            var fragment = Resources.Load("Fragment") as GameObject;
            var frag = Instantiate(fragment);
            frag.transform.position = posgb.transform.position;
            frag.GetComponent<FragmentScript>().setting(mtrgb);
        }
    }
}
