using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Edgar.Dossier.Core;

namespace Edgar.Dossier.UI
{
    public class ClueEntryUI : MonoBehaviour
    {
        [SerializeField] private Image _thumbnail;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private GameObject _newBadge;
        [SerializeField] private GameObject _selectedHighlight;
        [SerializeField] private Button _button;

        private ClueEntry _entry;                          // The data this UI represents
        private Action<ClueEntry> _onClick;         // What to do when clicked

        void Start()
        {
            // When the button is clicked, call the callback with this entry
            _button.onClick.AddListener(() => _onClick?.Invoke(_entry));
        }

        public void Bind(ClueEntry entry, Action<ClueEntry> onClick)
        {
            _entry = entry;
            _onClick = onClick;
            Refresh();
        }

        public void Refresh()
        {
            if (_entry == null) return;

            _title.text = _entry.Data.title;
            _thumbnail.sprite = _entry.Data.sprite;
            _newBadge.SetActive(_entry.IsNew);
        }

        public void SetSelected(bool selected)
        {
            _selectedHighlight.SetActive(selected);
        }

        public ClueEntry Entry => _entry;
    }
}