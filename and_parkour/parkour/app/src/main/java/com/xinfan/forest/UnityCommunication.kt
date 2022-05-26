package com.xinfan.forest

import android.content.Context
import android.util.Log
import android.widget.Toast
import com.alibaba.fastjson.JSON
import com.unity3d.player.UnityPlayer

object UnityCommunication {

    fun ShowUnityDebug() {
        UnityPlayer.UnitySendMessage("Reporter", "doShow", "")
    }

    fun DebLog(msg: String?) {
        SendUnityDebugLog(msg)
        Log.i(MainActivity.TAG, msg!!)
    }

    fun DebLogError(msg: String?) {
        SendUnityDebugLog(msg)
        Log.e(MainActivity.TAG, msg!!)
    }

    /**
     * 展示 Toast
     *
     * @param context
     * @param msg
     */
    fun ShowToast(context: Context?, msg: String?) {
        val ts = Toast.makeText(context, msg, Toast.LENGTH_LONG)
        ts.show()
    }

    /**
     * 安卓 - Unity 消息
     *
     * @param msg
     * @param functionName
     */
    private fun SendUnity(msg: String?, functionName: String) {
        UnityPlayer.UnitySendMessage("UnityObject", functionName, msg)
    }

    /**
     * 安卓Debug消息
     *
     * @param msg
     */
    fun SendUnityDebugLog(msg: String?) {
        SendUnity(msg, "AndroidDebugEvent")
    }

    /**
     * 魔镜完整骨骼信息
     *
     * @param msg
     */
    fun SendUnityBodynode(msg: String?) {
        SendUnity(msg, "AndroidBodynodeEvent")
    }

    /**
     * 魔镜动作检测
     */
    fun SendUnityAction(
        actionId: Int,
        actionType: String?,
        personId: Int,
        isFinished: Boolean,
        code: Int,
        message: String?,
        extra: String?
    ) {
        val msgData = SdkActionMsgData()
        msgData.actionId = actionId
        msgData.actionType = actionType ?: ""
        msgData.personId = personId
        msgData.isFinished = isFinished
        msgData.code = code
        msgData.message = message ?: ""
        msgData.extra = extra ?: ""
        val msg: String = JSON.toJSONString(msgData)
        SendUnity(msg, "AndroidActionEvent")
    }

    fun SendUnityIsOpenBoxLine() {
        SendUnity("", "IsOpenBoxLine")
    }

    fun SendUnityIsRightHand() {
        SendUnity("", "IsRightHand")
    }

    fun SendUnityIsRightHand(msg: Float) {
        SendUnity(msg.toString(), "IsRightHand")
    }

//    fun AddPoseVal(score: Float) {
//        Log.i(MainActivity.TAG, "Android - 发送姿势匹配度，${score}");
//        SendUnity(score.toString(), "AddPoseVal")
//    }


    fun AddPoseVal(actionId : Int,score: Float) {
        Log.i(MainActivity.TAG, "Android - 发送姿势匹配度，actionId = ${actionId} , score = ${score}");

        var msgData = SdkActionMatchMsgData()
        msgData.actionId = actionId
        msgData.grade = score
        var msg: String = JSON.toJSONString(msgData)

        SendUnity(msg, "AddPoseVal")
    }


    fun GameAction(action: String?, grade: Int) {
        val msgData = GameActionVo()

        if (action != null) msgData.action = action
        msgData.grade = grade

        val msg: String = JSON.toJSONString(msgData)

        Log.i(MainActivity.TAG, "Android - 接收到通用动作匹配，${msg}");
        SendUnity(msg, "GameAction")
    }


    fun SendBotSdkOrder(order: String) {
        Log.i(MainActivity.TAG, "Android - 发送语音识别，${order}");
        SendUnity(order, "BotSdkOrder")
    }

    fun PauseGame() {
        SendUnity("", "PauseGame")
    }

    fun ContinueGame() {
        SendUnity("", "ContinueGame")
    }

    fun AllCalorie(string: String?) {
        SendUnity(string, "AllCalorie")
    }

    fun GameRankingList(string: String?) {
        SendUnity(string, "GameRankingList")
    }

    fun sendNotifyDownloadOk() {
        SendUnity("", "sendNotifyDownloadOk")
    }

    fun sendClientWidth(width: String) {
        SendUnity(width, "sendClientWidth")
    }


//    /**
//     * 更新头框数据
//     *
//     * @param val
//     */
//    fun SendHeadData(`val`: Float) {
//        SendUnity(`val`.toString(), "AndroidHeadDataUpdateEvent")
//    }


}