using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject playCanvas;
    [SerializeField] private GameObject levelCanvas;
    public static SceneLoader instance;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadLevelScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode arg1)
    {
        SceneManager.SetActiveScene(scene);
        Debug.Log(SceneManager.GetActiveScene().name);
        LevelManager.Load();
    }

    public void LoadLevelCanvas()
    {
        playCanvas.SetActive(false);
        var info = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = info.GetFiles();
        int i = 0;
        levelCanvas.SetActive(true);
        foreach (Button bt in levelCanvas.GetComponentsInChildren<Button>())
        {
            bt.interactable = false;
        }
        foreach (Button bt in levelCanvas.GetComponentsInChildren<Button>())
        {
            string filename = "";
            if (i < files.Length)
                filename = Path.GetFileNameWithoutExtension(files[i].Name);
            else
                break;
            bt.GetComponentInChildren<TextMeshProUGUI>().text = filename;
            bt.interactable = true;
            i++;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else // only a single instance is allowed
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}