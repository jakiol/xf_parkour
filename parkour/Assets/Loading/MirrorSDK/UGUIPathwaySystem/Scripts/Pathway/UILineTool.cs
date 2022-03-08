using System.Collections.Generic;
using UnityEngine;

namespace Spore.Unity.UI
{
    public static class UILineTool
    {
        /// <summary>
        /// 创建新的线头。
        /// </summary>
        /// <param name="template">模板</param>
        /// <param name="parent">父对象</param>
        /// <param name="start">初始位置</param>
        /// <param name="radius">多边形半径</param>
        /// <param name="color">颜色</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static RegularPolygonUIMesh CreateNewHead(RegularPolygonUIMesh template, RectTransform parent, UIWaypoint start, float radius, Color color, string name)
        {
            var headCom = Object.Instantiate(template, parent, false);
            var rectTrans = headCom.GetComponent<RectTransform>();

            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.anchoredPosition = Vector2.zero;
            rectTrans.sizeDelta = Vector2.zero;
            rectTrans.pivot = Vector2.zero;

            headCom.name = name;
            headCom.Color = color;
            headCom.Radius = radius;
            headCom.Center = start.Position;
            headCom.gameObject.SetActive(true);

            return headCom;
        }

        /// <summary>
        /// 创建新的线体。
        /// </summary>
        /// <param name="template">模板</param>
        /// <param name="parent">父对象</param>
        /// <param name="start">起点</param>
        /// <param name="width">线宽</param>
        /// <param name="color">颜色</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static LineUIMesh CreateNewLine(LineUIMesh template, RectTransform parent, UIWaypoint start, float width, Color color, string name)
        {
            var lineCom = Object.Instantiate(template, parent, false);
            var rectTrans = lineCom.GetComponent<RectTransform>();

            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.anchoredPosition = Vector2.zero;
            rectTrans.sizeDelta = Vector2.zero;
            rectTrans.pivot = Vector2.zero;

            lineCom.name = name;
            lineCom.Color = color;
            lineCom.Width = width;
            lineCom.StartPosition = start.Position;
            lineCom.EndPosition = start.Position;
            lineCom.gameObject.SetActive(true);

            return lineCom;
        }


        /// <summary>
        /// 创建新的线体。
        /// </summary>
        /// <param name="template">模板</param>
        /// <param name="parent">父对象</param>
        /// <param name="start">起点</param>
        /// <param name="width">线宽</param>
        /// <param name="color">颜色</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static LineUIMesh CreateNewLine(LineUIMesh template, RectTransform parent, float width, Color color, string name) {
            var lineCom = Object.Instantiate(template, parent, false);
            var rectTrans = lineCom.GetComponent<RectTransform>();

            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.anchoredPosition = Vector2.zero;
            rectTrans.sizeDelta = Vector2.zero;
            rectTrans.pivot = Vector2.zero;

            lineCom.name = name;
            lineCom.Color = color;
            lineCom.Width = width;
            lineCom.StartPosition = Vector2.zero;
            lineCom.EndPosition = Vector2.zero;
            lineCom.gameObject.SetActive(true);

            return lineCom;
        }


        /// <summary>
        /// 查找最长到达时间。
        /// </summary>
        /// <param name="waypoints">UIWaypoint集合</param>
        /// <returns></returns>
        public static float FindMaxTime(IEnumerable<UIWaypoint> waypoints)
        {
            var maxTime = 0f;
            foreach (var waypoint in waypoints)
            {
                if (maxTime < waypoint.Time)
                {
                    maxTime = waypoint.Time;
                }
            }

            return maxTime;
        }

        /// <summary>
        /// 计算点ID。
        /// </summary>
        /// <param name="waypoint">点</param>
        /// <returns></returns>
        public static string GetUIWaypointId(UIWaypoint waypoint)
        {
            return $"{waypoint.Position.ToString()}:{waypoint.Time.ToString()}";
        }

        /// <summary>
        /// 计算线ID。
        /// </summary>
        /// <param name="start">线段起点</param>
        /// <param name="end">线段终点</param>
        /// <returns></returns>
        public static string GetLineId(UIWaypoint start, UIWaypoint end)
        {
            return $"{start.Position.ToString()}:{start.Time.ToString()}_{end.Position.ToString()}:{end.Time.ToString()}";
        }

#if UNITY_EDITOR

        /// <summary>
        /// 重新计算节点时间。
        /// </summary>
        /// <param name="waypoints">UIWaypoint集合</param>
        /// <param name="speedInPixel">运动速度（像素）</param>
        public static void ResetUIWaypointsTime(IEnumerable<UIWaypoint> waypoints, float speedInPixel)
        {
            void CalcNextsTime(UIWaypoint current)
            {
                foreach (var next in current.Nexts)
                {
                    if (next.AutoCalcTime)
                    {
                        var deltaTime = 0f;
                        if (current.Type != UIWaypointType.Jump)
                        {
                            var distance = Vector2.Distance(current.Position, next.Position);
                            deltaTime = distance / speedInPixel;
                        }
                        next.Time = current.Time + deltaTime;
                        next.UpdateName();
                    }

                    if (next.Type != UIWaypointType.End)
                    {
                        CalcNextsTime(next);
                    }
                }
            }

            foreach (var waypoint in waypoints)
            {
                if (waypoint.Type == UIWaypointType.Start)
                {
                    CalcNextsTime(waypoint);
                }
            }
        }

        /// <summary>
        /// 根据时间对UIWaypoint进行排序。
        /// </summary>
        /// <param name="waypoints">UIWaypoint集合</param>
        public static void SortUIWaypointsByTime(List<UIWaypoint> waypoints)
        {
            waypoints.Sort((a, b) =>
            {
                if (a.Time < b.Time) return -1;
                else if (a.Time > b.Time) return 1;
                else return 0;
            });

            for (int i = 0; i < waypoints.Count; i++)
            {
                waypoints[i].transform.SetSiblingIndex(i);
            }
        }

#endif
    }
}
