package com.xinfan.forest.botsdkutils;


public class BotConstants {

    /**
     * BotID和签名key ,可以在自己的技能控制台看到相关信息：https://dueros.baidu.com/dbp/main/console
     */
    public static final String BOTID = "89e5186f-3fae-841e-4634-50248b3e84a2";
    public static final String SIGNATURE_KEY = "98230e5d-cfd2-85a2-5114-18dcadf31857";


    // 两个随机值的前缀，这里对格式不作要求
    public static final String RANDOM1_PREFIX = "random1";
    public static final String RANDOM2_PREFIX = "random2";

    // UIControl用到的路由常量
    /**
     * 开始
     */
    public static final String StartGame = "xinfan://startgame";
    /**
     * 暂停
     */
    public static final String PauseGame = "xinfan://pausegame";
    /**
     * 继续
     */
    public static final String ContinueGame = "xinfan://continuegame";
    /**
     * 返回
     */
    public static final String GoBackGame = "xinfan://gobackgame";
    /**
     * 退出
     */
    public static final String QuitGame = "xinfan://quitgame";

//    public static final String CLICK_LOGIN_URL = "sdkdemo://clicklogin";
//    public static final String CLICK_REGISTER_URL = "sdkdemo://clickregister";
//    public static final String SELETC_TEST_URL = "sdkdemo://selecttest/";
//    public static final String INPUT_TEST_URL = "sdkdemo://inputtest/";
//    // 课程表 or 日程表页面
//    public static final String OPEN_CANLENDAR_URL = "dueros://6402fc3a-7825-3147-b52e-c25911b6d03c/entity"
//            + "/timerShow/allUsedList?startDay=MONTH_START_DAY&endDay=MONTH_END_DAY&selectedDay=TODAY&requestType"
//            + "=SHOW&habitChannel=duyayaapk";
//
//    // 自定义快进意图名称
//    public static final String FAST_FORWARD_INTENT = "fast_forward_intent";
//    // 系统预定义[数字] 槽位名称
//    public static final String SYS_NUMBER = "sys.number";
//    // 用户自定义词典[时间单位] 槽位名称
//    public static final String TIME_UNIT = "time.unit";
//    // 用户自定义词典值
//    public static final String TIME_UNIT_SECOND = "second";
//    public static final String TIME_UNIT_MINUTE = "minute";
}