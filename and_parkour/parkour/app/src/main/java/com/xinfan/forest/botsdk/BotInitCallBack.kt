package com.xinfan.forest.botsdk

interface BotInitCallBack {
    /**
     * 失败
     */
    fun Failed();

    /**
     * 成功
     */
    fun Succeed();
}