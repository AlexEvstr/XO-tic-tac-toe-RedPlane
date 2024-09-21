using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameBehavior : MonoBehaviour
{
    private enum Player { None, PlayerX, PlayerO }

    private Player[,] grid = new Player[3, 3];
    private Player currentPlayer = Player.PlayerX;
    private bool isGameActive = true;

    public Sprite spriteX; // Спрайт для крестика
    public Sprite spriteO; // Спрайт для нолика

    public Button[] gridButtons; // Кнопки игрового поля

    // Линии для победы (предположим, у нас 8 линий: 3 горизонтали, 3 вертикали, 2 диагонали)
    public GameObject[] winLines;

    // Спрайты для победной линии
    public Sprite winLineRed;    // Спрайт для красной линии (победа игрока)
    public Sprite winLineGreen;  // Спрайт для зелёной линии (поражение компьютера)

    // TMP поля для уровня и отображения текущего хода
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text _winLevelText;
    [SerializeField] private TMP_Text _loseLevelText;
    [SerializeField] private TMP_Text currentPlayerText;
    [SerializeField] private TMP_Text scoreText;

    // Объекты для переключения (игра, победа, поражение)
    public GameObject gameField;
    public GameObject winPanel;
    public GameObject losePanel;
    public Button continueWinButton; // Кнопка "Продолжить" после победы
    public Button continueLoseButton; // Кнопка "Продолжить" после поражения

    // Уровень и очки
    private int currentLevel = 1;
    private int currentScore = 0;

    // Количество лучших результатов, которые мы будем сохранять
    private const int maxHighScores = 7;

    void Start()
    {
        // Загружаем уровень и очки из PlayerPrefs или устанавливаем по умолчанию
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        ResetGame();
    }

    public void PlayerMove(int index)
    {
        if (!isGameActive) return; // Если игра закончена, ничего не делать

        int row = index / 3;
        int col = index % 3;

        if (grid[row, col] == Player.None)
        {
            grid[row, col] = currentPlayer;

            // Устанавливаем спрайт крестика или нолика в зависимости от текущего игрока
            gridButtons[index].GetComponent<Image>().sprite = currentPlayer == Player.PlayerX ? spriteX : spriteO;
            gridButtons[index].GetComponent<Image>().color = new Color(1, 1, 1, 1);

            // Делаем кнопку неактивной, чтобы по ней нельзя было нажимать снова
            gridButtons[index].interactable = false;

            if (CheckWin(currentPlayer, out int winLineIndex))
            {
                ShowWinLine(winLineIndex, currentPlayer);
                StartCoroutine(WaitAndEndGame(currentPlayer == Player.PlayerX ? "Player X wins!" : "Player O wins!"));
                return;
            }

            if (CheckDraw())
            {
                currentPlayerText.text = "Draw!";
                currentScore += 25; // При ничье добавляем 25 очков
                UpdateScore();
                SaveHighScore(currentScore);
                PlayerPrefs.SetInt("CurrentLevel", currentLevel); // Сохраняем текущий уровень
                PlayerPrefs.SetInt("CurrentScore", currentScore); // Сохраняем текущий счёт
                PlayerPrefs.Save();
                Invoke("ResetGame", 2f); // Перезапуск игры через 2 секунды после ничьей
                return;
            }

            // Меняем ход
            currentPlayer = currentPlayer == Player.PlayerX ? Player.PlayerO : Player.PlayerX;

            // Обновляем текст для отображения текущего хода
            currentPlayerText.text = currentPlayer == Player.PlayerX ? "Player X's turn" : "Player O's turn";

            if (currentPlayer == Player.PlayerO)
            {
                Invoke("ComputerMove", 0.5f); // Задержка перед ходом компьютера
            }
        }
    }

    // Метод для выполнения паузы перед показом победы/поражения
    private IEnumerator WaitAndEndGame(string result)
    {
        yield return new WaitForSeconds(1f); // Пауза в 1 секунду
        EndGame(result); // Вызов метода для окончания игры после паузы
    }

    private void EndGame(string result)
    {
        isGameActive = false;

        if (result.Contains("Player X"))
        {
            // Увеличиваем уровень на 1 при победе игрока
            currentLevel++;
            int pointsGained = currentLevel * 2 * 100;
            currentScore += pointsGained;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel); // Сохраняем уровень в PlayerPrefs
            PlayerPrefs.SetInt("CurrentScore", currentScore); // Сохраняем очки в PlayerPrefs
            UpdateScore();
            SaveHighScore(currentScore);
            PlayerPrefs.SetInt("CurrentLevel", currentLevel); // Сохраняем текущий уровень
            PlayerPrefs.SetInt("CurrentScore", currentScore); // Сохраняем текущий счёт
            PlayerPrefs.Save();
            winPanel.SetActive(true);
            continueWinButton.gameObject.SetActive(true); // Включаем кнопку "Продолжить" для победы
        }
        else if (result.Contains("Player O"))
        {
            // При проигрыше сбрасываем уровень и очки
            currentLevel = 1;
            currentScore = 0;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel); // Сбрасываем уровень
            PlayerPrefs.SetInt("CurrentScore", currentScore); // Сбрасываем очки
            UpdateScore();
            losePanel.SetActive(true);
            continueLoseButton.gameObject.SetActive(true); // Включаем кнопку "Продолжить" для поражения
        }

        // Сохраняем лучший результат
        SaveHighScore(currentScore);

        gameField.SetActive(false); // Отключаем игровое поле
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + currentScore;
    }

    private void ComputerMove()
    {
        if (!isGameActive) return;

        // Случайная ошибка компьютера (например, 10% шанс на ошибку)
        if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
        {
            // Компьютер делает случайный ход
            RandomComputerMove();
        }
        else
        {
            // Умный ход компьютера (алгоритм MiniMax)
            SmartComputerMove();
        }
    }

    private void SaveHighScore(int score)
    {
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Считываем существующие результаты
        int[] highScores = new int[maxHighScores];
        string[] highScoreDates = new string[maxHighScores];

        for (int i = 0; i < maxHighScores; i++)
        {
            highScores[i] = PlayerPrefs.GetInt($"HighScore_{i}", 0);
            highScoreDates[i] = PlayerPrefs.GetString($"HighScoreDate_{i}", "");
        }

        // Если это первый лучший результат, обновляем его
        if (highScores[0] == 0 || score > highScores[0])
        {
            highScores[0] = score;
            highScoreDates[0] = currentDate;
        }
        else if (currentLevel == 1) // При проигрыше смещаем текущий результат вниз
        {
            // Сдвигаем результаты ниже
            for (int i = maxHighScores - 1; i > 0; i--)
            {
                highScores[i] = highScores[i - 1];
                highScoreDates[i] = highScoreDates[i - 1];
            }

            highScores[0] = score;
            highScoreDates[0] = currentDate;
        }

        // Сохраняем обновленный топ-7
        for (int j = 0; j < maxHighScores; j++)
        {
            PlayerPrefs.SetInt($"HighScore_{j}", highScores[j]);
            PlayerPrefs.SetString($"HighScoreDate_{j}", highScoreDates[j]);
        }

        PlayerPrefs.Save(); // Сохраняем изменения
    }


    // Умный ход компьютера (алгоритм MiniMax)
    private void SmartComputerMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;

            if (grid[row, col] == Player.None)
            {
                grid[row, col] = Player.PlayerO;
                int score = MiniMax(grid, 0, false);
                grid[row, col] = Player.None;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        PlayerMove(bestMove);
    }

    // Простой случайный ход компьютера (если он "ошибается")
    private void RandomComputerMove()
    {
        int move = UnityEngine.Random.Range(0, 9);
        while (grid[move / 3, move % 3] != Player.None)
        {
            move = UnityEngine.Random.Range(0, 9);
        }
        PlayerMove(move);
    }

    // MiniMax алгоритм для компьютера
    private int MiniMax(Player[,] board, int depth, bool isMaximizing)
    {
        if (CheckWin(Player.PlayerO, out _)) return 10;
        if (CheckWin(Player.PlayerX, out _)) return -10;
        if (CheckDraw()) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            for (int i = 0; i < 9; i++)
            {
                int row = i / 3;
                int col = i % 3;

                if (board[row, col] == Player.None)
                {
                    board[row, col] = Player.PlayerO;
                    int score = MiniMax(board, depth + 1, false);
                    board[row, col] = Player.None;
                    bestScore = Mathf.Max(score, bestScore);
                }
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            for (int i = 0; i < 9; i++)
            {
                int row = i / 3;
                int col = i % 3;

                if (board[row, col] == Player.None)
                {
                    board[row, col] = Player.PlayerX;
                    int score = MiniMax(board, depth + 1, true);
                    board[row, col] = Player.None;
                    bestScore = Mathf.Min(score, bestScore);
                }
            }

            return bestScore;
        }
    }

    void ShowWinLine(int index, Player player)
    {
        // Меняем логику: красная линия для победы игрока, зелёная для победы компьютера
        if (index >= 0 && index < winLines.Length)
        {
            Image lineImage = winLines[index].GetComponent<Image>();
            lineImage.sprite = (player == Player.PlayerX) ? winLineRed : winLineGreen;
            winLines[index].SetActive(true);
        }
    }

    bool CheckWin(Player player, out int winLineIndex)
    {
        winLineIndex = -1;

        // Проверка строк
        for (int i = 0; i < 3; i++)
        {
            if (grid[i, 0] == player && grid[i, 1] == player && grid[i, 2] == player)
            {
                winLineIndex = i; // Линии 0, 1, 2 для горизонталей
                return true;
            }
        }

        // Проверка столбцов
        for (int i = 0; i < 3; i++)
        {
            if (grid[0, i] == player && grid[1, i] == player && grid[2, i] == player)
            {
                winLineIndex = 3 + i; // Линии 3, 4, 5 для вертикалей
                return true;
            }
        }

        // Проверка диагоналей
        if (grid[0, 0] == player && grid[1, 1] == player && grid[2, 2] == player)
        {
            winLineIndex = 6; // Диагональ сверху слева направо
            return true;
        }
        if (grid[0, 2] == player && grid[1, 1] == player && grid[2, 0] == player)
        {
            winLineIndex = 7; // Диагональ сверху справа налево
            return true;
        }

        return false;
    }

    bool CheckDraw()
    {
        foreach (var cell in grid)
        {
            if (cell == Player.None) return false;
        }
        return true;
    }

    // Метод для сброса игры (исправление для белых квадратов)
    public void ResetGame()
    {
        grid = new Player[3, 3];
        currentPlayer = Player.PlayerX;
        isGameActive = true;

        // Сбрасываем спрайты (клетки будут "пустыми" в начале)
        foreach (Button button in gridButtons)
        {
            button.GetComponent<Image>().color = new Color(1, 1, 1, 0); // Убираем спрайты для "пустых" клеток
            button.interactable = true;
        }

        // Скрываем все победные линии
        foreach (GameObject line in winLines)
        {
            line.SetActive(false);
        }

        // Обновляем уровень и очки
        levelText.text = "Level: " + currentLevel;
        _winLevelText.text = "Level: " + currentLevel;
        _loseLevelText.text = "Level: " + currentLevel;
        scoreText.text = "Score: " + currentScore;

        // Текущий ход — игрок X
        currentPlayerText.text = "Player X's turn";

        // Скрываем панели победы/поражения и кнопки продолжения
        gameField.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        continueWinButton.gameObject.SetActive(false);
        continueLoseButton.gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        StartCoroutine(ExitBehavior());
    }

    private IEnumerator ExitBehavior()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("MenuScene");
    }
}
