using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraNavigationSystem : MonoBehaviour
{
    [SerializeField] private string startPointId;

    private CameraPointMarker _current;
    private Dictionary<string, CinemachineCamera> _camMap;
    private Dictionary<string, CameraPointMarker> _markerMap;

    public event Action<CameraPointMarker> OnCameraChanged;

    void Start()
    {
        BuildMap();
        GoTo(startPointId);
    }

    void BuildMap()
    {
        _camMap = new();
        _markerMap = new();
        var markers = FindObjectsByType<CameraPointMarker>(FindObjectsSortMode.None);
        foreach (var m in markers)
        {
            var cam = m.GetComponent<CinemachineCamera>();
            if (cam == null) continue;
            cam.Priority = 0;
            _camMap[m.data.pointId] = cam;
            _markerMap[m.data.pointId] = m;
        }
    }

    public void GoTo(string pointId)
    {
        if (!_camMap.TryGetValue(pointId, out var target)) return;

        foreach (var cam in _camMap.Values)
            cam.Priority = 0;

        target.Priority = 10;
        _current = _markerMap[pointId];
        OnCameraChanged?.Invoke(_current);
    }

    public List<string> GetNeighbors() =>
        _current?.data.neighbors ?? new List<string>();

    public string GetDisplayName(string pointId) =>
        _markerMap.TryGetValue(pointId, out var m) ? m.data.pointName : pointId;

    public string GetCurrentPointName() => _current?.data.pointName ?? "";
}
