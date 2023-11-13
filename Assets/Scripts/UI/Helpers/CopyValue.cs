using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyValue : MonoBehaviour
{
    private TMP_InputField _myInputField;
    private void Start()
    {
        _myInputField = GetComponent<TMP_InputField>();
    }

    public void CopyText(TMP_InputField inputFieldToCopy)
    {
        _myInputField.SetTextWithoutNotify(inputFieldToCopy.text);
    }
}
