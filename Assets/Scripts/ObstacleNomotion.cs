/*========================================

    public class ObstacleNomotion
        不動の立方体の障害物に持たせるクラスです。

========================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleNomotion : ObstacleBase
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
    }
}
