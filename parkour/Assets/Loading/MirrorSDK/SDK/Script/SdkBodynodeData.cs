
using System.Collections.Generic;
using UnityEngine;

public class Point {
    public float x {
        get; set;
    }
    public float y {
        get; set;
    }

    public Vector2 ToV2() {
        return new Vector2(x, y);
    }

}


public class Head {
    public Point leftTop {
        get; set;
    }
    public Point rightBottom {
        get; set;
    }
}

public class Skeletons {
    /// <summary>
    /// 鼻子
    /// </summary>
    public Point nose {
        get; set;
    }
    /// <summary>
    /// 左眼
    /// </summary>
    public Point leftEye {
        get; set;
    }
    /// <summary>
    /// 右眼
    /// </summary>
    public Point rightEye {
        get; set;
    }
    /// <summary>
    /// 左耳
    /// </summary>
    public Point leftEar {
        get; set;
    }
    /// <summary>
    /// 右耳
    /// </summary>
    public Point rightEar {
        get; set;
    }


    /// <summary>
    /// 左肩
    /// </summary>
    public Point leftShoulder {
        get; set;
    }
    /// <summary>
    /// 右肩
    /// </summary>
    public Point rightShoulder {
        get; set;
    }
    /// <summary>
    /// 左肘
    /// </summary>
    public Point leftElbow {
        get; set;
    }
    /// <summary>
    /// 右肘
    /// </summary>
    public Point rightElbow {
        get; set;
    }
    /// <summary>
    /// 左手腕
    /// </summary>
    public Point leftWrist {
        get; set;
    }
    /// <summary>
    /// 右手腕
    /// </summary>
    public Point rightWrist {
        get; set;
    }


    /// <summary>
    /// 左髋
    /// </summary>
    public Point leftHip {
        get; set;
    }
    /// <summary>
    /// 右髋
    /// </summary>
    public Point rightHip {
        get; set;
    }
    /// <summary>
    /// 左膝盖
    /// </summary>
    public Point leftKnee {
        get; set;
    }
    /// <summary>
    /// 右膝盖
    /// </summary>
    public Point rightKnee {
        get; set;
    }
    /// <summary>
    /// 左脚踝
    /// </summary>
    public Point leftAnkle {
        get; set;
    }
    /// <summary>
    /// 右脚踝
    /// </summary>
    public Point rightAnkle {
        get; set;
    }
    public Point unUsed18 {
        get; set;
    }
    public Point unUsed19 {
        get; set;
    }
}

public class Persons {
    /// <summary>
    /// 人Id
    /// </summary>
    public int personId {
        get; set;
    }
    /// <summary>
    /// 状态
    /// </summary>
    public float state {
        get; set;
    }
    /// <summary>
    /// 距离
    /// </summary>
    public float distance {
        get; set;
    }

    /// <summary>
    /// 头
    /// </summary>
    public Head head {
        get; set;
    }

    /// <summary>
    /// 骨架
    /// </summary>
    public Skeletons skeletons {
        get; set;
    }

    /// <summary>
    /// 左手
    /// </summary>
    public List<Point> leftHand {
        get; set;
    }

    /// <summary>
    /// 右手
    /// </summary>
    public List<Point> rightHand {
        get; set;
    }
}

public class SdkBodynodeData {
    /// <summary>
    /// 人ID
    /// </summary>
    public int targetPersonId {
        get; set;
    }
    /// <summary>
    /// 帧ID
    /// </summary>
    public int frameId {
        get; set;
    }
    /// <summary>
    /// x3 时间戳
    /// </summary>
    public ulong x3TimeStamp {
        get; set;
    }
    /// <summary>
    /// jni 时间戳
    /// </summary>
    public ulong jniTimeStamp {
        get; set;
    }
    /// <summary>
    /// sdk 时间消耗，感觉是 sdk 的运行时长
    /// </summary>
    public float sdkTimeConsume {
        get; set;
    }
    /// <summary>
    /// 识别到的人
    /// </summary>
    public List<Persons> persons {
        get; set;
    }
}