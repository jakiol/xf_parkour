using UnityEngine;

namespace Spore.Unity.UI
{
    /// <summary>
    /// UIWaypoint类型。
    /// </summary>
    public enum UIWaypointType
    {
        /// <summary>
        /// 未设置功能。
        /// </summary>
        None,
        /// <summary>
        /// 起点。
        /// </summary>
        Start,
        /// <summary>
        /// 中继点。
        /// </summary>
        Relay,
        /// <summary>
        /// 跳跃点。
        /// </summary>
        Jump,
        /// <summary>
        /// 终点。
        /// </summary>
        End,
    }

    /// <summary>
    /// UIPathway中的路径点。
    /// 添加到UIPathway下的RectTransform对象上使用。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIWaypoint : MonoBehaviour
    {
        /// <summary>
        /// 点位置。
        /// </summary>
        public Vector2 Position => (transform as RectTransform).anchoredPosition;

        /// <summary>
        /// 点类型。
        /// </summary>
        public UIWaypointType Type;
        /// <summary>
        /// 目标到达时间。
        /// </summary>
        public float Time;
        /// <summary>
        /// 顶点颜色。当连线时使用后一个顶点的颜色作为线段的颜色。
        /// </summary>
        public Color Color = Color.magenta;
        /// <summary>
        /// 后续连接点。
        /// </summary>
        public UIWaypoint[] Nexts;


        /// <summary>
        /// 根据时间和类型更新节点名称。
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void UpdateName()
        {
            name = $"[Waypoint_{Time.ToString()}s_{Type.ToString()}_#{ColorUtility.ToHtmlStringRGBA(Color)}]";
        }

#if UNITY_EDITOR

        [Header("Editor Only")]
        /// <summary>
        /// 是否允许控制器根据节点之间的距离自动计算Time字段的值。
        /// </summary>
        public bool AutoCalcTime = true;

        private void Reset()
        {
            var rectTrans = transform as RectTransform;
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.zero;
            rectTrans.sizeDelta = Vector2.zero;
        }

        private void OnValidate()
        {
            CheckType();
            UpdateName();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(Position, 5);
            if (Nexts != null && Nexts.Length > 0)
            {
                foreach (var next in Nexts)
                {
                    if (next)
                    {
                        var debugColor = Type == UIWaypointType.Jump ? Color.grey : next.Color;
                        Debug.DrawLine(Position, next.Position, debugColor);
                    }
                }
            }
        }

        // 检查节点类型设置
        private bool CheckType()
        {
            switch (Type)
            {
                case UIWaypointType.Start:
                case UIWaypointType.Relay:
                case UIWaypointType.Jump:
                    if (Nexts == null || Nexts.Length < 1)
                    {
                        Debug.LogError($"{Type.ToString()} 类型的节点需要至少1个后续节点。", this);
                        return false;
                    }
                    return true;
                case UIWaypointType.End:
                    Nexts = null;
                    return true;
                default:
                    Debug.LogError($"无效类型：{Type.ToString()}", this);
                    return false;
            }
        }

#endif
    }
}
