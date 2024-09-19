using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Image loadingBar;
    private float loadingTime = 3f;

    private float elapsedTime = 0f;

    void Start()
    {
        loadingBar.fillAmount = 0f;
        StartCoroutine(LoadBar());
    }

    IEnumerator LoadBar()
    {
        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            loadingBar.fillAmount = Mathf.Clamp01(elapsedTime / loadingTime); // Заполняем бар
            yield return null;
        }

        SceneManager.LoadScene("MenuScene");
    }
}