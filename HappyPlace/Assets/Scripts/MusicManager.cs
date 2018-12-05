using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour {

    private List<Button> m_menuButtons = new List<Button>();
    [SerializeField]
    private GameObject m_songContainer = null;
    [SerializeField]
    private GameObject m_buttonTemplate = null;

    private GameManager m_gameManager = null;

    [SerializeField]
    private AudioSource m_audioSource = null;
    public AssetBundle JulianRayAssetBundle { get; private set; }
    public AssetBundleCreateRequest LoadedAssetBundle { get; private set; }

    public string[] JulianRayAssetNames { get; private set; }
    private AudioClip m_currentSong = null;
    private string m_currentSongName = null;

    private bool m_done = false;

    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        //m_audioSource = m_gameManager.AudioPlayer;
        GetComponent<Canvas>().enabled = false;

        CreateJulianRayAssetBundleLoadRequest();
    }

    public void CreateJulianRayAssetBundleLoadRequest()
    {
        if (m_gameManager.JulianRayAssetBundle == null)
        {
            LoadedAssetBundle = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "julianray"));
            print("loading julian ray asset bundle");
        }

    }

    public string GetCurrentSongName()
    {
        return m_currentSongName != null ? m_currentSongName : null;
    }

   private void Update()
    {
        if(LoadedAssetBundle.isDone && !m_done && m_gameManager.JulianRayAssetBundle == null)
        {
            print("loaded julian ray asset bundle");
            m_done = true;
            JulianRayAssetBundle = LoadedAssetBundle.assetBundle;
            JulianRayAssetNames = JulianRayAssetBundle.GetAllAssetNames();

            //m_audioSource = GetComponent<AudioSource>();

            if (m_songContainer != null && m_buttonTemplate != null)
            {
                for (int i = 0; i < JulianRayAssetNames.Length; i++)
                {
                    GameObject obj = Instantiate(m_buttonTemplate, m_songContainer.transform);
                    Button newButton = obj.GetComponent<Button>();
                    //appears as "assets/audio/music/julianray/julian ray - smooth & jazzy - 08 strawberry moon.wav"
                    string songName = JulianRayAssetNames[i];
                    songName = songName.Replace("assets/audio/music/julianray/julian ray - smooth & jazzy - ", string.Empty);
                    songName = songName.Remove(0, 3);
                    songName = songName.Replace(".mp3", string.Empty);
                    print(songName);
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = songName;
                    newButton.onClick.AddListener(delegate { OnSongClicked(newButton); });
                    m_menuButtons.Add(newButton);
                }
            }
            m_gameManager.FinalizeLoadingJulianRayMusic();
        }
        
    }

    private void OnSongClicked(Button button)
    {
        int songNum = m_menuButtons.IndexOf(button);

        PlaySongByName(JulianRayAssetNames[songNum]);
    }

    public void PlaySongByName(string name)
    {
        if(name != null)
        {
            AudioClip clip = JulianRayAssetBundle.LoadAsset<AudioClip>(name);
            m_currentSong = clip;
            m_currentSongName = name;
            if(m_gameManager.GetComponent<AudioSource>().isPlaying)
            {
                Stop();
            }
            m_gameManager.GetComponent<AudioSource>().clip = m_currentSong;
            Play();
        }
    }

    public void Play()
    {
        m_gameManager.GetComponent<AudioSource>().Play();
    }

    public void Pause()
    {
        m_gameManager.GetComponent<AudioSource>().Pause();
    }

    public void Stop()
    {
        m_gameManager.GetComponent<AudioSource>().Stop();
    }
}
