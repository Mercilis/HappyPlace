using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private string m_currentScene = "";
    public bool m_loadScene = false;
    
    [SerializeField]
    private TextMeshProUGUI m_loadingText;

    private GameManager m_gameManager = null;

    private void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();
    }

    public void SetLoadingText(TextMeshProUGUI loadingText)
    {
        m_loadingText = loadingText;
    }

    // Updates once per frame
    void Update()
    {
        // If the new scene has started loading...
        if (m_loadScene == true)
        {

            // ...then pulse the transparency of the loading text to let the player know that the computer is still working.
            m_loadingText.color = new Color(m_loadingText.color.r, m_loadingText.color.g, m_loadingText.color.b, Mathf.PingPong(Time.time, 1));

        }

    }

    public IEnumerator LoadSceneByName(string name)
    {
        print("load scene by name called");
        m_loadScene = true;
        AsyncOperation async = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        m_currentScene = name;

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            print("async not done");
            yield return null;
        }
    }

    public void UnloadCurrentScene()
    {
        if(m_currentScene != string.Empty)
        {
            SceneManager.UnloadSceneAsync(m_currentScene);
        }
    }
}
