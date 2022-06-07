using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VideoPoker {
    public class CardInfo : MonoBehaviour {
        public Card Card;
        public bool IsHeld = false;
        public GameObject HeldText;

        private GameManager _gameManager;
        // Start is called before the first frame update
        void Awake() {
            _gameManager = FindObjectOfType<GameManager>();
            Debug.Assert(_gameManager != null, "Game Manager not found!");
        }

        // Update is called once per frame
        void Update() {

        }

        public void ToggleIsHeld() {
            if (_gameManager.GameState == GameManager.State.Redeal) {
                IsHeld = !IsHeld;
                HeldText.SetActive(IsHeld);
            }
        }

        public void ResetIsHeld() {
            IsHeld = false;
            HeldText.SetActive(false);
        }
    }
}
