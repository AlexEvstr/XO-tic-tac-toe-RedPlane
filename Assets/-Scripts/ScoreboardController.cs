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
        LoadHighScores();
        _menuWindow.SetActive(false);
        _scoreboardWindow.SetActive(true);
    }

    private void CloseScoreboard()
    {
        _scoreboardWindow.SetActive(false);
        _menuWindow.SetActive(true);
    }

    public void ResetScoreResult()
    {
        // Удаляем топ-7 результатов из PlayerPrefs
        for (int i = 0; i < maxHighScores; i++)
        {
            PlayerPrefs.DeleteKey($"HighScore_{i}");
            PlayerPrefs.DeleteKey($"HighScoreDate_{i}");
        }

        // Сохраняем изменения в PlayerPrefs
        PlayerPrefs.Save();

        // Очищаем отображение результатов на экране
        for (int i = 0; i < maxHighScores; i++)
        {
            scoreFields[i].text = "";
            dateFields[i].text = "";
        }
    }


    private void LoadHighScores()
    {
        // Загрузка текущего уровня
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Создаем массив для хранения очков и дат
        (int score, string date)[] highScores = new (int score, string date)[maxHighScores];

        // Загружаем все сохранённые результаты
        for (int i = 0; i < maxHighScores; i++)
        {
            highScores[i].score = PlayerPrefs.GetInt($"HighScore_{i}", 0);
            highScores[i].date = PlayerPrefs.GetString($"HighScoreDate_{i}", "");
        }

        // Сортируем результаты по убыванию очков
        Array.Sort(highScores, (a, b) => b.score.CompareTo(a.score));

        // Отображаем результаты на экране
        for (int i = 0; i < maxHighScores; i++)
        {
            if (highScores[i].score > 0 && !string.IsNullOrEmpty(highScores[i].date))
            {
                scoreFields[i].text = highScores[i].score.ToString();
                dateFields[i].text = FormatDate(highScores[i].date);
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