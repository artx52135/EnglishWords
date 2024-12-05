using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeaderScript : MonoBehaviour
{
    public DataScript data;
    public Canvas menuCanvas;
    MenuScript menu;
    Button lBut, rBut;
    int curInd;
    CanvasGroup canvasGroup;

    void Start()
    {
        lBut = transform.GetChild(0).GetComponent<Button>();
        rBut = transform.GetChild(2).GetComponent<Button>();
        curInd = SceneManager.GetActiveScene().buildIndex;
        if (menuCanvas != null)
            menu = menuCanvas.GetComponent<MenuScript>();
        canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }
    void Update()
    {
        if (!Input.anyKeyDown || canvasGroup != null && !canvasGroup.interactable)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
            OnClickHandler(-1);
        else if (Input.GetKeyDown(KeyCode.F5))
            OnClickHandler(1);
        else if (Input.GetKeyDown(KeyCode.F6))
            OnClickHandler(2);
        else if (Input.GetKeyDown(KeyCode.F7))
            OnClickHandler(3);
        else if (Input.GetKeyDown(KeyCode.F2))
            OnClickHandler(4);
        else if (Input.GetKeyDown(KeyCode.F1))
            OnClickHandler(5);
        else if (Input.GetKeyDown(KeyCode.F3)
        && lBut.IsActive() && lBut.interactable)
            lBut.onClick.Invoke();
        else if (Input.GetKeyDown(KeyCode.F4)
        && rBut.IsActive() && rBut.interactable)
            rBut.onClick.Invoke();
    }

    void OnDestroy() => data.SavePrefs();

    public void OnClickHandler(int index)
    {
        if (index >= 0)
            SceneManager.LoadScene(index);
        else if (index == -1)
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if (index == -2)
        {
            if (menu != null)
                menu.ShowMenu();
        }
    }
}
