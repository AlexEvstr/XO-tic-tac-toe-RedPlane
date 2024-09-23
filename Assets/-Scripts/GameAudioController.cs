using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _soundManager;
    [SerializeField] private AudioSource _musicManager;

    [SerializeField] private AudioClip _xSound;
    [SerializeField] private AudioClip _oSound;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _loseSound;
    [SerializeField] private AudioClip _lineSound;
    [SerializeField] private AudioClip _drawSound;
    [SerializeField] private AudioClip _startGameSound;

    private void Start()
    {
        _soundManager.volume = PlayerPrefs.GetFloat("SoundSource", 1);
        float musicVolume = PlayerPrefs.GetFloat("MusicSource", 1);
        if (musicVolume == 1)
        {
            _musicManager.volume = 0.35f;
        }
    }

    public void PlayXSound()
    {
        _soundManager.PlayOneShot(_xSound);
    }

    public void PlayOSound()
    {
        _soundManager.PlayOneShot(_oSound);
    }

    public void PlayClickSound()
    {
        _soundManager.PlayOneShot(_clickSound);
    }

    public void PlayWinSound()
    {
        _soundManager.PlayOneShot(_winSound);
    }

    public void PlayLineSound()
    {
        _soundManager.PlayOneShot(_lineSound);
    }

    public void PlayDrawSound()
    {
        _soundManager.PlayOneShot(_drawSound);
    }

    public void PlayLoseSound()
    {
        _soundManager.PlayOneShot(_loseSound);
    }

    public void PlayStartGameSound()
    {
        _soundManager.PlayOneShot(_startGameSound);
    }

    public void MuteMusic()
    {
        _musicManager.volume = 0;
    }

    public void UnMuteMusic()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicSource", 1);
        if (musicVolume == 1)
        {
            _musicManager.volume = 0.35f;
        }
    }
}