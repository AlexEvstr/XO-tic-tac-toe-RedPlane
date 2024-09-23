using UnityEngine;
using UnityEngine.Advertisements;

public class UnityBannerAd : MonoBehaviour
{
    [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID";
    [SerializeField] private string iosGameId = "YOUR_IOS_GAME_ID";
    [SerializeField] private string androidBannerAdUnitId = "Banner_Android";
    [SerializeField] private string iosBannerAdUnitId = "Banner_iOS";
    private string bannerAdUnitId;
    private string gameId;
    private bool isBannerLoaded = false;
    private float retryDelay = 5f; // Задержка перед повторной попыткой (в секундах)

    void Start()
    {
        // Определяем Game ID и Ad Unit ID в зависимости от платформы
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameId = iosGameId;
            bannerAdUnitId = iosBannerAdUnitId;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            gameId = androidGameId;
            bannerAdUnitId = androidBannerAdUnitId;
        }

        // Проверяем, установлен ли Game ID
        if (string.IsNullOrEmpty(gameId))
        {
            Debug.LogError("Game ID is not set. Please set the Game ID for your platform.");
            return;
        }

        // Инициализируем Unity Ads
        Advertisement.Initialize(gameId, testMode: false); // Для production используйте testMode: false

        // Устанавливаем позицию баннера (например, внизу экрана)
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        // Загружаем баннер
        LoadBanner();
    }

    // Загружаем баннер каждый раз, когда сцена активируется
    void OnEnable()
    {
        LoadBanner();
    }

    // Метод для загрузки баннера с автоматической повторной попыткой
    public void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Unity Ads not initialized.");
            return;
        }

        Debug.Log("Attempting to load Banner...");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(bannerAdUnitId, options);
    }

    // Метод для показа баннера
    public void ShowBanner()
    {
        if (isBannerLoaded)
        {
            Advertisement.Banner.Show(bannerAdUnitId);
            Debug.Log("Banner shown.");
        }
        else
        {
            Debug.Log("Banner is not loaded yet.");
        }
    }

    // Коллбэк при успешной загрузке баннера
    void OnBannerLoaded()
    {
        isBannerLoaded = true;
        Debug.Log("Banner loaded successfully.");
        ShowBanner(); // Автоматически показываем баннер после загрузки

        // Останавливаем повторные попытки после успешной загрузки
        CancelInvoke("LoadBanner");
    }

    // Коллбэк при ошибке загрузки баннера
    void OnBannerError(string message)
    {
        Debug.LogError("Banner failed to load: " + message);

        // Если баннер не загрузился, запускаем повторную попытку через некоторое время
        Debug.Log("Retrying to load banner in " + retryDelay + " seconds...");
        Invoke("LoadBanner", retryDelay); // Повторная попытка через 5 секунд
    }

    // Метод для скрытия баннера при выходе из сцены
    public void HideBanner()
    {
        Advertisement.Banner.Hide();
        Debug.Log("Banner hidden.");
    }
}
