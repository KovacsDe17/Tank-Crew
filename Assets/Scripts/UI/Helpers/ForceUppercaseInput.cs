using UnityEngine;

/// <summary>
/// Forces uppercase input for the attached input field.
/// </summary>
public class ForceUppercaseInput : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<TMPro.TMP_InputField>().onValidateInput +=
            delegate (string s, int i, char c) { return char.ToUpper(c); };
    }
}
