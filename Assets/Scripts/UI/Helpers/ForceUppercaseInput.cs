using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceUppercaseInput : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<TMPro.TMP_InputField>().onValidateInput +=
            delegate (string s, int i, char c) { return char.ToUpper(c); };
    }
}
