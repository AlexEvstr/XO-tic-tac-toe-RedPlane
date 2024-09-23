using UnityEngine;
using UnityEngine.Advertisements;

public class GameAdManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID";
    [SerializeField] private string iosGameId = "YOUR_IOS_GAME_ID";
    [SerializeField] private string androidInterstitialAdUnitId = "Interstitial_Android";
    [SerializeField] private string iosInterstitialAdUnitId = "Interstitial_iOS";
    private string gameId;
    private string interstitialAdUnitId;
    private bool isInterstitialAdLoaded = false;

    // Счётчики вызовов методов
    private int adCallCount1of5 = 0;
    private int adCallCount1of10 = 0;

    void Start()
    {
        // Определяем Game ID и Ad Unit ID для межстраничной рекламы в зависимости от платформы
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameId = iosGameId;
            interstitialAdUnitId = iosInterstitialAdUnitId;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            gameId = androidGameId;
            interstitialAdUnitId = androidInterstitialAdUnitId;
        }

        // Инициализируем Unity Ads
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode: false, this);
        }
        else
        {
            LoadInterstitialAd();
        }
    }

    // Метод для загрузки межстраничной рекламы
    public void LoadInterstitialAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Unity Ads is not initialized.");
            return;
        }

        Debug.Log("Loading Interstitial Ad...");
        Advertisement.Load(interstitialAdUnitId, new UnityAdsLoadListener(this));
    }

    // Метод для показа рекламы при каждом вызове
    public void ShowAdEveryTime(System.Action onAdCompletedAction)
    {
        ShowAd(onAdCompletedAction);
    }

    // Метод для показа рекламы 1 раз из 5 вызовов
    public void ShowAd1Of5(System.Action onAdCompletedAction)
    {
        adCallCount1of5++;
        if (adCallCount1of5 >= 5)
        {
            ShowAd(onAdCompletedAction); // Показываем рекламу
            adCallCount1of5 = 0; // Сброс счётчика после показа
        }
        else
        {
            Debug.Log($"Ad will be shown after {5 - adCallCount1of5} more calls.");
            onAdCompletedAction.Invoke(); // Вызываем ResetGame, если реклама не показывается
        }
    }

    // Метод для показа рекламы 1 раз из 10 вызовов
    public void ShowAd1Of10(System.Action onAdCompletedAction)
    {
        adCallCount1of10++;
        if (adCallCount1of10 >= 10)
        {
            ShowAd(onAdCompletedAction); // Показываем рекламу
            adCallCount1of10 = 0; // Сброс счётчика после показа
        }
        else
        {
            Debug.Log($"Ad will be shown after {10 - adCallCount1of10} more calls.");
            onAdCompletedAction.Invoke(); // Вызываем ResetGame, если реклама не показывается
        }
    }

    // Общий метод показа рекламы
    private void ShowAd(System.Action onAdCompletedAction)
    {
        if (isInterstitialAdLoaded)
        {
            OnAdCompleted = onAdCompletedAction; // Сохраняем действие, которое будет выполнено после рекламы
            Advertisement.Show(interstitialAdUnitId, new UnityAdsShowListener(this));
        }
        else
        {
            Debug.Log("Interstitial ad is not loaded yet.");
            onAdCompletedAction.Invoke(); // Вызываем действие, если реклама не загружена
        }
    }

    // Действие, которое выполняется после просмотра рекламы
    public System.Action OnAdCompleted;

    // Реализация интерфейса IUnityAdsInitializationListener
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadInterstitialAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Обработчики для загрузки межстраничной рекламы
    class UnityAdsLoadListener : IUnityAdsLoadListener
    {
        private GameAdManager adManager;

        public UnityAdsLoadListener(GameAdManager manager)
        {
            adManager = manager;
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Interstitial Ad loaded: " + adUnitId);
            adManager.isInterstitialAdLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Failed to load Interstitial Ad Unit {adUnitId}: {error.ToString()} - {message}");
            adManager.Invoke("LoadInterstitialAd", 5f); // Повторная попытка через 5 секунд
        }
    }

    // Обработчики для показа межстраничной рекламы
    class UnityAdsShowListener : IUnityAdsShowListener
    {
        private GameAdManager adManager;

        public UnityAdsShowListener(GameAdManager manager)
        {
            adManager = manager;
        }

        public void OnUnityAdsShowStart(string adUnitId)
        {
            Debug.Log("Interstitial Ad started showing: " + adUnitId);
        }

        public void OnUnityAdsShowClick(string adUnitId)
        {
            Debug.Log("Interstitial Ad clicked: " + adUnitId);
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log("Interstitial Ad completed showing: " + adUnitId);

            // Выполняем сохранённое действие после завершения показа рекламы
            if (adManager.OnAdCompleted != null)
            {
                adManager.OnAdCompleted.Invoke();
            }

            adManager.isInterstitialAdLoaded = false; // Сбрасываем флаг после показа рекламы
            adManager.LoadInterstitialAd(); // Загружаем новую рекламу после показа
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Failed to show Interstitial Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }
    }
}
