using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainButtonScript : MonoBehaviour, ISelectHandler
{
    public DataScript data;
    public void OnClickHandler()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                SceneManager.LoadScene(6);
                return;
            case 6:
                data.PlayAudio(transform.GetSiblingIndex());
                return;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            data.S1ItemIndex = transform.GetSiblingIndex();
    }

}