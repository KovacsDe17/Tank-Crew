using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssignValue : MonoBehaviour
{

    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetFloatValue(float value)
    {
        text.SetText(value.ToString("0.000"));
    }
}
