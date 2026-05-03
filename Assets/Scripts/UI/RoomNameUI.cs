using UnityEngine;
using TMPro;

public class RoomNameUI : MonoBehaviour
{
    [SerializeField] private CameraNavigationSystem navSystem;
    [SerializeField] private TMP_Text label;

    void Start()
    {
        navSystem.OnCameraChanged += OnCameraChanged;
    }

    void OnDestroy()
    {
        navSystem.OnCameraChanged -= OnCameraChanged;
    }

    private void OnCameraChanged(CameraPointMarker marker)
    {
        label.text = marker.data.pointName;
    }
}
