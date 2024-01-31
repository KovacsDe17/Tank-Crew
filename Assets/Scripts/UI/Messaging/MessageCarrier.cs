using TMPro;
using UnityEngine;

/// <summary>
/// Displays the given message.
/// </summary>
[RequireComponent(typeof(Animator))]
public class MessageCarrier : MonoBehaviour
{
    private const string ANIMATION_INITIAL = "MessageCarrier_Initial";
    private const string ANIMATION_UP = "MessageCarrier_Up";
    private const string ANIMATION_TRIGGER_GODOWN = "GoDown";
    private const string ANIMATION_TRIGGER_GOUP = "GoUp";

    Animator animator;

    [SerializeField]
    TextMeshProUGUI messageText;    //Text to display

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void GoDown()
    {
        animator.ResetTrigger(ANIMATION_TRIGGER_GOUP);
        animator.SetTrigger(ANIMATION_TRIGGER_GODOWN);
    }
    public void GoUp()
    {
        animator.ResetTrigger(ANIMATION_TRIGGER_GODOWN);
        animator.SetTrigger(ANIMATION_TRIGGER_GOUP);
    }

    public bool IsUp()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATION_INITIAL) ||
            (animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATION_UP) && 
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
    }

    /// <summary>
    /// Set the text of the message.
    /// </summary>
    /// <param name="message">The text to display.</param>
    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
