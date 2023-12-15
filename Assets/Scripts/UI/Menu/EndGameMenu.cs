using System;
using TMPro;
using UnityEngine;

public class EndGameMenu : MonoBehaviour
{
    private const string ON_WIN_TITLE = "Glorious Victory!";
    private const string ON_LOSE_TITLE = "Miserable Defeat!";

    [Header("Elements of End Game Menu")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _timeElapsedText;
    [SerializeField] private TextMeshProUGUI _enemiesDestroyedText;
    [SerializeField] private TextMeshProUGUI _shotsFiredText;

    [Header("Other UI elements")]
    [SerializeField] private GameObject _playerControls;

    private void Start()
    {
        GameManager.Instance.OnGameEnd += ShowMenuOnGameEnd;

        Hide();
    }

    private void ShowMenuOnGameEnd(object sender, EventArgs e)
    {
        UpdateInfos((GameManager.EndGameEventArgs) e);
        _playerControls.SetActive(false);
        Show();
    }

    /// <summary>
    /// Set the local scale to one to show the menu.
    /// </summary>
    private void Show()
    {
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Set the local scale to zero to hide the menu.
    /// </summary>
    private void Hide()
    {
        transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Update all texts based on the given infos
    /// </summary>
    private void UpdateInfos(GameManager.EndGameEventArgs e)
    {
        _titleText.SetText(e.won ? ON_WIN_TITLE : ON_LOSE_TITLE);

        string timeElapsed = TimeSpan.FromSeconds(e.timeElapsed).ToString(@"mm\:ss");
        _timeElapsedText.SetText(timeElapsed);

        _enemiesDestroyedText.SetText(e.enemiesDestroyed.ToString());

        _shotsFiredText.SetText(e.shotsFired.ToString());
    }

    public void UnMuteIngameSounds()
    {
        AudioManager.Instance.MuteCategory(AudioManager.Category.IngameSound, false);
    }
}
