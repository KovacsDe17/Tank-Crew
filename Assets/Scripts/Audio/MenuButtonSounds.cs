using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add sounds to the menu elements.
/// </summary>
public class MenuButtonSounds : MonoBehaviour
{
    private void Start()
    {
        foreach(Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
        {
            button.onClick.AddListener(() => {
                AudioManager.Instance.PlaySound(AudioManager.Sound.Button_Click);
            });
        }
    }
}