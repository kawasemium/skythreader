using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRotate : ObstacleBase
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        RandomAngle();
    }

    // Update is called once per frame
    void Update()
    {
        Flowing();
        Rotating();
    }
}
