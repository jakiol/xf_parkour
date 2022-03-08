package com.xinfan.forest.botsdkutils;

import android.content.Context;
import android.util.Log;

public class ContextUtil {

    private static Context mContext;

    public static void setContext(Context context) {
        Log.i( "ContextUtil", "设置 context");
        mContext = context;
    }

    public static Context getContext() {
        return mContext;
    }
}
