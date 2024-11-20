using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class LearnScript : MonoBehaviour
{
    public DataScript data;
    public Button mainButton;
    void Start()
    {
        var dropdown = GameObject.Find("MainDropdown")
        .GetComponent<Dropdown>();
        dropdown.value = data.Level;
        dropdown.onValueChanged.AddListener(OnValueChangeHandler);
        for (int i = 0; i < data.TopicCount; i++)
        {
            var b = Instantiate(mainButton);
            b.GetComponentInChildren<Text>().text = data.Topic(i);
            b.transform.SetParent(transform);
            b.transform.localScale = Vector2.one;
        }
        var es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        es.SetSelectedGameObject(transform.GetChild(data.S1ItemIndex).gameObject);
        var scrollbar = GameObject.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
        scrollbar.value = data.S1ScrollbarValue;
        scrollbar.onValueChanged.AddListener(v => data.S1ScrollbarValue = v);
    }
    void OnValueChangeHandler(int i)
    {
        if (i != data.Level)
        {
            data.SetLevel(i);
            SceneManager.LoadScene(1);
        }
    }

    void Update()
    {
        if (Input.inputString.Length > 0)
        {
            int i = Input.inputString[0] - '1';
            if (i >= 0 && i <= 3)
                OnValueChangeHandler(i);
        }
    }

}