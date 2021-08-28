using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIActions : MonoBehaviour
{
    [SerializeField] DrivableInputModule recordableModule;

    void Update()
    {
        ManualActions();
    }

    //Example
    void ManualActions()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Click(Vector2.zero, 1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Hover(new Vector2(50, 500), 1);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            var startPos = Vector2.one * 200;
            var endPos = Vector2.one * 500;

            Drag(startPos, endPos, 2f, 2);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            var startPos = Vector2.one * 200;
            var endPos = Vector2.one * 500;

            Drag(endPos, startPos, 2f, 3);
        }
    }

    void Hover(Vector2 pos, int index) => recordableModule.SimulatePointer(index, pos, PointerEventData.FramePressState.NotChanged);
    void Click(Vector2 pos, int index) => recordableModule.SimulatePointer(index, pos, PointerEventData.FramePressState.PressedAndReleased);

    void Drag(Vector2 from, Vector2 to, float duration, int index) 
    {
        var lastPos = from;

        Animate(from, to, 2f,
            apply: (pos) =>
            {
                recordableModule.SimulatePointer(index, pos, pos - lastPos, PointerEventData.FramePressState.NotChanged);
                lastPos = pos;
            },
            start: (pos) =>
            {
                recordableModule.SimulatePointer(index, pos, PointerEventData.FramePressState.Pressed);
            },
            end: (pos) =>
            {
                recordableModule.SimulatePointer(index, pos, PointerEventData.FramePressState.Released);
            }
        );
    }

    void Animate(Vector2 from, Vector2 to, float duration, Action<Vector2> apply, Action<Vector2> start = null, Action<Vector2> end = null)
        => StartCoroutine(AnimateAsync(from, to, duration, apply, start, end));
    IEnumerator AnimateAsync(Vector2 from, Vector2 to, float duration, Action<Vector2> apply, Action<Vector2> start = null, Action<Vector2> end = null)
    {
        var startT = Time.time;
        var endT = startT + duration;

        start?.Invoke(from);

        while(Time.time < endT)
        {
            var t = Mathf.InverseLerp(startT, endT, Time.time);
            var pos = Vector2.Lerp(from, to, t);
            apply(pos);

            yield return null;
        }

        end?.Invoke(to);
    }
}
