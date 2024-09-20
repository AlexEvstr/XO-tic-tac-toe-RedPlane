using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private GameObject _scoreboardWindow;

    [SerializeField] private Button _scoreboardButton;
    [SerializeField] private Button _exitScoreboardButton;
    [SerializeField] private Button _resetResultButton;

    public TMP_Text[] scoreFields; // Поля для отображения счетов (7 полей)
    public TMP_Text[] dateFields;  // Поля для отображения дат (7 полей)

    private const int maxHighScores = 7;

    private void Start()
    {
        _scoreboardButton.onClick.AddListener(OpenScoreboard);
        _exitScoreboardButton.onClick.AddListener(CloseScoreboard);
        _resetResultButton.onClick.AddListener(ResetScoreResult);

        LoadHighScores();
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

    private void LoadHighScores()
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            // Загружаем счет и дату из PlayerPrefs
            int score = PlayerPrefs.GetInt($"HighScore_{i}", 0);
            string date = PlayerPrefs.GetString($"HighScoreDate_{i}", "");

            // Если есть сохраненные данные, выводим их, иначе оставляем пустые поля
            if (score > 0 && !string.IsNullOrEmpty(date))
            {
                scoreFields[i].text = score.ToString();
                dateFields[i].text = FormatDate(date);
            }
            else
            {
                scoreFields[i].text = "";
                dateFields[i].text = "";
            }
        }
    }

    // Преобразуем дату в формат dd.MM.yyyy
    private string FormatDate(string originalDate)
    {
        if (DateTime.TryParse(originalDate, out DateTime parsedDate))
        {
            return parsedDate.ToString("dd.MM.yyyy");
        }
        return "";
    }
}