package com.xinfan.forest;

import com.baidu.duer.bot.event.payload.LinkClickedEventPayload;
import com.baidu.duer.botsdk.BotSdk;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;

public class AndroidTool {

    private AndroidTool() {

    }

    public static AndroidTool INSTANCE;


    public static void Init() {
        if (INSTANCE == null) {
            INSTANCE = new AndroidTool();
        }
    }


    public void bindService() {
        AIProxy.INSTANCE.bindService();
    }

    public void unbindService() {
        AIProxy.INSTANCE.unbindService();
    }

    public void resetAction(int actionId, String type, String payload) {
        AIProxy.INSTANCE.resetAction(actionId, type, payload);
    }

    public void notifyActionList(String actionIds) {
        AIProxy.INSTANCE.notifyActionList(actionIds);
    }

    public void registerActionListener() {
        AIProxy.INSTANCE.registerActionListener();
    }

    public void unregisterActionListener() {
        AIProxy.INSTANCE.unregisterActionListener();
    }

    public void registerBodyListener() {
        AIProxy.INSTANCE.registerBodyListener();
    }

    public void unregisterBodyListener() {
        AIProxy.INSTANCE.unregisterBodyListener();
    }

    public void registerVideoFrameListener() {
        AIProxy.INSTANCE.registerVideoFrameListener();
    }

    public void unRegisterVideoFrameListener() {
        AIProxy.INSTANCE.unRegisterVideoFrameListener();
    }


    public void DebLogTest() {
        UnityCommunication.INSTANCE.DebLog("DebLogTest");
    }


    public void enableGameActionMatch(int id) {
        AIProxy.INSTANCE.enableGameActionMatch(id, true);
    }

    public void registerGameActionListener() {
        AIProxy.INSTANCE.registerGameActionListener();
    }

    public void resetGameActionMatch(int id, int time) {
        AIProxy.INSTANCE.resetGameActionMatch(id, time);
    }

    public void unRegisterGameActionListener() {
        AIProxy.INSTANCE.unregisterGameActionListener();
    }

    public void unEnableGameActionMatch(int id) {
        AIProxy.INSTANCE.enableGameActionMatch(id, false);
    }

    public void OpenResetGameActionList() {
        // 99803, 走/跑 - WALK_FORWARD
        // 99804, 左移 - WALK_LEFT
        // 99805, 右移 - WALK_RIGHT
        // 99806, 跳跃/下蹲 - JUMP/SQUAT
        // 99807, 大字 - ARMS_RAISED
        int[] array = {99806,99807};
        AIProxy.INSTANCE.resetGameActionList(array, true);
    }

    public void CloseResetGameActionList() {
        int[] array = {99804,99805,99806,99807};
        AIProxy.INSTANCE.resetGameActionList(array, false);
    }


    public void updateGameData(String session, int calorie, int duration, String extra) {
        try {
            LinkClickedEventPayload linkClickedEventPayload = new LinkClickedEventPayload();
            linkClickedEventPayload.url = "dueros://duer_mirror/fitness_data_report"
                    + "?session_id=" + URLEncoder.encode(session, "UTF-8")
                    + "&type=game"
                    + "&calorie=" + calorie
                    + "&duration=" + duration
                    + "&extra=" + URLEncoder.encode(extra, "UTF-8");
            BotSdk.getInstance().uploadLinkClickedEvent(linkClickedEventPayload);
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
    }

    public void updateDataAndRequestRankingList(int calorie, int num) {
        LinkClickedEventPayload linkClickedEventPayload = new LinkClickedEventPayload();
        linkClickedEventPayload.url = "dueros://duer_mirror/game_top_list"
                + "?calorie=" + calorie
                + "&num=" + num;
        BotSdk.getInstance().uploadLinkClickedEvent(linkClickedEventPayload);
    }

    public void fetchGameTotalCalorie() {
        LinkClickedEventPayload linkClickedEventPayload = new LinkClickedEventPayload();
        linkClickedEventPayload.url = "dueros://duer_mirror/user_game_data";
        BotSdk.getInstance().uploadLinkClickedEvent(linkClickedEventPayload);
    }


    public void notifyDownload() {
        AIProxy.INSTANCE.notifyDownload();
    }
}
