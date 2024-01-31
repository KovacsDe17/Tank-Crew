using System.Collections;
using UnityEngine;

/// <summary>
/// Create new messages.
/// </summary>
public class MessageManager : MonoBehaviour
{
    [SerializeField]
    MessageCarrier messageCarrierPrefab;

    private MessageCarrier _messageCarrier;

    public static MessageManager Instance { get; private set; }    //Singleton instance

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (_messageCarrier == null)
        {
            _messageCarrier = Instantiate(messageCarrierPrefab);
        }

        _messageCarrier.transform.parent = GameObject.FindGameObjectWithTag("MainCanvas").transform;
    }

    public MessageCarrier GetMessageCarrier()
    {
        return _messageCarrier;
    }

    private IEnumerator ShowMessageCR(string title, string message, float duration)
    {
        if (!_messageCarrier.IsUp())
        {
            _messageCarrier.GoUp();
            yield return new WaitUntil(() => _messageCarrier.IsUp());
        }

        _messageCarrier.SetMessage(title, message);
        _messageCarrier.GoDown();

        yield return new WaitForSecondsRealtime(duration);

        _messageCarrier.GoUp();
    }

    public void ShowMessage(string title, string message, float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageCR(title, message, duration));
    }
}

/// <summary>
/// Made for easier reference.
/// </summary>
class Message
{
    /// <summary>
    /// Show a message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="duration">For how long the message stays on screen, in seconds.</param>
    public static void Show(string title, string message, float duration = 3f)
    {
        MessageManager.Instance.ShowMessage(title, message, duration);
    }
}