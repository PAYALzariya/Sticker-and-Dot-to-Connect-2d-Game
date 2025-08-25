using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneManager : MonoBehaviour
{
    public static HomeSceneManager Instance;
    public GameObject LoadingScreen, MainMenuScreen, LevelScreen;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadGameStarting());
    }
    IEnumerator LoadGameStarting()
    {
        LoadingScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        LoadingScreen.SetActive(false);
        MainMenuScreen.SetActive(true);
    }
    public void ChangeScene(int currentlevel)
    {
        PlayerPrefs.SetInt("CurrentLevel", currentlevel);
        SceneManager.LoadScene("GamePlay");
    }
}
