using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorSdkDataModel
{

    private static MirrorSdkDataModel instance;
    public static MirrorSdkDataModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MirrorSdkDataModel();
                instance.IsOpenUpdateData = true;
            }
            return instance;
        }
    }

    private bool isOpenUpdateData = false;
    public bool IsOpenUpdateData
    {
        get
        {
            return isOpenUpdateData;
        }
        set
        {
            isOpenUpdateData = value;
            if (isOpenUpdateData == true)
            {
                Debug.Log("开启 Json 解析");
            }
            else
            {
                bodynodeMsg = string.Empty;
                Debug.Log("关闭 Json 解析");
            }
        }
    }

    public bool isOpenBoxLine = false;
    /// <summary>
    /// 是否是右手
    /// </summary>
    public bool isRightHand = false;


    private string bodynodeMsg = string.Empty;
    private SdkBodynodeData bodynodeData = null;
    public SdkBodynodeData BodynodeData => bodynodeData;
    private float bodynodeMsgIsNullTime = 0;
    private bool bodynodeMsgIsNullTime_bool = false;



    private string actionMsg = string.Empty;
    private SdkActionData actionMsgData = null;
    public SdkActionData ActionMsgData => actionMsgData;


    public void UpdateBodynodeMsg(string bodynodeMsg)
    {
        this.bodynodeMsg = bodynodeMsg;
    }

    public void UpdateBodynodeData()
    {
        if (IsOpenUpdateData)
        {
            if (string.IsNullOrEmpty(bodynodeMsg) == false)
            {
                try
                {
                    var bodynodeData = JsonConvert.DeserializeObject<SdkBodynodeData>(bodynodeMsg);
                    this.bodynodeData = bodynodeData;
                    Debug.Log("bodynodeMsg Json 解析成功， targetPersonId " + bodynodeData.targetPersonId);
                    bodynodeMsgIsNullTime = 0;
                    bodynodeMsgIsNullTime_bool = false;
                }
                catch
                {
                    bodynodeData = null;
                    bodynodeMsgIsNullTime = 0;
                    bodynodeMsgIsNullTime_bool = true;
                    Debug.LogError("bodynodeMsg Json 解析失败，" + bodynodeMsg);
                }
            }
            else
            {
                bodynodeMsgIsNullTime += Time.deltaTime;

                if (bodynodeMsgIsNullTime > 1)
                {

                    bodynodeData = null;

                    if (bodynodeMsgIsNullTime_bool == false)
                    {
                        bodynodeMsgIsNullTime_bool = true;
                        Debug.LogWarning("bodynode 数据丢失超过 1 秒");
                    }
                }
            }
        }
        else
        {
            bodynodeMsgIsNullTime = 0;
            bodynodeMsgIsNullTime_bool = false;
        }
    }




    public void UpdateActionMsg(string actionMsg)
    {
        this.actionMsg = actionMsg;
    }

    public void UpdateActionData()
    {
        if (IsOpenUpdateData)
        {
            if (actionMsg != string.Empty)
            {
                try
                {
                    var actionMsgData = JsonConvert.DeserializeObject<SdkActionData>(actionMsg);
                    this.actionMsgData = actionMsgData;
                    Debug.Log("actionMsg Json 解析成功， actionId " + actionMsgData.actionId);
                }
                catch
                {
                    Debug.LogError("actionMsg Json 解析失败，" + actionMsg);
                }
            }
        }
    }


    private bool isCanAddPoseVal = false;
    public bool IsCanAddPoseVal
    {
        get => isCanAddPoseVal;
        set
        {
            isCanAddPoseVal = value;
            if (IsCanAddPoseVal)
            {
                PoseVales.Clear();
            }
        }
    }
    private List<SdkActionMatchData> PoseVales = new List<SdkActionMatchData>();

    //public void AddPoseVal(float val) {
    //    if (isCanAddPoseVal) {
    //        PoseVales.Add(val);
    //    }
    //}

    public void AddPoseVal(SdkActionMatchData val)
    {
        if (isCanAddPoseVal)
        {
            PoseVales.Add(val);
        }
    }






    public void ClearData()
    {
        actionMsg = string.Empty;
        bodynodeMsg = string.Empty;

        actionMsgData = null;
    }




}
