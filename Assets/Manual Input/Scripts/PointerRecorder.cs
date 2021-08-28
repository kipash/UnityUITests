using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerRecorder : MonoBehaviour
{
    [SerializeField] DrivableInputModule inputModule;

    bool isRecording;
    List<PointerSnapshot> currentRecording = new List<PointerSnapshot>();

    void Awake()
    {
        inputModule.OnPointerChanged += OnPointerUpdate;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            currentRecording.Clear();
            isRecording = true;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isRecording = false;
            SimulateRecording(currentRecording);
        }
    }

    void OnPointerUpdate(int index, Vector2 position, Vector2 delta, PointerEventData.FramePressState state)
    {
        if (!isRecording)
            return;

        var snapshot = new PointerSnapshot()
        {
            Index = index,
            Position = position,
            Delta = delta,
            State = state,
            Time = Time.time,
        };

        currentRecording.Add(snapshot);
    }

    void SimulateRecording(List<PointerSnapshot> recording) => StartCoroutine(SimulateRecordingAsync(recording));
    IEnumerator SimulateRecordingAsync(List<PointerSnapshot> recording)
    {
        var size = recording.Count;
        float applyTime = -1;
        for (int i = 0; i < size; i++)
        {
            var snapshot = recording[i];

            if (applyTime == -1)
                applyTime = snapshot.Time;

            if (!Mathf.Approximately(applyTime, snapshot.Time))
            {
                yield return new WaitForSeconds(snapshot.Time - applyTime);
                applyTime = snapshot.Time;
            }

            inputModule.SimulatePointer(snapshot.Index + 10, snapshot.Position, snapshot.Delta, snapshot.State);
        }
    }

    class PointerSnapshot
    {
        public int Index;
        public Vector2 Position;
        public Vector2 Delta;
        public PointerEventData.FramePressState State;
        public float Time;
    }
}