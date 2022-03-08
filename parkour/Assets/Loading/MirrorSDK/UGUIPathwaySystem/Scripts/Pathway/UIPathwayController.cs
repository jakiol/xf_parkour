using System.Collections.Generic;
using UnityEngine;

namespace Spore.Unity.UI
{
    /// <summary>
    /// UGUI画线控制器组件。
    /// 添加到Canvas下的RectTransform对象上使用。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIPathwayController : MonoBehaviour
    {
        /// <summary>
        /// 当前是否处于播放状态。
        /// </summary>
        public bool IsPlaying { get; private set; }
        /// <summary>
        /// 绘制动画持续时长。
        /// </summary>
        public float Duration { get; private set; }
        /// <summary>
        /// 当前进度。
        /// </summary>
        public float Progress => Mathf.Clamp(_timer / Duration, 0f, 1f);
        /// <summary>
        /// 线段宽度（像素）。
        /// </summary>
        public float LineWidth { get => _lineWidth; set => SetLinesWidth(value); }
        /// <summary>
        /// 线头半径（像素）。
        /// </summary>
        public int HeadRadius { get => _headRadius; set => SetHeadsRadius(value); }

        [SerializeField] LineUIMesh _lineTemplate = null;
        [SerializeField] RegularPolygonUIMesh _headTemplate = null;
        [SerializeField] float _lineWidth = 4;
        [SerializeField] int _headRadius = 8;
        [SerializeField] List<UIWaypoint> _waypoints = new List<UIWaypoint>();
        private float _timer = 0;
        private Dictionary<string, LineUIMesh> _currentLines = new Dictionary<string, LineUIMesh>();
        private Dictionary<string, RegularPolygonUIMesh> _currentHeads = new Dictionary<string, RegularPolygonUIMesh>();
        private Dictionary<string, UIWaypoint> _currentStarts = new Dictionary<string, UIWaypoint>();
        private List<string> _completedLineIds = new List<string>();
        private Queue<UIWaypoint[]> _goToNextJobs = new Queue<UIWaypoint[]>();


        private void Awake()
        {
            // 关闭模板
            _lineTemplate.gameObject.SetActive(false);
            _headTemplate.gameObject.SetActive(false);

            Duration = UILineTool.FindMaxTime(_waypoints);
        }

        private void Update()
        {
            if (IsPlaying)
            {
                _timer += Time.deltaTime;

                // 更新当前活动线段
                UpdateAllUILines();
            }
        }


        /// <summary>
        /// 播放线段绘制动画。
        /// 如果传入不小于0的时间参数，则动画从该时间点的状态开始播放；
        /// 否则动画从当前状态继续播放。
        /// </summary>
        /// <param name="time">从指定时间点的状态开始播放</param>
        public void Play(float time = -1)
        {
            // 如果当前没有正在绘制的线段，则重头绘制
            if (_currentStarts.Count == 0)
            {
                StartNew();
            }

            if (time >= _timer) // 快进
            {
                _timer = time;
            }
            else if (time >= 0 && time < _timer) // 快退
            {
                // 假的快退，只是删除已经绘制的线段然后重新快速画一遍到指定时间
                StartNew();
                _timer = time;
            }

            IsPlaying = true;
        }

        /// <summary>
        /// 暂停线段绘制动画。
        /// </summary>
        public void Pause()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// 清空所有已经绘制的线段。
        /// </summary>
        public void Clear()
        {
            foreach (var line in this.GetComponentsInChildren<LineUIMesh>())
            {
                Destroy(line.gameObject);
            }

            foreach (var head in _currentHeads.Values)
            {
                Destroy(head.gameObject);
            }

            _currentStarts.Clear();
            _currentLines.Clear();
            _currentHeads.Clear();

            _goToNextJobs.Clear();
            _completedLineIds.Clear();

            _timer = 0;
            IsPlaying = false;
        }

        /// <summary>
        /// 覆盖线段颜色，将会同时修改所有UIWaypoint中的颜色数据。
        /// </summary>
        /// <param name="color">新的颜色</param>
        public void OverrideColors(Color color)
        {
            SetUIWaypointColors(color);
            SetLinesColor(color);
        }


        // 开始重头绘制
        private void StartNew()
        {
            Clear();

            // 查找起点
            foreach (var waypoint in _waypoints)
            {
                if (waypoint.Type == UIWaypointType.Start)
                {
                    SetNewData(waypoint);
                }
            }
        }

        // 更新当前活动线段
        private void UpdateAllUILines()
        {
            // 检查是否全部走完
            if (_currentStarts.Count == 0)
            {
                _timer = 0;
                IsPlaying = false;
            }

            // 更新当前活动线段
            foreach (var start in _currentStarts.Values)
            {
                foreach (var end in start.Nexts)
                {
                    UpdateUILine(start, end);
                }
            }

            // 绘制新线段
            while (_goToNextJobs.Count > 0)
            {
                var job = _goToNextJobs.Dequeue();
                GoToNext(job[0], job[1]);
            }
        }

        // 更新UI线段的位置
        private void UpdateUILine(UIWaypoint start, UIWaypoint end)
        {
            var lineId = UILineTool.GetLineId(start, end);
            if (_completedLineIds.Contains(lineId))
            {
                return;
            }

            var clampedTimer = Mathf.Clamp(_timer, 0, end.Time);
            var position = Vector2.Lerp(start.Position, end.Position, (clampedTimer - start.Time) / (end.Time - start.Time));
            var line = _currentLines[lineId];
            var head = _currentHeads[lineId];
            line.EndPosition = position;
            head.Center = position;

            // 当前线段已经结束
            if (_timer > end.Time)
            {
                _goToNextJobs.Enqueue(new UIWaypoint[] { start, end });
            }
        }

        // 开始绘制下一条线段
        private void GoToNext(UIWaypoint currentStart, UIWaypoint currentEnd)
        {
            RemoveOldData(currentStart, currentEnd);
            if (currentEnd.Type != UIWaypointType.End)
            {
                SetNewData(currentEnd);
            }
        }

        // 移除旧数据
        private void RemoveOldData(UIWaypoint oldStart, UIWaypoint oldEnd)
        {
            var completedLineId = UILineTool.GetLineId(oldStart, oldEnd);
            _completedLineIds.Add(completedLineId);

            // 同一个起点可能对应多个终点，如果当前起点还被其他终点使用，则不应该移除
            var shouldRemoveStart = true;
            foreach (var next in oldStart.Nexts)
            {
                if (_timer < next.Time)
                {
                    shouldRemoveStart = false;
                    break;
                }
            }
            if (shouldRemoveStart)
            {
                var oldStartId = UILineTool.GetUIWaypointId(oldStart);
                _currentStarts.Remove(oldStartId);
            }

            // 线体和线头
            var currentHead = _currentHeads[completedLineId];
            _currentLines.Remove(completedLineId);
            _currentHeads.Remove(completedLineId);
            // 保留线体，但不保留线头
            Destroy(currentHead.gameObject);
        }

        // 设置新数据
        private void SetNewData(UIWaypoint newStart)
        {
            // 如果是跳转类型的节点，则直接将跳转目标作为新的起点
            if (newStart.Type == UIWaypointType.Jump)
            {
                foreach (var jumpTo in newStart.Nexts)
                {
                    SetNewData(jumpTo);
                }

                return;
            }

            // 添加新数据，原来的终点作为新的起点
            foreach (var newEnd in newStart.Nexts)
            {
                // 多条线可能汇合到同一条线，因此要检查Key是否已经存在

                var newStartId = UILineTool.GetUIWaypointId(newStart);
                if (!_currentStarts.ContainsKey(newStartId))
                {
                    _currentStarts.Add(newStartId, newStart);
                }

                var newLineAndHeadId = UILineTool.GetLineId(newStart, newEnd);
                if (!_currentLines.ContainsKey(newLineAndHeadId))
                {
                    var newLine = UILineTool.CreateNewLine(_lineTemplate, transform as RectTransform, newStart, LineWidth, newEnd.Color, $"[Line_{newLineAndHeadId}]");
                    _currentLines.Add(newLineAndHeadId, newLine);
                }

                if (!_currentHeads.ContainsKey(newLineAndHeadId))
                {
                    var newHead = UILineTool.CreateNewHead(_headTemplate, transform as RectTransform, newStart, HeadRadius, newEnd.Color, $"[Head_{newLineAndHeadId}]");
                    _currentHeads.Add(newLineAndHeadId, newHead);
                }
            }
        }

        #region 设置参数

        private void SetUIWaypointColors(Color color)
        {
            foreach (var waypoint in _waypoints)
            {
                waypoint.Color = color;
                waypoint.UpdateName();
            }
        }

        private void SetLinesColor(Color color)
        {
            // 如果颜色变化频繁，应该将 LineUIMesh 实例缓存起来
            foreach (var line in this.GetComponentsInChildren<LineUIMesh>() /*_currentLines.Values*/)
            {
                line.Color = color;
            }

            foreach (var head in _currentHeads.Values)
            {
                head.Color = color;
            }
        }

        private void SetLinesWidth(float lineWidth)
        {
            _lineWidth = lineWidth;

            // 如果颜色变化频繁，应该将 LineUIMesh 实例缓存起来
            foreach (var line in this.GetComponentsInChildren<LineUIMesh>() /*_currentLines.Values*/)
            {
                line.Width = lineWidth;
            }
        }

        private void SetHeadsRadius(int headRadius)
        {
            _headRadius = headRadius;

            foreach (var head in _currentHeads)
            {
                head.Value.Radius = headRadius;
            }
        }

        #endregion

#if UNITY_EDITOR  // 编辑器工具

        // 为了方便编辑器操作而添加的字段，替代编写编辑器扩展代码
        [Header("Editor Only")]
        [SerializeField] bool _refresh = false;
        [SerializeField] float _speedInPixel = 200;
        [SerializeField] bool _autoCalcWaypointsTime = true;
        [SerializeField] Color _colorOverride = Color.magenta;
        [SerializeField] bool _overrideColors = false;
        [SerializeField] bool _sortWaypointsByTime = false;

        private void OnValidate()
        {
            _refresh = false;

            // 模板
            if (!_lineTemplate) { _lineTemplate = GetComponentInChildren<LineUIMesh>(); }
            if (!_headTemplate) { _headTemplate = GetComponentInChildren<RegularPolygonUIMesh>(); }

            // 使用List便于排序，仅在编辑器中执行，便于查看
            _waypoints = new List<UIWaypoint>(GetComponentsInChildren<UIWaypoint>());

            // 自动计算节点时间
            if (_autoCalcWaypointsTime) { UILineTool.ResetUIWaypointsTime(_waypoints, _speedInPixel); }

            // 覆盖顶点颜色
            if (_overrideColors) { OverrideColors(_colorOverride); }

            // 排序
            if (_sortWaypointsByTime) { UILineTool.SortUIWaypointsByTime(_waypoints); }

            // 样式
            SetLinesWidth(_lineWidth);
            SetHeadsRadius(_headRadius);
        }

#endif
    }
}
