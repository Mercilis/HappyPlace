namespace VRTK.Examples
{
    using UnityEngine;

    public class Controller_Menu : MonoBehaviour
    {
        public GameObject menuObject;

        private GameObject clonedMenuObject;

        private bool menuInit = false;
        private bool menuActive = false;

        private GameManager m_gameManager = null; //added by Dakota

        private void Start()
        {
            GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(DoMenuOn);
            GetComponent<VRTK_ControllerEvents>().ButtonTwoReleased += new ControllerInteractionEventHandler(DoMenuOff);
            menuInit = false;
            menuActive = false;
            m_gameManager = FindObjectOfType<GameManager>(); //added by Dakota
        }

        private void InitMenu()
        {
            clonedMenuObject = Instantiate(menuObject, transform.position, Quaternion.identity) as GameObject;
            clonedMenuObject.SetActive(true);
            menuInit = true;
        }

        private void DoMenuOn(object sender, ControllerInteractionEventArgs e)
        {
            if(m_gameManager.GameState == GameManager.eGameState.EDIT)
            {
                if (!menuInit)
                {
                    InitMenu();
                }
                if (clonedMenuObject != null)
                {
                    clonedMenuObject.SetActive(true);
                    menuActive = true;
                }
            }
        }

        private void DoMenuOff(object sender, ControllerInteractionEventArgs e)
        {
            if (m_gameManager.GameState == GameManager.eGameState.EDIT)
            {
                if (clonedMenuObject != null)
                {
                    clonedMenuObject.SetActive(false);
                    menuActive = false;
                }
            }
        }

        public void ForceCloseMenu()
        {
            if (clonedMenuObject != null)
            {
                clonedMenuObject.SetActive(false);
                menuActive = false;
            }
        }

        private void Update()
        {
            if (clonedMenuObject != null && menuActive)
            {
                clonedMenuObject.transform.rotation = transform.rotation;
                clonedMenuObject.transform.position = transform.position;
            }
        }
    }
}