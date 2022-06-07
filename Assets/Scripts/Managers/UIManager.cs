using UnityEngine;
using UnityEngine.UI;

namespace VideoPoker
{
	//-//////////////////////////////////////////////////////////////////////
	///
	/// Manages UI including button events and updates to text fields
	/// 
	public class UIManager : MonoBehaviour
	{
		[SerializeField]
		private Text currentBalanceText = null;

		[SerializeField]
		private Text winningText = null;

		[SerializeField]
		private Button betButton = null;

		[SerializeField]
		private Text buttonText = null;

		private GameManager _gameManager;

		public Image[] CardImages;

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Awake()
		{
			_gameManager = FindObjectOfType<GameManager>();
			currentBalanceText = GameObject.Find("CurrentBalanceText").GetComponent<Text>();
			winningText = GameObject.Find("WinningText").GetComponent<Text>();
			buttonText = GameObject.Find("BetButton").GetComponentInChildren<Text>();
		}

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Start()
		{
			betButton.onClick.AddListener(OnBetButtonPressed);
		}

		//-//////////////////////////////////////////////////////////////////////
		///
		/// Event that triggers when bet button is pressed
		/// 
		private void OnBetButtonPressed()
		{
			if(_gameManager != null) {
				CardInfo[] cards = _gameManager.Bet(1);
				Debug.Assert(cards.Length == 5, "Wrong Number of Cards in Hand!");
				SetCardImages(cards);
				currentBalanceText.text = "Balance: " + _gameManager.TotalPoints + " Credits";
				winningText.text = _gameManager.LastWonString + " You won " + _gameManager.LastWonPoints + " credits.";
				buttonText.text = _gameManager.GameState == GameManager.State.Bet ? "BET" : "REDEAL";
			}
		}

		private Sprite GetCardSprite(Card card) {
			char suitChar;
			switch (card.Suit) {
				case CardSuit.Club:
					suitChar = 'c';
					break;
				case CardSuit.Diamond:
					suitChar = 'd';
					break;
				case CardSuit.Heart:
					suitChar = 'h';
					break;
				case CardSuit.Spade:
					suitChar = 's';
					break;
				default:
					Debug.Log("Shouldn't get here!");
					return null;
			}
			string fileName = "img_card_" + suitChar + card.Value.ToString("00");
			Texture2D tex = Resources.Load("Art/Cards/" + fileName) as Texture2D;
			if (tex != null) {
				return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
			}
			Debug.Log("Texture not found! : " + fileName);
			return null;
		}

		private void UpdateUI() {
			//if(_gameManager.GameState == GameManager.State.)
		}

		private void SetCardImages(CardInfo[] cards) {
			for(int i = 0; i < 5; i++) {
				CardImages[i].sprite = GetCardSprite(cards[i].Card);
			}
		}
	}
}