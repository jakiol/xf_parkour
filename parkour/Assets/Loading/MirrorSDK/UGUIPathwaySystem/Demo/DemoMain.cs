using System.Collections;
using System.Collections.Generic;
using Spore.Unity.UI;
using UnityEngine;

public class DemoMain : MonoBehaviour
{
    [SerializeField] UIPathwayController _pathway;
    private string _time = "0";


    private void OnValidate()
    {
        if (!_pathway) _pathway = FindObjectOfType<UIPathwayController>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            _pathway.Play();
        }

        if (GUILayout.Button("Pause"))
        {
            _pathway.Pause();
        }

        _time = GUILayout.TextField(_time);
        if (GUILayout.Button("Play with time"))
        {
            if (float.TryParse(_time, out var time))
            {
                _pathway.Play(time);
            }
        }

        if (GUILayout.Button("Clear"))
        {
            _pathway.Clear();
        }
    }
}
