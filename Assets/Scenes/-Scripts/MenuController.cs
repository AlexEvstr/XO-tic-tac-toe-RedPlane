using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _privacyPolicyButton;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _exitPrivacyButton;

    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private GameObject _onBoardWindow;
    [SerializeField] private GameObject _privacyWindow;

    private void Start()
    {
        int firstEnter = PlayerPrefs.GetInt("FirstEnterOnBoard", 0);
        if (firstEnter != 0) StartGame();

        _privacyPolicyButton.onClick.AddListener(OpenPrivacy);
        _okButton.onClick.AddListener(AcceptPrivacy);
        _startGameButton.onClick.AddListener(StartGame);
        _exitPrivacyButton.onClick.AddListener(ClosePrivacy);

        _startGameButton.interactable = false;
    }

    private void OpenPrivacy()
    {
        _onBoardWindow.SetActive(false);
        _privacyWindow.SetActive(true);
    }

    private void ClosePrivacy()
    {
        _privacyWindow.SetActive(false);
        _onBoardWindow.SetActive(true);
    }


    private void AcceptPrivacy()
    {
        _startGameButton.interactable = true;
    }

    private void StartGame()
    {
        _onBoardWindow.SetActive(false);
        _menuWindow.SetActive(true);
        PlayerPrefs.SetInt("FirstEnterOnBoard", 1);
    }
}