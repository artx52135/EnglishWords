using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Control2Script : MonoBehaviour
{
    public DataScript data;
    EventSystem es;
    Text headerText;
    Image progressBar;
    string title;
    float progress;
    string[] labels = new string[6];
    Button[] buttons = new Button[6];
    Text[] texts = new Text[6];

    void Start()
    {
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        headerText = GameObject.Find("HText").GetComponent<Text>();
        progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
        for (int i = 0; i < 6; i++)
        {
            buttons[i] = transform.GetChild(i).GetComponent<Button>();
            texts[i] = transform.GetChild(i).GetComponentInChildren<Text>();
        }
        data.InitTest();
        data.NextQuestion(labels, out title, out progress);
        UpdateTestInfo();
        UpdateQuestionInfo();
        for (int i = 0; i < 6; i++)
            buttons[i].onClick.AddListener(OnClickHandler);
    }

    void UpdateTestInfo()
    {
        headerText.text = title;
        progressBar.fillAmount = progress;
    }

    void OnClickHandler()
    {
        int ind = es.currentSelectedGameObject.transform.GetSiblingIndex();
        if (ind == 0)
        {
            data.AdditionalTestAction();
            return;
        }
        string res = data.CheckAnswer(ind, ref title, ref progress);
        if (res == "")
        {
            bool b = data.NextQuestion(labels, out title, out progress);
            UpdateTestInfo();
            if (!b)
            {
                es.SetSelectedGameObject(GameObject.Find("HRButton"));
                for (int i = 0; i < 6; i++)
                    buttons[i].interactable = false;
            }
            else
                UpdateQuestionInfo();
        }
        else
        {
            texts[ind].text = res;
            buttons[ind].interactable = false;
            UpdateTestInfo();
            es.SetSelectedGameObject(buttons[0].gameObject);
        }
    }

    void OnDestroy() => data.SaveResult();

    void UpdateQuestionInfo()
    {
        for (int i = 0; i < 6; i++)
        {
            texts[i].text = labels[i];
            buttons[i].interactable = true;
        }
        es.SetSelectedGameObject(buttons[0].gameObject);
    }

    void Update()
    {
        if (Input.inputString.Length > 0)
        {
            int i = Input.inputString[0] - '0';
            if (i >= 0 && i <= 5 && buttons[i].interactable)
            {
                es.SetSelectedGameObject(buttons[i].gameObject);
                OnClickHandler();
            }
        }
    }
}
