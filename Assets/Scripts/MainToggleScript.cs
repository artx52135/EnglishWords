using UnityEngine;
using UnityEngine.EventSystems;

public class MainToggleScript : MonoBehaviour, ISelectHandler
{
    public DataScript data;

    public void OnSelect(BaseEventData eventData) =>
    data.S2ItemIndex = transform.GetSiblingIndex();
}
