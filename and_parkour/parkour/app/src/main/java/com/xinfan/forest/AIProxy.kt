package com.xinfan.forest

import android.content.ComponentName
import android.content.Context
import android.content.ServiceConnection
import android.graphics.ImageFormat
import android.graphics.Point
import android.media.ImageReader
import android.os.IBinder
import android.os.SystemClock
import android.util.Log
import android.view.WindowManager
import com.baidu.duermirror.*
import com.baidu.duermirror.aiclient.AiServiceClient
import com.xinfan.forest.botsdk.BotInitCallBack


object AIProxy {
    private lateinit var context: Context
    private var aiClient: IAIClientAidlInterface? = null
    private val aiServiceClient: AiServiceClient by lazy { AiServiceClient() }
    private var mImageReader: ImageReader

    private lateinit var mIActionListener: IActionListener
    private lateinit var mIBodynodesListener: IBodynodesListener
    private lateinit var mIGameActionListener: IGameActionListener

    var isBind = false;

    var type = "";
    var downloadUrl = "";
    var md5 = "";
    var courseId = "";

    var isNotifyDownloadOk = false;

    const val TAG = "AIProxy"

    var _BotInitCallBack: BotInitCallBack? = null


    init {
        initListener()

        mImageReader = ImageReader.newInstance(1080, 1920, ImageFormat.YUV_420_888, 1)
        mImageReader.setOnImageAvailableListener(ImageReader.OnImageAvailableListener {
            val image = it.acquireNextImage()
            if (image != null) {
                val planes = image.planes
                val buffer = planes[0].buffer
                var type = buffer.getShort()
                val length = buffer.getInt()
                val data = ByteArray(length)
                buffer.position(4)
                buffer.get(data, 0, length)
                Log.e(
                    TAG, "TIME_DEBUG.onImageAvailable type=${type}, length=${length}, " +
                            "width=${image.width}, height=${image.height}, " +
                            "time=${image.timestamp}, cost=${SystemClock.elapsedRealtime() - image.timestamp}"
                )
                image.close()
            }
        }, null)
    }

    private var mConnection = object : ServiceConnection {
        override fun onServiceConnected(name: ComponentName?, service: IBinder?) {
            Log.i(MainActivity.TAG, "连接到远程服务!")
            aiClient = IAIClientAidlInterface.Stub.asInterface(service)
            service?.linkToDeath(mDeathRecipient, 0)
        }

        override fun onServiceDisconnected(name: ComponentName?) {
            Log.e(MainActivity.TAG, "远程服务断开!")
            aiClient = null
        }

    }

    private var mDeathRecipient: IBinder.DeathRecipient = object : IBinder.DeathRecipient {
        override fun binderDied() {
            aiClient?.asBinder()?.unlinkToDeath(this, 0)
            aiClient == null

            // 重新拉起server
            bindService()
        }
    }

    private fun initListener() {
        mIActionListener = object : IActionListener.Stub() {
            override fun onActionCallback(
                actionId: Int,
                actionType: String?,
                personId: Int,
                isFinished: Boolean,
                code: Int,
                message: String?,
                extra: String?
            ) {
                Log.i(
                    MainActivity.TAG,
                    "onActionDetected actionId = ${actionId}, msg = ${message}, extro = ${extra}"
                )
            }

        }

        mIBodynodesListener = object : IBodynodesListener.Stub() {
            override fun onBodynodesCallback(nodes: String?) {
                Log.i(MainActivity.TAG, "onBodynodesCallback nodes")// = ${nodes}
                UnityCommunication.SendUnityBodynode(nodes);
            }
        }

        mIGameActionListener = object : IGameActionListener.Stub() {
            override fun onGameActionCallback(action: String?, grade: Int) {

                Log.i(
                    MainActivity.TAG,
                    "Android - IGameActionListener onGameActionCallback action = ${action}, grade = ${grade}"
                )

                UnityCommunication.GameAction(action, grade);
            }

//            override fun onMatchCallback(score: Float) {
//                Log.i(
//                    MainActivity.TAG,
//                    "Android - IGameActionListener onMatchCallback action = ${score}"
//                )
//                UnityCommunication.AddPoseVal(score)
//            }

            override fun onMatchCallback(actionId: Int, score: Float) {
                Log.i(
                    MainActivity.TAG,
                    "Android - IGameActionListener onMatchCallback actionId = ${actionId}  score = ${score}"
                )
                UnityCommunication.AddPoseVal(actionId, score)
            }
        }
    }

    fun initContext(context: Context) {
        this.context = context
        Log.i(
            MainActivity.TAG,
            "Android - initContext"
        )
    }

    fun bindService() {
        aiServiceClient.registerServiceConnectionListener(object :
            AiServiceClient.IServiceConnection {
            override fun onBinderDied() {
                isBind = false;
                Log.i(MainActivity.TAG, "Android - onBinderDied")
            }

            override fun onServiceConnected() {
                Log.i(MainActivity.TAG, "Android - onServiceConnected")

//                registerGameActionListener()
//                val array = intArrayOf(99806)
//                resetGameActionList(array, true)

                _BotInitCallBack?.Succeed()

                isBind = true;
            }

            override fun onServiceDisconnected() {
                isBind = false;

                Log.i(MainActivity.TAG, "Android - onServiceDisconnected")
            }
//            override fun onServiceConnected(name: ComponentName?, service: IBinder?) {
//                Log.i(MainActivity.TAG, "连接到远程服务!")
//                aiClient = IAIClientAidlInterface.Stub.asInterface(service)
//                service?.linkToDeath(mDeathRecipient, 0)
//            }
//
//            override fun onServiceDisconnected(name: ComponentName?) {
//                Log.e(MainActivity.TAG, "远程服务断开!")
//                aiClient = null
//            }
        })

        aiServiceClient.bindService(context)
        Log.i(MainActivity.TAG, "Android - bindService")
    }

    fun unbindService() {
        aiServiceClient.unBindService()
    }

    fun resetAction(actionId: Int, type: String, payload: String) {
        aiServiceClient.resetAction(actionId, type, payload)
    }

    fun notifyActionList(actionIds: String) {
//        aiServiceClient.notifyActionList(actionIds)
    }

    fun resetGameActionList(actionList: IntArray, enable: Boolean): Int {
        var actionListId = "";
        for (element in actionList) {
            actionListId += " $element"
        }
        Log.i(
            MainActivity.TAG,
            "Android - 开启/关闭 通用动作匹配，actionList：${actionListId}，enable：${enable}"
        )
        return aiServiceClient.resetGameActionList(courseId, actionList, enable)
    }

    fun enableGameActionMatch(gameId: Int, enable: Boolean) {
        aiServiceClient.enableGameActionMatch(courseId, gameId, enable)
        Log.i(
            MainActivity.TAG,
            "Android - 开启/关闭 游戏匹配，id：${gameId}，enable：${enable}"
        )
    }

    fun resetGameActionMatch(actionId: Int, time: Int) {
        aiServiceClient.resetGameActionMatch(courseId, actionId, time)
        Log.i(
            MainActivity.TAG,
            "Android - 开启具体动作的匹配，id：${actionId}，time：${time}"
        )
    }

    fun registerActionListener() {
        aiServiceClient.registerActionListener(mIActionListener)
    }

    fun unregisterActionListener() {
        aiServiceClient.unRegisterActionListener(mIActionListener)
    }

    fun registerBodyListener() {
        aiServiceClient.registerBodyNodesDataListener(mIBodynodesListener)
    }

    fun unregisterBodyListener() {
        aiServiceClient.unRegisterBodyNodesDataListener(mIBodynodesListener)
    }

    fun registerGameActionListener() {
        aiServiceClient.registerGameActionListener(mIGameActionListener)
        Log.i(
            MainActivity.TAG,
            "Android - 开启通用动作监听"
        )
    }

    fun unregisterGameActionListener() {
        aiServiceClient.unRegisterGameActionListener(mIGameActionListener)
        Log.i(
            MainActivity.TAG,
            "Android - 关闭通用动作监听"
        )
    }

    fun registerVideoFrameListener() {
//        aiServiceClient.registerVideoFrameListener(mImageReader)
    }

    fun unRegisterVideoFrameListener() {
//        aiServiceClient.unRegisterVideoFrameListener(mImageReader)
    }

    fun checkOneOrMini() {
        // 获取设备宽度
        var width = 0;
        val wm = context.getSystemService(Context.WINDOW_SERVICE) as WindowManager
        val display = wm.defaultDisplay
        val size = Point()
        display.getSize(size)
        width = size.x
        Log.d(TAG, "checkOneOrMini: 获取的屏幕宽度是::" + width)
        UnityCommunication.sendClientWidth(width.toString())
    }



    fun notifyDownload() {
        Log.i(MainActivity.TAG, "模型数据 - ${courseId} - ${downloadUrl} - ${md5}")
        aiServiceClient.notifyCourse(
            courseId,

            downloadUrl,
            md5,
            object : IDownloadResultListener.Stub() {
                override fun onDownloadResultCallback(code: Int, message: String?) {
                    if (code == 1000) {
                        isNotifyDownloadOk = true;
                        AndroidTool.INSTANCE.enableGameActionMatch(1000)
                        AndroidTool.INSTANCE.registerGameActionListener();
                        AndroidTool.INSTANCE.OpenResetGameActionList();

                        UnityCommunication.sendNotifyDownloadOk()
                        Log.i(MainActivity.TAG, "模型下载成功")

                    } else {
                        Log.i(MainActivity.TAG, "模型下载失败，${message}")
                    }
                    // code = 1000 为成功，其他code 为失败，检测到1000
                }
            })
    }


}