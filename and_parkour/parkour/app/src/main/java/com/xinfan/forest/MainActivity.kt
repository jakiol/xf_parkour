package com.xinfan.forest

import android.content.Intent
import android.graphics.Color
import android.media.Image
import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.view.View
import android.view.WindowManager
import android.widget.ImageView
import androidx.appcompat.app.AppCompatActivity
import com.baidu.duer.botsdk.BotIntent
import com.baidu.duer.botsdk.BotSdk
import com.baidu.duer.botsdk.IDialogStateListener
import com.baidu.duer.botsdk.UiContextPayload
import com.unity3d.player.UnityPlayerActivity
import com.xinfan.forest.botsdk.BotInitCallBack
import com.xinfan.forest.botsdk.BotMessageListener
import com.xinfan.forest.botsdk.IBotIntentCallback
import com.xinfan.forest.botsdkutils.BotConstants
import java.util.*

class MainActivity : UnityPlayerActivity(), IBotIntentCallback, IDialogStateListener {

    companion object {
        const val TAG: String = "MainActivity"
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Log.i(MainActivity.TAG,"启动开始：" + Date());

        window.addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        AndroidTool.Init();
        initAIProxy();

        if (BotMessageListener.getInstance().IsRegisterFailedOrSucceed) {
            initBot()
        } else {
            BotMessageListener.getInstance()._BotInitCallBack = object : BotInitCallBack {
                override fun Failed() {
                    Log.i(TAG, "失败")
                }

                override fun Succeed() {
                    Log.i(TAG, "成功")
                    initBot()
                }
            }
        }
    }


    private fun initAIProxy() {
        AIProxy.initContext(this)
        AIProxy._BotInitCallBack = object : BotInitCallBack {
            override fun Failed() {
            }

            override fun Succeed() {
                Log.i(TAG, "解析 - parseModelInfo")
                parseModelInfo(intent)
            }
        }
        AIProxy.bindService();
//        AIProxy.registerBodyListener()
    }

    private fun parseModelInfo(intent: Intent) {
        val url: Uri = Uri.parse((intent.extras?.get("url") ?: "") as String);

        val type = url.getQueryParameter("type")
        val downloadUrl = url.getQueryParameter("url")
        val md5 = url.getQueryParameter("md5")
        val courseId = url.getQueryParameter("course_id")


        if (type != null) {
            AIProxy.type = type
        }
        if (downloadUrl != null) {
            AIProxy.downloadUrl = downloadUrl
        }
        if (md5 != null) {
            AIProxy.md5 = md5
        }
        if (courseId != null) {
            AIProxy.courseId = courseId
        };

        Log.e(TAG, "download type = $type")
        Log.e(TAG, "download url = $downloadUrl")
        Log.e(TAG, "download md5 = $md5")
        Log.e(TAG, "download course_id = $courseId")

    }


    fun initBot() {

        Log.i(TAG, "Android - InitBot")

        BotMessageListener.getInstance().addCallback(this)
        BotSdk.getInstance().setDialogStateListener(this)

        // 1.创建UIContextPayload对象开始组装
        val payload = UiContextPayload()

        // 2.定义用户语音意图集合
        val list_start = listOf(
            "开始",
            "开始游戏",
            "游戏开始",
            "进入游戏",
            "游戏进入",
            "重新开始",
            "重新开始游戏",
            "游戏重新开始",
            "再来一次",
            "再开始一次",
            "再开始一次游戏"
        )

        payload.addHyperUtterance(BotConstants.StartGame, list_start, null, null);

        // 2.定义用户语音意图集合？
        val list_pause = listOf("暂停", "暂停游戏", "游戏暂停", "停止", "停止游戏", "游戏停止");
//        "暂停"  "暂停游戏" "游戏暂停"
//        "停止" "停止游戏" "游戏停止"
        payload.addHyperUtterance(BotConstants.PauseGame, list_pause, null, null);

        // 2.定义用户语音意图集合
        val list_continue = listOf("继续", "继续游戏", "游戏继续", "恢复", "恢复游戏", "游戏恢复")
        payload.addHyperUtterance(BotConstants.ContinueGame, list_continue, null, null);

//        // 2.定义用户语音意图集合
//        val list_goBack = listOf("返回大厅")
//        payload.addHyperUtterance(BotConstants.GoBackGame, list_goBack, null, null);

        // 2.定义用户语音意图集合
        val list_quit = listOf("退出", "退出游戏", "游戏退出", "结束", "结束游戏", "游戏结束", "关闭", "关闭游戏", "游戏关闭")
        payload.addHyperUtterance(BotConstants.QuitGame, list_quit, null, null);

        BotSdk.getInstance().updateUiContext(payload);
    }


    override fun handleIntent(intent: BotIntent, customData: String?) {
//        Log.i(TAG, "Android - 貌似是云端意图处理 - " + intent.name)
        if (intent.name == "game_top_list") {
            Log.i(TAG, "Android - 游戏排行榜 - $customData")
            UnityCommunication.GameRankingList(customData);
        } else if (intent.name == "user_game_data") {
            Log.i(TAG, "Android - 累计卡路里 - $customData")
            UnityCommunication.AllCalorie(customData);
        }
    }

    /**
     * 云端返回的UIContext匹配结果
     * @param url 自定义交互描述中的url
     * @param paramMap 对于系统内建类型，参数列表。参数就是从query中通过分词取得的关键词。
     */
    override fun onClickLink(url: String, paramMap: HashMap<String?, String?>) {
        Log.i(TAG, "Android - 云端返回的UIContext匹配结果 - $url")

        when (url) {
            BotConstants.StartGame -> {
                UnityCommunication.SendBotSdkOrder("StartGame")
            }
            BotConstants.PauseGame -> {
                UnityCommunication.SendBotSdkOrder("PauseGame")
            }
            BotConstants.ContinueGame -> {
                UnityCommunication.SendBotSdkOrder("ContinueGame")
            }
            BotConstants.GoBackGame -> {
                UnityCommunication.SendBotSdkOrder("GoBackGame")
            }
            BotConstants.QuitGame -> {
                UnityCommunication.SendBotSdkOrder("QuitGame")
            }
        }
    }

    override fun onHandleScreenNavigatorEvent(event: Int) {

    }

    override fun onDialogStateChanged(p0: IDialogStateListener.DialogState?) {
        if (p0 != null) {
            Log.i(TAG, "唤醒窗口状态: " + p0.name)
        };
    }


}