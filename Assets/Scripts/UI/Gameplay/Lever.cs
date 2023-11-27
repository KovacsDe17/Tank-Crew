using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles a lever, which moves the players tank
/// </summary>
public class Lever : MonoBehaviour
{
    private Scrollbar leverScrollbar;

    private void Awake()
    {
        InitalizeValues();
    }

    /// <summary>
    /// Assign a scrollbar object to this element, and set it to be centered at start
    /// </summary>
    private void InitalizeValues()
    {
        leverScrollbar = GetComponent<Scrollbar>();
        leverScrollbar.value = 0.5f;
    }

    /// <summary>
    /// Set the value for the scrollbar object to move its handle to a desired position
    /// </summary>
    /// <param name="value">Position of the handle, where 0.0 is the bottom and 1.0 is the top</param>
    public void SetValue(float value)
    {
        leverScrollbar.value = value;
    }

    private float Normalize(float value)
    {
        return (value - 0.5f) * -2f;
    }

    public float GetNormalizedValue()
    {
        return leverScrollbar == null ? 0 : Normalize(leverScrollbar.value);
    }

    public bool HasMoved()
    {
        return leverScrollbar.value != 0.5f;
    }
}
