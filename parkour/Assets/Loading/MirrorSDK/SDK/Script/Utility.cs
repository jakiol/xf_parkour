using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {


    /// <summary>
    /// 反转Y轴
    /// </summary>
    /// <param name="pos"></param>
    public static Vector2 ReversalPosY(this Vector2 pos) {
        Vector2 newPos = new Vector2 {
            x = pos.x,
            y = Screen.height - pos.y
        };
        return newPos;
    }

    public static Vector2 PointToV2(this Point point) {
        Vector2 pos = new Vector2 {
            x = point.x,
            y = point.y
        };
        return pos;
    }


}
