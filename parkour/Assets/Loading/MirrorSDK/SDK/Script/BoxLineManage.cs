using Spore.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxLineManage : MonoBehaviour {

    public LineUIMesh template;
    public RectTransform BoxLineParent;

    Dictionary<E_HeadLine, LineUIMesh> Dic_HeadLine = new Dictionary<E_HeadLine, LineUIMesh>();
    List<LineUIMesh> HandLineList = new List<LineUIMesh>();
    List<LineUIMesh> SkeletonsLineList = new List<LineUIMesh>();


    public void SetData(SdkBodynodeData sdkData) {

        if (sdkData == null) {
            BoxLineParent.gameObject.SetActive(false);
        }
        else {
            if (sdkData.persons.Count > 0) {
                BoxLineParent.gameObject.SetActive(true);
                Persons persons = sdkData.persons[0];
                SetHeadLines(persons.head);
                //SetHandLines(persons.leftHand, persons.rightHand);
                SetSkeletonsLines(persons.skeletons);
            }
            else {
                BoxLineParent.gameObject.SetActive(false);
            }
        }

    }



    // 头

    public void SetHeadLines(Head head) {
        // v1   v2
        //
        // v3   v4
        Vector2 v1 = new Vector2(head.leftTop.x, head.leftTop.y);
        Vector2 v2 = new Vector2(head.rightBottom.x, head.leftTop.y);
        Vector2 v3 = new Vector2(head.leftTop.x, head.rightBottom.y);
        Vector2 v4 = new Vector2(head.rightBottom.x, head.rightBottom.y);

        v1 = v1.ReversalPosY();
        v2 = v2.ReversalPosY();
        v3 = v3.ReversalPosY();
        v4 = v4.ReversalPosY();

        //  -- L1 --
        // |        |
        // L2      L3
        // |        |
        //  -- L4 --

        LineUIMesh L1 = GetHeadLine(E_HeadLine.Head_L_1);
        LineUIMesh L2 = GetHeadLine(E_HeadLine.Head_L_2);
        LineUIMesh L3 = GetHeadLine(E_HeadLine.Head_L_3);
        LineUIMesh L4 = GetHeadLine(E_HeadLine.Head_L_4);

        L1.StartPosition = v1;
        L1.EndPosition = v2;

        L2.StartPosition = v1;
        L2.EndPosition = v3;

        L3.StartPosition = v2;
        L3.EndPosition = v4;

        L4.StartPosition = v3;
        L4.EndPosition = v4;
    }

    public LineUIMesh GetHeadLine(E_HeadLine line) {
        if (Dic_HeadLine.ContainsKey(line) == false) {
            LineUIMesh L1 = UILineTool.CreateNewLine(template, BoxLineParent, 4, Color.magenta, line.ToString());
            Dic_HeadLine.Add(line, L1);
        }
        Dic_HeadLine[line].gameObject.SetActive(true);
        return Dic_HeadLine[line];
    }


    // 手
    public void SetHandLines(List<Point> leftHand, List<Point> rightHand) {
        foreach (var line in HandLineList) {
            line.gameObject.SetActive(false);
        }

        int index = 0;
        for (int i = 0; i < leftHand.Count - 1; i++) {
            SetHandLine(index, leftHand[i], leftHand[i + 1]);
            index++;
        }

        for (int i = 0; i < rightHand.Count - 1; i++) {
            SetHandLine(index, rightHand[i], rightHand[i + 1]);
            index++;
        }
    }

    public LineUIMesh GetHandLine(int index) {
        if (HandLineList.Count <= index) {
            LineUIMesh L1 = UILineTool.CreateNewLine(template, BoxLineParent, 4, Color.magenta, "Hand_L_" + index);
            HandLineList.Add(L1);
        }
        HandLineList[index].gameObject.SetActive(true);
        return HandLineList[index];
    }
    public void SetHandLine(int index, Point point_start, Point point_end) {
        LineUIMesh L1 = GetHandLine(index);
        Vector2 start = new Vector2(point_start.x, point_start.y);
        Vector2 end = new Vector2(point_end.x, point_end.y);

        start = start.ReversalPosY();
        end = end.ReversalPosY();

        L1.StartPosition = start;
        L1.EndPosition = end;
    }


    // 骨架


    public void SetSkeletonsLines(Skeletons skeletons) {
        foreach (var line in SkeletonsLineList) {
            line.gameObject.SetActive(false);
        }

        int index = 0;

        // 口耳鼻眼
        if (SetSkeletonsNode(index, skeletons.nose, 4, "nose", Color.magenta)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftEar, skeletons.leftEye, 2, "leftEar_leftEye", Color.magenta)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.rightEar, skeletons.rightEye, 2, "rightEar_rightEye", Color.magenta)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftEye, skeletons.rightEye, 2, "rightEar_rightEye", Color.magenta)) {
            index++;
        }

        // 手肘肩
        if (SetSkeletonsLine(index, skeletons.leftWrist, skeletons.leftElbow, 2, "leftEar_leftEye", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftElbow, skeletons.leftShoulder, 2, "rightEar_rightEye", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.rightWrist, skeletons.rightElbow, 2, "rightEar_rightEye", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.rightElbow, skeletons.rightShoulder, 2, "rightEar_rightEye", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftShoulder, skeletons.rightShoulder, 2, "rightEar_rightEye", Color.green)) {
            index++;
        }

        // 肩胯
        if (SetSkeletonsLine(index, skeletons.leftShoulder, skeletons.rightShoulder, 2, "leftShoulder_rightShoulder", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftHip, skeletons.rightHip, 2, "leftHip_rightHip", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftShoulder, skeletons.leftHip, 2, "leftShoulder_leftHip", Color.green)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.rightShoulder, skeletons.rightHip, 2, "rightShoulder_rightHip", Color.green)) {
            index++;
        }

        //骻膝脚
        if (SetSkeletonsLine(index, skeletons.leftHip, skeletons.leftKnee, 2, "rightEar_rightEye", Color.red)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.leftKnee, skeletons.leftAnkle, 2, "rightEar_rightEye", Color.red)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.rightHip, skeletons.rightKnee, 2, "rightEar_rightEye", Color.red)) {
            index++;
        }
        if (SetSkeletonsLine(index, skeletons.rightKnee, skeletons.rightAnkle, 2, "rightEar_rightEye", Color.red)) {
            index++;
        }

    }


    public LineUIMesh GetSkeletonsLine(int index, string name, Color color) {
        if (SkeletonsLineList.Count <= index) {
            LineUIMesh L1 = UILineTool.CreateNewLine(template, BoxLineParent, 4, color, "Skeletons_" + name);
            SkeletonsLineList.Add(L1);
        }
        LineUIMesh L = SkeletonsLineList[index];
        L.Color = color;
        L.name = "Skeletons_" + name;
        L.gameObject.SetActive(true);
        return L;
    }

    public bool SetSkeletonsNode(int index, Point point, float size, string name, Color color) {
        if (point.x > 0 && point.y > 0) {

            LineUIMesh L1 = GetSkeletonsLine(index, name, color);

            float size_2 = size / 2;

            Vector2 start = new Vector2(point.x - size_2, point.y);
            Vector2 end = new Vector2(point.x + size_2, point.y);

            start = start.ReversalPosY();
            end = end.ReversalPosY();

            L1.Width = size;
            L1.StartPosition = start;
            L1.EndPosition = end;
            return true;
        }
        return false;
    }

    public bool SetSkeletonsLine(int index, Point point_start, Point point_end, float size, string name, Color color) {
        if (point_start.x > 0 && point_start.y > 0 && point_end.x > 0 && point_end.y > 0) {
            LineUIMesh L1 = GetSkeletonsLine(index, name, color);

            Vector2 start = new Vector2(point_start.x, point_start.y);
            Vector2 end = new Vector2(point_end.x, point_end.y);

            start = start.ReversalPosY();
            end = end.ReversalPosY();

            L1.Width = size;
            L1.StartPosition = start;
            L1.EndPosition = end;
            return true;
        }
        return false;
    }






}

public enum E_HeadLine {
    Head_L_1,
    Head_L_2,
    Head_L_3,
    Head_L_4,
}
