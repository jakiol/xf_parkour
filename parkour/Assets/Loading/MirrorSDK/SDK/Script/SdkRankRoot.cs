using System.Collections.Generic;

public class SdkRankTopListItem {
    /// <summary>
    /// 
    /// </summary>
    public string avatar {
        get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    public string id {
        get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    public string name {
        get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    public int rank {
        get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    public int topCalorie {
        get; set;
    }
}

public class SdkRankUserData : SdkRankTopListItem
{
    /// <summary>
    /// 
    /// </summary>
    public int totalCalorie {
        get; set;
    }
}

public class SdkRankRoot {
    /// <summary>
    /// 
    /// </summary>
    public List<SdkRankTopListItem> topList {
        get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    public string type {
        get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    public SdkRankUserData userData {
        get; set;
    }
}