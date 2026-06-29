using TMPro;
using UnityEngine;

namespace Edgar.UI
{
    public class RoomNameUI : MonoBehaviour
    {
        [SerializeField] private CameraNavigationSystem _navigationSystem;
        [SerializeField] private TMP_Text _roomNameText;

        private void Start()
        {
            if (_navigationSystem != null)
            {
                _navigationSystem.OnCameraChanged += _ => UpdateRoomName();
            }

            UpdateRoomName();
        }

        private void UpdateRoomName()
        {
            if (_roomNameText != null && _navigationSystem != null)
            {
                _roomNameText.text = _navigationSystem.GetCurrentPointName();
            }
        }
    }
}