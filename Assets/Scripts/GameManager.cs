using System.Collections;
using SaveLoadSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject loadingScreen;

    public bool IsGamePaused { get; private set; }
    
    private void Awake()
    {
        if (Instance is null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MainMenu).isLoaded)
            {
                SceneManager.LoadSceneAsync((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public GameObject SpawnItem(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        return Instantiate(prefab, pos, rot);
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsGamePaused = true;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        AudioListener.pause = false;
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameWorldRoutine(false));
    }

    public void LoadGameFromSave()
    {
        StartCoroutine(LoadGameWorldRoutine(true));
    }

    public void QuitToMainMenu()
    {
        StartCoroutine(LoadMainMenuRoutine());
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    private IEnumerator LoadGameWorldRoutine(bool loadSavedData)
    {
        loadingScreen.SetActive(true);
        
        const int gameWorldIndex = (int)SceneIndexes.GameWorld;

        // Unload if already loaded
        Scene gameWorldScene = SceneManager.GetSceneByBuildIndex(gameWorldIndex);
        if (gameWorldScene.isLoaded)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(gameWorldIndex);
            yield return unloadOperation;
            
            gameWorldScene = SceneManager.GetSceneByBuildIndex(gameWorldIndex);
        }
        
        // Load game world
        if (!gameWorldScene.isLoaded)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(gameWorldIndex, LoadSceneMode.Additive);
            yield return loadOperation;
            
            gameWorldScene = SceneManager.GetSceneByBuildIndex(gameWorldIndex);
        }
        
        
        // Let all Awake() run
        yield return null;

        // Let all Start() run
        yield return null;
        
        SceneManager.SetActiveScene(gameWorldScene);

        // Unload main menu
        Scene mainMenuScene = SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MainMenu);
        if (mainMenuScene.isLoaded)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync((int)SceneIndexes.MainMenu);
            yield return unloadOperation;
        }
        
        // Load or create save data
        if (loadSavedData)
            SaveGameManager.Instance.LoadGame();
        else
        {
            SaveGameManager.Instance.ChangeProfileId("default");
            SaveGameManager.Instance.SaveGame();
        }
        
        yield return null;
        
        loadingScreen.SetActive(false);
    }

    private IEnumerator LoadMainMenuRoutine()
    {
        loadingScreen.SetActive(true);
        
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
        yield return loadOperation;
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MainMenu));
        
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync((int)SceneIndexes.GameWorld);
        yield return unloadOperation;
        
        yield return null;
        
        loadingScreen.SetActive(false);
    }
}
