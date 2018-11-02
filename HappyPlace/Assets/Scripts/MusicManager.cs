using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    private AudioSource m_audioSource = null;
    private AssetBundle m_julianRayAssetBundle;
    private string[] m_julianRayAssetNames;
    private AudioClip m_currentSong = null;
	
	void Start () {
        //load music from asset bundles?
        var loadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "julianray"));
        if (loadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle julianray!");
            return;
        }
        m_julianRayAssetBundle = loadedAssetBundle;
        m_julianRayAssetNames = m_julianRayAssetBundle.GetAllAssetNames();

        m_audioSource = GetComponent<AudioSource>();
    }

    public void PlaySongByName(string name)
    {

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
