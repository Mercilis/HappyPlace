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
    private Canvas m_musicMenuCanvas = null;
    [SerializeField]
    private GameObject m_buttonTemplate = null;

    private GameManager m_gameManager = null;

    private AudioSource m_audioSource = null;
    private AssetBundle m_julianRayAssetBundle;
    private string[] m_julianRayAssetNames;
    private AudioClip m_currentSong = null;

    private void Awake()
    {
        var loadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "julianray"));
        if (loadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle julianray!");
            return;
        }
        m_julianRayAssetBundle = loadedAssetBundle;
        m_julianRayAssetNames = m_julianRayAssetBundle.GetAllAssetNames();

        m_audioSource = GetComponent<AudioSource>();

        if (m_songContainer != null && m_buttonTemplate != null)
        {
            for (int i = 0; i < m_julianRayAssetNames.Length; i++)
            {
                GameObject obj = Instantiate(m_buttonTemplate, m_songContainer.transform);
                Button newButton = obj.GetComponent<Button>();
                //appears as "assets/audio/music/julianray/julian ray - smooth & jazzy - 08 strawberry moon.wav"
                string songName = m_julianRayAssetNames[i];
                songName = songName.Replace("assets/audio/music/julianray/julian ray - smooth & jazzy - ", string.Empty);
                songName = songName.Remove(0, 3);
                songName = songName.Replace(".wav", string.Empty);
                print(songName);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = songName;
                newButton.onClick.AddListener(delegate { OnSongClicked(newButton); });
                m_menuButtons.Add(newButton);
            }
        }
    }

    private void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_audioSource = m_gameManager.AudioPlayer;
    }


    private void OnSongClicked(Button button)
    {
        int songNum = m_menuButtons.IndexOf(button);

        PlaySongByName(m_julianRayAssetNames[songNum]);
    }

    public void PlaySongByName(string name)
    {
        AudioClip clip = m_julianRayAssetBundle.LoadAsset<AudioClip>(name);
        m_currentSong = clip;
        if(m_audioSource.isPlaying)
        {
            Stop();
        }
        m_audioSource.clip = m_currentSong;
        Play();
    }

    public void Play()
    {
        m_audioSource.Play();
    }

    public void Pause()
    {
        m_audioSource.Pause();
    }

    public void Stop()
    {
        m_audioSource.Stop();
    }
}
