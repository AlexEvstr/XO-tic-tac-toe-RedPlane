using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private GameObject _optionsWindow;
    [SerializeField] private GameObject _privacyWindow;

    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _saveOptionsButton;
    [SerializeField] private Button _exitOptionsButton;

    [SerializeField] private AudioSource _soundManager;
    [SerializeField] private AudioSource _musicManager;

    [SerializeField] private AudioClip _clickSound;

    [SerializeField] private Button _privacyButton;
    [SerializeField] private Button _exitPrivacyButton;
    [SerializeField] private Button _soundOffButton;
    [SerializeField] private Button _soundOnButton;
    [SerializeField] private Button _musicOffButton;
    [SerializeField] private Button _musicOnButton;

    private void Start()
    {
        _saveOptionsButton.onClick.AddListener(CloseOptions);
        _exitOptionsButton.onClick.AddListener(CloseOptions);
        _optionsButton.onClick.AddListener(OpenOptions);

        _privacyButton.onClick.AddListener(OpenPrivacyWindow);
        _exitPrivacyButton.onClick.AddListener(ClosePrivacyWindow);

        _soundOffButton.onClick.AddListener(EnableSounds);
        _soundOnButton.onClick.AddListener(DisabeSounds);
        _musicOffButton.onClick.AddListener(EnableMusic);
        _musicOnButton.onClick.AddListener(DisableMusic);

        _soundManager.volume = PlayerPrefs.GetFloat("SoundSource", 1);
        if (_soundManager.volume == 1) EnableSounds();
        else DisabeSounds();

        _musicManager.volume = PlayerPrefs.GetFloat("MusicSource", 1);
        if (_musicManager.volume == 1) EnableMusic();
        else DisableMusic();

        _saveOptionsButton.gameObject.SetActive(false);
        _exitOptionsButton.gameObject.SetActive(true);
    }

    private void CloseOptions()
    {
        _saveOptionsButton.gameObject.SetActive(false);
        _exitOptionsButton.gameObject.SetActive(true);
        _optionsWindow.SetActive(false);
        _menuWindow.SetActive(true);
    }

    public void DisabeSounds()
    {
        _soundOnButton.gameObject.SetActive(false);
        _soundOffButton.gameObject.SetActive(true);
        _soundManager.volume = 0;
        PlayerPrefs.SetFloat("SoundSource", 0);

        _saveOptionsButton.gameObject.SetActive(true);
        _exitOptionsButton.gameObject.SetActive(false);
    }

    public void EnableSounds()
    {
        _soundOffButton.gameObject.SetActive(false);
        _soundOnButton.gameObject.SetActive(true);
        _soundManager.volume = 1;
        PlayerPrefs.SetFloat("SoundSource", 1);

        _saveOptionsButton.gameObject.SetActive(true);
        _exitOptionsButton.gameObject.SetActive(false);
    }

    public void DisableMusic()
    {
        _musicOnButton.gameObject.SetActive(false);
        _musicOffButton.gameObject.SetActive(true);
        _musicManager.volume = 0;
        PlayerPrefs.SetFloat("MusicSource", 0);

        _saveOptionsButton.gameObject.SetActive(true);
        _exitOptionsButton.gameObject.SetActive(false);
    }

    public void EnableMusic()
    {
        _musicOffButton.gameObject.SetActive(false);
        _musicOnButton.gameObject.SetActive(true);
        _musicManager.volume = 1;
        PlayerPrefs.SetFloat("MusicSource", 1);

        _saveOptionsButton.gameObject.SetActive(true);
        _exitOptionsButton.gameObject.SetActive(false);
    }

    private void OpenOptions()
    {
        _menuWindow.SetActive(false);
        _optionsWindow.SetActive(true);
    }

    private void OpenPrivacyWindow()
    {
        _optionsWindow.SetActive(false);
        _privacyWindow.SetActive(true);
    }

    private void ClosePrivacyWindow()
    {
        _privacyWindow.SetActive(false);
        _optionsWindow.SetActive(true);
    }

    public void ClickSound()
    {
        _soundManager.PlayOneShot(_clickSound);
    }
}