using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogScript : MonoBehaviour
{
    public Canvas mainCanvas;
    CanvasGroup mainCanvasGroup;
    GameObject mainSelectedObject;
    EventSystem es;
    Transform dialogPanel;
    Button[] buttons = new Button[3];
    int defaultInd;
    int cancelInd;

    void Start()
    {
        dialogPanel = GameObject.Find("DialogPanel").transform;
        for (int i = 0; i < buttons.Length; i++)
            buttons[i] = dialogPanel.GetChild(2).GetChild(i).GetComponent<Button>();
        gameObject.SetActive(false);
    }

    public void ShowDialog(string header, string text,
        string[] buttonText = null, System.Action<int> dialogHandler = null,
        int defaultIndex = 0, int cancelIndex = 0)
    {
        //(1)
        dialogPanel.GetChild(0).GetComponentInChildren<Text>().text = header;
        dialogPanel.GetChild(1).GetComponent<Text>().text = text;
        int btnCount = 0;
        if (buttonText != null)
            btnCount = buttonText.Length;
        if (btnCount == 0)
        {
            btnCount = 1;
            SetButton("OK", dialogHandler, 0);
        }
        else
        {
            if (btnCount > buttons.Length)
                btnCount = buttons.Length;
            for (int i = 0; i < btnCount; i++)
                SetButton(buttonText[i], dialogHandler, i);
        }
        for (int i = btnCount; i < buttons.Length; i++)
            buttons[i].gameObject.SetActive(false);
        defaultInd = defaultIndex < btnCount ? defaultIndex : 0;
        cancelInd = cancelIndex < btnCount ? cancelIndex : 0;
        //(2)
        mainCanvasGroup = mainCanvas.GetComponent<CanvasGroup>();
        if (mainCanvasGroup == null)
        {
            Debug.LogError("The main canvas doesn't contain the CanvasGroup component.");
            return;
        }
        mainCanvasGroup.interactable = false;
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        mainSelectedObject = es.currentSelectedGameObject;
        gameObject.SetActive(true);
        es.SetSelectedGameObject(buttons[defaultInd].gameObject);
    }

    void SetButton(string buttonText, System.Action<int> dialogHandler, int index)
    {
        var b = buttons[index];
        b.gameObject.SetActive(true);
        b.GetComponentInChildren<Text>().text = buttonText;
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(CloseDialog);
        if (dialogHandler != null)
            b.onClick.AddListener(() => dialogHandler(index));
    }

    void CloseDialog()
    {
        gameObject.SetActive(false);
        mainCanvasGroup.interactable = true;
        es.SetSelectedGameObject(mainSelectedObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            buttons[cancelInd].onClick.Invoke();
    }
}
