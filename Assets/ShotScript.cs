using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    public GameObject fragment;
    float sp = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.forward*sp);

        if(Vector3.Distance(transform.position, Vector3.zero) > 30)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider cl)
    {
        if (gameObject.name == "Shot(Clone)" && cl.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);

            cl.GetComponent<EnemyCommon>().HP--;
            if (cl.GetComponent<EnemyCommon>().HP <= 0)
            {
                PlayerScript.audio[2].PlayOneShot(PlayerScript.audio[2].clip);
                FragmentScript.crash(cl.gameObject,cl.gameObject);
                Destroy(cl.gameObject);

                PlayerScript.NumOfEnemies--;
                if (PlayerScript.NumOfEnemies <= 0) PlayerScript.clear();
            }
            else
            {
                PlayerScript.audio[1].PlayOneShot(PlayerScript.audio[1].clip);
            }
        }

        if (gameObject.name == "EnemyShot(Clone)" && cl.gameObject.tag == "Player")
        {
            PlayerScript.audio[0].PlayOneShot(PlayerScript.audio[0].clip);
            Destroy(gameObject);
            FragmentScript.crash(GameObject.Find("Player"),GameObject.Find("Body"));
            Destroy(GameObject.Find("Body"));
            PlayerScript.gameover();
        }
    }
}
