using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edgar.UI
{
    public class NavigationUI : MonoBehaviour
    {
        [SerializeField] private CameraNavigationSystem _navSystem;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private Button _buttonPrefab;

        private void Start()
        {
            if (_navSystem != null)
                _navSystem.OnCameraChanged += _ => RebuildButtons();
        }

        private void RebuildButtons()
        {
            if (_buttonContainer == null || _navSystem == null) return;

            foreach (Transform child in _buttonContainer)
                Destroy(child.gameObject);

            foreach (var neighborId in _navSystem.GetNeighbors())
            {
                string id = neighborId;
                var btn = Instantiate(_buttonPrefab, _buttonContainer);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = _navSystem.GetDisplayName(id);
                btn.onClick.AddListener(() => _navSystem.GoTo(id));
            }
        }
    }
}