using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;
using VRTK.Examples;
using TMPro;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class GameManager : MonoBehaviour
{

    public enum eGameState
    {
        EDIT,
        PLAY,
        MAINMENU,
        PAUSE
    }

    public enum eEnvironmentType
    {
        REALISTIC_FOREST,
        POLY_FOREST,
        BEDROOM
    }

    public eGameState GameState { get; private set; }
    //For maintain which state the game was in before opening the pause menu
    public eGameState PreviousGameState { get; private set; }
    public eEnvironmentType CurrentEnvironment { get; private set; }
    public string[] CurrentEnvironmentCategories { get; private set; }
    public List<GameObject>[] ObjectsSortedByCategory;
    [SerializeField]
    private GameObject m_buttonTemplate = null;
    [SerializeField]
    public GameObject m_MainObjectStorage = null;
    [SerializeField]
    private GameObject m_pauseMenu = null;
    [SerializeField]
    private GameObject m_musicMenu = null;
    [SerializeField]
    private GameObject m_loadingScreenVisual = null;
    [SerializeField]
    private GameObject m_mainMenu = null;
    [SerializeField]
    private GameObject m_mainMenuObject = null;
    [SerializeField]
    private GameObject m_loadMenu = null;
    [SerializeField]
    private GameObject m_saveMenu = null;

    public GameObject PlacedObjectStorage = null;

    public AudioSource AudioPlayer { get; private set; }
    private SceneLoader m_sceneLoader = null;

    public float GLOBAL_FLOOR_HEIGHT { get; private set; }

    #region AssetBundleStuff
    //Realistic Forest Asset Bundle
    private LoadRealisticForest m_realisticForestLoader = null;
    public AssetBundle RealistForestAssetBundle { get; private set; }
    public string[] AllRealisticForestNames { get; private set; }
    public string[] AllRealisticForestSimplifiedNames { get; private set; }

    //Julian Ray Asset Bundle
    private MusicManager m_musicManager = null;
    public AssetBundle JulianRayAssetBundle { get; private set; }
    public string[] AllJulianRayAssetNames { get; private set; }
    #endregion

    #region PlayerControllers
    [SerializeField]
    public GameObject m_leftController = null;
    [SerializeField]
    public GameObject m_rightController = null;

    private VRTK_ControllerEvents m_leftControllerEvents = null;
    private VRTK_ControllerEvents m_rightControllerEvents = null;

    //Controller_Menu m_leftMenu = null;
    //Controller_Menu m_rightMenu = null;
    #endregion

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        GLOBAL_FLOOR_HEIGHT = -1.0f;
        GameState = eGameState.MAINMENU;
        PreviousGameState = GameState;
        CurrentEnvironment = eEnvironmentType.REALISTIC_FOREST;

        if (CurrentEnvironment == eEnvironmentType.REALISTIC_FOREST)
        {
            CurrentEnvironmentCategories = new string[] { "Plants", "Trees", "Rocks", "Props" };
            ObjectsSortedByCategory = new List<GameObject>[CurrentEnvironmentCategories.Length];
            for (int i = 0; i < CurrentEnvironmentCategories.Length; i++)
            {
                ObjectsSortedByCategory[i] = new List<GameObject>();
            }
        }

        //m_leftMenu = m_leftController.GetComponent<Controller_Menu>();
        //m_rightMenu = m_rightController.GetComponent<Controller_Menu>();
        AudioPlayer = GetComponent<AudioSource>();

        m_leftControllerEvents = m_leftController.GetComponent<VRTK_ControllerEvents>();
        m_rightControllerEvents = m_rightController.GetComponent<VRTK_ControllerEvents>();
        m_leftControllerEvents.ButtonTwoPressed += OpenPauseMenuOnButtonTwoPress;
        m_rightControllerEvents.ButtonTwoPressed += OpenPauseMenuOnButtonTwoPress;

        m_sceneLoader = GetComponent<SceneLoader>();
    }

    private void Start()
    {
        m_sceneLoader.SetLoadingText(m_loadingScreenVisual.GetComponentInChildren<TextMeshProUGUI>());

        FindAndSavePauseMenu();
        //GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        //Button playButton = mainMenu.GetComponentInChildren<Button>();
        //playButton.onClick.AddListener(delegate { LoadRealisticForestScene(); });
    }

    private void Update()
    {

    }

    #region LoadRealisticForest
    public void LoadRealisticForestScene()
    {
        m_loadingScreenVisual.SetActive(true);
        StartCoroutine(m_sceneLoader.LoadSceneByName("RealisticForest"));
        m_mainMenuObject.SetActive(false);
        m_musicMenu.SetActive(true);
        m_musicManager = FindObjectOfType<MusicManager>();
        m_musicManager.GetComponent<Canvas>().enabled = false;
        print("Load realistic forest scene called");
    }

    public void FinalizeLoadingRealisticForest()
    {
        //FindAndSavePauseMenu();
        //FindAndSaveMusicMenu();
        
        m_realisticForestLoader = FindObjectOfType<LoadRealisticForest>();
        RealistForestAssetBundle = m_realisticForestLoader.RealistForestAssetBundle;
        if(AllRealisticForestNames == null)
        {
            AllRealisticForestNames = m_realisticForestLoader.AllRealisticForestNames;
            AllRealisticForestSimplifiedNames = new string[AllRealisticForestNames.Length];
            CleanUpRealisticForestNamesAndMakeObjects();
        }
        if(JulianRayAssetBundle != null)
        {
            m_loadingScreenVisual.SetActive(false);
        }
    }

    private void CleanUpRealisticForestNamesAndMakeObjects()
    {
        for (int i = 0; i < AllRealisticForestNames.Length; i++)
        {
            string s = AllRealisticForestNames[i];
            //comes out like this
            //wanna trim it for user readability
            if (s.Contains("plants"))
            {
                //assets/polygonnature/prefabs/plants/sm_plant_hedge_bush_02.prefab
                GameObject obj = SpawnItemByName(s);
                obj.SetActive(false);
                ObjectsSortedByCategory[0].Add(obj);
                s = s.Replace("assets/polygonnature/prefabs/plants/sm_plant_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            else if (s.Contains("props"))
            {
                //assets/polygonnature/prefabs/props/sm_prop_campfire_01
                GameObject obj = SpawnItemByName(s);
                obj.SetActive(false);
                ObjectsSortedByCategory[3].Add(obj);
                s = s.Replace("assets/polygonnature/prefabs/props/sm_prop_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            else if (s.Contains("trees"))
            {
                //assets/polygonnature/prefabs/trees/sm_tree_generic_giant_01
                GameObject obj = SpawnItemByName(s);
                obj.SetActive(false);
                ObjectsSortedByCategory[1].Add(obj);
                s = s.Replace("assets/polygonnature/prefabs/trees/sm_tree_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            else if (s.Contains("rocks"))
            {
                //assets/polygonnature/prefabs/rocks/sm_rock_cluster_large_01
                GameObject obj = SpawnItemByName(s);
                obj.SetActive(false);
                ObjectsSortedByCategory[2].Add(obj);
                s = s.Replace("assets/polygonnature/prefabs/rocks/sm_rock_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            //print(s);
            AllRealisticForestSimplifiedNames[i] = s;
        }
    }
    #endregion

    #region UnloadRealisticForest
    private void UnloadRealisticForestScene()
    {
        RealistForestAssetBundle.Unload(true);
        m_sceneLoader.UnloadCurrentScene();
        m_musicManager.Stop();
        RealistForestAssetBundle = null;
        Destroy(m_realisticForestLoader.gameObject);
        m_mainMenuObject.SetActive(true);
        if(m_loadMenu.activeInHierarchy)
        {
            m_loadMenu.SetActive(false);
            m_mainMenu.SetActive(true);
        }
        GameState = eGameState.MAINMENU;
    }
    #endregion

    public void FinalizeLoadingJulianRayMusic()
    {
        PreviousGameState = GameState;
        GameState = eGameState.EDIT;
        m_loadingScreenVisual.SetActive(false);
        m_sceneLoader.m_loadScene = false;
        JulianRayAssetBundle = m_musicManager.JulianRayAssetBundle;
        AllJulianRayAssetNames = m_musicManager.JulianRayAssetNames;
        m_musicMenu.SetActive(false);
        m_musicManager.GetComponent<Canvas>().enabled = true;
        //FindAndSaveMusicMenu();
    }

    public GameObject SpawnItemByName(string name)
    {
        GameObject prefab = RealistForestAssetBundle.LoadAsset<GameObject>(name);
        if (!prefab.GetComponent<Rigidbody>())
        {
            prefab.AddComponent<BoxCollider>();

            BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
            boxCollider.center = renderer.bounds.center;
            boxCollider.size = renderer.bounds.size;

            Rigidbody body = prefab.AddComponent<Rigidbody>();
            body.isKinematic = true;
            //body.useGravity = false;
            //body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            VRTK_InteractableObject interactableObject = prefab.AddComponent<VRTK_InteractableObject>();
            interactableObject.allowedTouchControllers = VRTK_InteractableObject.AllowedController.Both;
            interactableObject.isGrabbable = true;
            interactableObject.holdButtonToGrab = false;
            interactableObject.stayGrabbedOnTeleport = true;
            interactableObject.holdButtonToUse = false;
            interactableObject.touchHighlightColor = Color.blue;

            VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter outline = prefab.AddComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
            outline.active = true;
            outline.unhighlightOnDisable = true;
            prefab.AddComponent<PlaceableObject>();
        }
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, m_MainObjectStorage.transform);
        obj.GetComponent<PlaceableObject>().ObjectName = name;
        return obj;
    }


    #region SavingAndLoading
    public SpaceData CreateSaveData()
    {
        SpaceData spaceData = new SpaceData();
        PlaceableObject[] objects = PlacedObjectStorage.GetComponentsInChildren<PlaceableObject>();
        spaceData.m_objectsInSpace = new ObjectData[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            ObjectData objData = new ObjectData();

            objData.AssetBundleName = objects[i].ObjectName;
            print(objects[i].ObjectName);
            objData.TransformLocation = objects[i].transform.position;
            objData.TransformRotation = objects[i].transform.rotation;
            objData.eObjectState = objects[i].ObjectState;

            spaceData.m_objectsInSpace[i] = objData;
        }

        spaceData.m_musicAssetBundle = "julianray";
        m_musicMenu.GetComponent<Canvas>().enabled = false;
        m_musicMenu.SetActive(true);
        spaceData.m_currentSongInSpaceName = m_musicManager.GetCurrentSongName();
        m_musicMenu.SetActive(false);
        m_musicMenu.GetComponent<Canvas>().enabled = true;

        return spaceData;
    }

    public void SaveGame(Button button)
    {
        SpaceData spaceData = CreateSaveData();

        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector surrogateSelector = new SurrogateSelector();
        Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quatSS = new QuaternionSerializationSurrogate();

        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
        surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quatSS);
        bf.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.persistentDataPath + "/" + CurrentEnvironment + ".space");
        bf.Serialize(file, spaceData);
        file.Close();
        print("Game saved");
        StartCoroutine(SavedGameAlert(button.GetComponentInChildren<TextMeshProUGUI>()));
    }

    private IEnumerator SavedGameAlert(TextMeshProUGUI text)
    {
        text.text = "Saved!";
        yield return new WaitForSeconds(0.3f);
        text.text = "Save";
    }

    public void LoadGame(string spaceName)
    {
        if (File.Exists(spaceName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            QuaternionSerializationSurrogate quatSS = new QuaternionSerializationSurrogate();

            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quatSS);
            bf.SurrogateSelector = surrogateSelector;

            FileStream file = File.Open(spaceName, FileMode.Open);
            SpaceData save = (SpaceData)bf.Deserialize(file);
            file.Close();

            //load sccene first then place all the objects
            LoadRealisticForestScene();
            StartCoroutine(LoadObjectsFromSave(save));
            print("saved song name: " + save.m_currentSongInSpaceName);
            StartCoroutine(LoadMusicAndPlay(save.m_currentSongInSpaceName));
            print("Game loaded");
        }
        else
        {
            print("Save not found");
        }
    }

    public IEnumerator LoadMusicAndPlay(string songName)
    {
        bool loadedSong = false;
        if (JulianRayAssetBundle == null)
        {
            print("Julian ray asset bundle null, loading it now");
            if (m_musicManager == null) m_musicManager = FindObjectOfType<MusicManager>();
            AssetBundleCreateRequest assetRequest = m_musicManager.LoadedAssetBundle;
            while (!assetRequest.isDone || !loadedSong)
            {
                assetRequest = m_musicManager.LoadedAssetBundle;
                if (assetRequest.isDone)
                {
                    print(songName);
                    m_musicManager.PlaySongByName(songName);

                    loadedSong = true;
                }
                yield return null;
            }
        } else
        {
            print("julian ray asset bundle not null, playing song");
            if (m_musicManager == null) m_musicManager = FindObjectOfType<MusicManager>();
            m_musicManager.PlaySongByName(songName);
            m_musicMenu.SetActive(false);
            m_musicManager.GetComponent<Canvas>().enabled = true;
        }
        print("left the load music and play coroutine");
    }

    public IEnumerator LoadObjectsFromSave(SpaceData save)
    {
        print("there are " + save.m_objectsInSpace.Length + "objects in this save");
        for (int i = 0; i < save.m_objectsInSpace.Length; i++)
        {
            if(m_realisticForestLoader == null)
            {
                m_realisticForestLoader = GetComponent<LoadRealisticForest>();
                i = -1;
            }
            else
            {
                print("successfully grabbed realistic forest loader");
                AssetBundleCreateRequest assetRequest = m_realisticForestLoader.LoadedAssetBundle;
                if (assetRequest.isDone)
                {
                    ObjectData objData = save.m_objectsInSpace[i];
                    print("asset bundle loaded and loading object: " + objData.AssetBundleName);
                    GameObject obj = SpawnItemByName(objData.AssetBundleName);
                    obj.transform.position = objData.TransformLocation;
                    obj.transform.rotation = objData.TransformRotation;
                    obj.transform.SetParent(PlacedObjectStorage.transform);
                    obj.GetComponent<PlaceableObject>().ObjectState = objData.eObjectState;
                }
                else
                {
                    i = -1;
                }

            }

            yield return null;
        }
        print("left the loading objects coroutine");
    }
    #endregion

    #region MenuFunctions
    public void FindAndSavePauseMenu()
    {
        m_pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        if (m_pauseMenu != null && m_pauseMenu.activeInHierarchy)
        {
            m_pauseMenu.SetActive(false);
        }
    }

    public void FindAndSaveMusicMenu()
    {
        m_musicMenu = GameObject.FindGameObjectWithTag("MusicMenu");
        if (m_musicMenu != null && m_musicMenu.activeInHierarchy)
        {
            m_musicMenu.SetActive(false);
        }
    }

    private void OpenPauseMenuOnButtonTwoPress(object sender, ControllerInteractionEventArgs e)
    {
        //Enable the pause menu and change the game state to pause only if not already paused or not in main menu
        if (GameState != eGameState.PAUSE && GameState != eGameState.MAINMENU)
        {
            PreviousGameState = GameState;
            GameState = eGameState.PAUSE;
            //m_leftMenu.ForceCloseMenu();
            //m_rightMenu.ForceCloseMenu();
            m_pauseMenu.SetActive(true);
            //When game pauses probable disable the entire level and open up the GUI
        }
        else if (GameState == eGameState.PAUSE)
        {
            ClosePauseMenu();
        }
    }

    //This will be hooked up to a GUI button
    public void ClosePauseMenu()
    {
        print(GameState);
        if (GameState == eGameState.PAUSE)
        {
            GameState = PreviousGameState;
            m_pauseMenu.SetActive(false);
        }
    }

    public void OpenMusicMenu()
    {
        m_musicMenu.SetActive(true);
        m_pauseMenu.SetActive(false);
    }

    public void CloseMusicMenu()
    {
        m_pauseMenu.SetActive(true);
        GameState = eGameState.PAUSE;
        m_musicMenu.SetActive(false);
    }

    public void OpenLoadMenu()
    {
        //Find the local saves and create buttons based off of them, if any
        foreach (Button obj in m_loadMenu.GetComponentsInChildren<Button>())
        {
            if(obj.gameObject.name != "Back") Destroy(obj.gameObject);
        }
        print(Application.persistentDataPath);
        foreach (string file in Directory.GetFiles(Application.persistentDataPath))
        {
            print("filename found using directory: " + file);

            CreateButtonForLoadMenu(file);
        }
        m_loadMenu.SetActive(true);
        m_mainMenu.SetActive(false);
    }

    private void CreateButtonForLoadMenu(string name)
    {
        //appears as C:/Users/Dakota/AppData/LocalLow/DarkPluto/HappyPlace\REALISTIC_FOREST.space
        //need it to only have REALISTIC_FOREST
        string s = name;
        string nameOfGame = "/HappyPlace";
        int indexOfName = s.IndexOf(nameOfGame);
        s = s.Remove(0, indexOfName + nameOfGame.Length + 1);
        s = s.Replace(".space", string.Empty);
        print("name for button: " + s);
        GameObject obj = Instantiate(m_buttonTemplate, m_loadMenu.transform);
        Button newButton = obj.GetComponent<Button>();
        obj.GetComponentInChildren<TextMeshProUGUI>().text = s;
        string filePath = name.Replace("\\", "/");
        print("filepath for loading: " + filePath);
        newButton.onClick.AddListener(delegate { LoadGame(filePath); });
        obj.transform.SetAsFirstSibling();
    }

    public void GoBackToMainMenu()
    {
        m_pauseMenu.SetActive(false);
        UnloadRealisticForestScene();
    }

    public void CloseLoadMenu()
    {
        m_mainMenu.SetActive(true);
        m_loadMenu.SetActive(false);
    }

    public void OpenSaveMenu()
    {
        m_saveMenu.SetActive(true);
        m_pauseMenu.SetActive(false);
    }

    public void CloseSaveMenu()
    {
        m_pauseMenu.SetActive(true);
        m_saveMenu.SetActive(false);
    }
    #endregion
}
