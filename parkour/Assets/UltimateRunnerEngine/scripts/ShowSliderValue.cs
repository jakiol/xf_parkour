using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
    public Text text;

    public Slider setText { set {
            text.text = value.value.ToString("F3");
        }
    }
}
