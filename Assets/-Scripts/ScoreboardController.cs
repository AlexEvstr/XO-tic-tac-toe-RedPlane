using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private GameObject _scoreboardWindow;

    [SerializeField] private Button _scoreboardButton;
    [SerializeField] private Button _exitScoreboardButton;
    [SerializeField] private Button _resetResultButton;

    private void Start()
    {
        _scoreboardButton.onClick.AddListener(OpenScoreboard);
        _exitScoreboardButton.onClick.AddListener(CloseScoreboard);
        _resetResultButton.onClick.AddListener(ResetScoreResult);
    }

    private void OpenScoreboard()
    {
        _menuWindow.SetActive(false);
        _scoreboardWindow.SetActive(true);
    }

    private void CloseScoreboard()
    {
        _scoreboardWindow.SetActive(false);
        _menuWindow.SetActive(true);
    }

    private void ResetScoreResult()
    {

    }
}