using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DrivableInputModule : SimpleInputModule
{
    public bool UserInput;
    static Dictionary<int, PointerEventData> manualEvents = new Dictionary<int, PointerEventData>();

    public delegate void OnPointerUpdate(int index, Vector2 position, Vector2 delta, PointerEventData.FramePressState state);
    public OnPointerUpdate OnPointerChanged;

    public override void Process()
    {
        if(UserInput)
            base.Process();
    }

    public void SimulatePointer(int index, Vector2 pos, PointerEventData.FramePressState state) => SimulatePointer(index, pos, Vector2.zero, state);
    public void SimulatePointer(int index, Vector2 pos, Vector2 delta, PointerEventData.FramePressState state)
    {
        var pointer = GetPointer(index);
        pointer.pointerId = index;
        pointer.delta = delta;
        pointer.position = pos;
        pointer.scrollDelta = Input.mouseScrollDelta;
        pointer.button = PointerEventData.InputButton.Left;

        eventSystem.RaycastAll(pointer, m_RaycastResultCache);

        var raycast = FindFirstRaycast(m_RaycastResultCache);
        pointer.pointerCurrentRaycast = raycast;

        m_RaycastResultCache.Clear();

        // --------------

        ProcessMousePress(new MouseButtonEventData()
        {
            buttonData = pointer,
            buttonState = state,
        });

        ProcessMove(pointer);
        ProcessDrag(pointer);
    }

    PointerEventData GetPointer(int index)
    {
        if (manualEvents.TryGetValue(index, out var pointer))
            return pointer;
        else
        {
            var newPointer = new PointerEventData(eventSystem);
            manualEvents.Add(index, newPointer);
            return newPointer;
        }
    }

    protected override void ProcessMousePress(MouseButtonEventData data)
    {
        base.ProcessMousePress(data);

        var pointerData = data.buttonData;
        var state = data.buttonState;

        OnPointerChanged?.Invoke(pointerData.pointerId, pointerData.position, pointerData.delta, state);
    }
}
