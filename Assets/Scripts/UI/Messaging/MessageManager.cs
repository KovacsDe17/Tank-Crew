using System.Collections;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;

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

    private IEnumerator ShowMessageCR(string message, float duration)
    {
        if (!_messageCarrier.IsUp())
        {
            _messageCarrier.GoUp();
            yield return new WaitUntil(() => _messageCarrier.IsUp());
        }

        _messageCarrier.SetMessage(message);
        _messageCarrier.GoDown();

        yield return new WaitForSecondsRealtime(duration);

        _messageCarrier.GoUp();
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageCR(message, duration));
    }
}

class Message
{
    public static void Show(string message, float duration = 2f)
    {
        MessageManager.Instance.ShowMessage(message, duration);
    }
}