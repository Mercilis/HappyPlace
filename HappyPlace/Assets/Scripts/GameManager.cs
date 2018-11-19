using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;
using VRTK.Examples;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

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
    private GameObject m_MainObjectStorage = null;
    [SerializeField]
    private GameObject m_pauseMenu = null;
    [SerializeField]
    private GameObject m_musicMenu = null;
    [SerializeField]
    private GameObject m_loadingScreenVisual = null;
    [SerializeField]
    private GameObject m_mainMenu = null;

    public GameObject PlaceObjectStorage = null;

    public AudioSource AudioPlayer { get; private set; }
    private SceneLoader m_sceneLoader = null;
    
    public float GLOBAL_FLOOR_HEIGHT { get; private set; }

    #region AssetBundleStuff
    private LoadRealisticForest m_realisticForestLoader = null;
    private AssetBundle m_realistForestAssetBundle;
    public string[] AllRealisticForestNames { get; private set; }
    public string[] AllRealisticForestSimplifiedNames { get; private set; }
    #endregion

    #region PlayerControllers
    [SerializeField]
    public GameObject m_leftController = null;
    [SerializeField]
    public GameObject m_rightController = null;

    private VRTK_ControllerEvents m_leftControllerEvents = null;
    private VRTK_ControllerEvents m_rightControllerEvents = null;

    Controller_Menu m_leftMenu = null;
    Controller_Menu m_rightMenu = null;
    #endregion

    private void Awake()
    {
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

        m_leftMenu = m_leftController.GetComponent<Controller_Menu>();
        m_rightMenu = m_rightController.GetComponent<Controller_Menu>();
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
        m_mainMenu.SetActive(false);
        print("Load realistic forest scene called");
    }

    public void FinalizeLoadingRealisticForest()
    {
        //FindAndSavePauseMenu();
        //FindAndSaveMusicMenu();
        PreviousGameState = GameState;
        GameState = eGameState.EDIT;
        m_loadingScreenVisual.SetActive(false);
        m_sceneLoader.m_loadScene = false;
        m_realistForestAssetBundle = m_realisticForestLoader.RealistForestAssetBundle;
        AllRealisticForestNames = m_realisticForestLoader.AllRealisticForestNames;
        AllRealisticForestSimplifiedNames = new string[AllRealisticForestNames.Length];
        CleanUpRealisticForestNamesAndMakeObjects();
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

    public GameObject SpawnItemByName(string name)
    {
        GameObject prefab = m_realistForestAssetBundle.LoadAsset<GameObject>(name);
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
            prefab.AddComponent<PlaceableObject>().ObjectName = name;
        }
        return Instantiate(prefab, Vector3.zero, Quaternion.identity, m_MainObjectStorage.transform);
    }


    #region SavingAndLoading
    public SpaceData CreateSaveData()
    {
        SpaceData spaceData = new SpaceData();
        PlaceableObject[] objects = PlaceObjectStorage.GetComponentsInChildren<PlaceableObject>();
        spaceData.m_objectsInSpace = new ObjectData[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            ObjectData objData = new ObjectData();

            objData.AssetBundleName = objects[i].ObjectName;
            objData.TransformLocation = objects[i].transform.position;
            objData.TransformRotation = objects[i].transform.rotation;

            spaceData.m_objectsInSpace[i] = objData;
        }

        spaceData.m_musicAssetBundle = "julianray";
        spaceData.m_currentSongInSpaceName = FindObjectOfType<MusicManager>().GetCurrentSongName();

        return spaceData;
    }

    public void SaveGame()
    {
        SpaceData spaceData = CreateSaveData();


    }

    public void LoadGame()
    {

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
        if(GameState != eGameState.PAUSE && GameState != eGameState.MAINMENU)
        {
            PreviousGameState = GameState;
            GameState = eGameState.PAUSE;
            m_leftMenu.ForceCloseMenu();
            m_rightMenu.ForceCloseMenu();
            m_pauseMenu.SetActive(true);
            //When game pauses probable disable the entire level and open up the GUI
        } else if(GameState == eGameState.PAUSE)
        {
            ClosePauseMenu();
        }
    }

    //This will be hooked up to a GUI button
    public void ClosePauseMenu()
    {
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
        m_musicMenu.SetActive(false);
    }
    #endregion
}
