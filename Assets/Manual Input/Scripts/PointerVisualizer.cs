using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerVisualizer : MonoBehaviour
{
    [SerializeField] DrivableInputModule inputModule;
    [SerializeField] GameObject markerPrefab;
    [SerializeField] RectTransform markersParent;

    Dictionary<int, GameObject> markers = new Dictionary<int, GameObject>();

    void Awake()
    {
        inputModule.OnPointerChanged += SetPointer;
    }

    void SetPointer(int index, Vector2 position, Vector2 delta, PointerEventData.FramePressState state)
    {
        var marker = GetPointer(index);

        marker.anchoredPosition = position;

        if (state == PointerEventData.FramePressState.Released || state == PointerEventData.FramePressState.PressedAndReleased)
            marker.gameObject.SetActive(false);
        else if (state == PointerEventData.FramePressState.Pressed)
            marker.gameObject.SetActive(true);
    }

    RectTransform GetPointer(int index)
    {
        if(markers.TryGetValue(index, out GameObject marker))
        {
            return marker.transform as RectTransform;
        }
        else
        {
            var newMarker = Instantiate(markerPrefab, markersParent);
            newMarker.name += $": {index}";
            markers.Add(index, newMarker);

            return newMarker.transform as RectTransform;
        }
    }
}
