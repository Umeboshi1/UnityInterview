using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VideoPoker
{
	//-//////////////////////////////////////////////////////////////////////
	/// 
	/// The main game manager
	/// 
	public class GameManager : MonoBehaviour
	{
		private List<Card> _deck = new List<Card>();

		private CardInfo[] _currCards;

		public int TotalPoints = 0;
		public int LastWonPoints = 0;
		public string LastWonString = "";

		public enum State {
			Bet,
			Redeal,
		}

		public State GameState = State.Bet;

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Awake()
		{
			_currCards = FindObjectsOfType<CardInfo>().OrderBy(a => a.name).Reverse().ToArray();
			Debug.Assert(_currCards.Length == 5, "Wrong amount of cards in Hand!");
		}

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Start()
		{
			NewDeck();
		}
		
		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Update()
		{
			//For Debugging!
			if (Input.GetKeyDown(KeyCode.Alpha1)){
				CreateHand(2, CardSuit.Diamond, 3, CardSuit.Heart, 2, CardSuit.Spade, 4, CardSuit.Spade, 3, CardSuit.Spade);
			}
		}

		public void CreateHand(int value1, CardSuit suit1, int value2, CardSuit suit2, int value3, CardSuit suit3, int value4, CardSuit suit4, int value5, CardSuit suit5) {
			_currCards[0].Card = new Card(suit1, value1);
			_currCards[1].Card = new Card(suit2, value2);
			_currCards[2].Card = new Card(suit3, value3);
			_currCards[3].Card = new Card(suit4, value4);
			_currCards[4].Card = new Card(suit5, value5);
			LastWonPoints = CalculatePoints(1);
		}

		public CardInfo[] Bet(int betAmount) {
			NewHand();
			if(GameState == State.Bet) {
				TotalPoints -= betAmount;
				GameState = State.Redeal;
			} else {
				//Calculate points and reset game
				LastWonPoints = CalculatePoints(betAmount);
				TotalPoints += LastWonPoints;

				GameState = State.Bet;
				NewDeck();
				_currCards.ToList().ForEach(a => a.ResetIsHeld());
			}
			return _currCards;
		}

		private void NewDeck() {
			CardSuit[] suitTypes = { CardSuit.Club, CardSuit.Diamond, CardSuit.Heart, CardSuit.Spade };

			_deck.Clear();
			foreach(CardSuit suit in suitTypes) {
				for(int i = 1; i < 14; i++) {
					_deck.Add(new Card(suit, i));
				}
			}

			Debug.Assert(_deck.Count == 52, "Deck has too many/too few cards!:" + _deck.Count);
		}

		public Card RemoveRandomCard() {
			Debug.Assert(_deck.Count > 0, "Deck is empty!");
			int randomIndex = Random.Range(0, _deck.Count - 1);
			Card chosenCard = _deck[randomIndex];
			_deck.RemoveAt(randomIndex);
			return chosenCard;
		}

		public void NewHand() {
			for(int i = 0; i < 5; i++) {
				if (!_currCards[i].IsHeld) {
					_currCards[i].Card = RemoveRandomCard();
				}
			}
		}

		public CardInfo[] GetSortedHand() {
			return _currCards.OrderBy(a => a.Card.Value).ThenBy(a => a.Card.Suit).ToArray();
		}

		private int CalculatePoints(int betAmount) {
			CardInfo[] sortedHand = GetSortedHand();
			if (RoyalFlush(sortedHand)) {
				return betAmount * 800;
			} else if (StraightFlush(sortedHand)) {
				return betAmount * 50;
			} else if (FourOfAKind(sortedHand)) {
				return betAmount * 25;
			} else if (FullHouse(sortedHand)) {
				return betAmount * 9;
			} else if (Flush(sortedHand)) {
				return betAmount * 6;
			} else if (Straight(sortedHand)) {
				return betAmount * 4;
			} else if (ThreeOfAKind(sortedHand)) {
				return betAmount * 3;
			} else if (TwoPair(sortedHand)) {
				return betAmount * 2;
			} else if (JacksOrBetter(sortedHand)) {
				return betAmount * 1;
			}
			LastWonString = "Try Again!";
			Debug.Log("No Points.");
			return 0;
		}

		private bool RoyalFlush(CardInfo[] sortedHand) { //x800
			CardSuit suit;
			suit = sortedHand[0].Card.Suit;
			for(int i = 1; i < sortedHand.Length; i++) {
				if(sortedHand[i].Card.Suit != suit) {
					return false;
				}
			}
			if(sortedHand[0].Card.Value == 1 &&
				sortedHand[1].Card.Value == 10 &&
				sortedHand[2].Card.Value == 11 &&
				sortedHand[3].Card.Value == 12 &&
				sortedHand[4].Card.Value == 13) {

				LastWonString = "Royal Flush!";
				Debug.Log("Royal Flush!");
				return true;
			}
			
			return false;
		}

		private bool StraightFlush(CardInfo[] sortedHand) { //x50
			CardSuit suit;
			suit = sortedHand[0].Card.Suit;
			for (int i = 0; i < sortedHand.Length - 1; i++) {
				if (sortedHand[i + 1].Card.Value != sortedHand[i].Card.Value + 1) {
					return false;
				}
				else if (sortedHand[i].Card.Suit != suit) {
					return false;
				}
			}
			if (sortedHand[4].Card.Suit != suit) {
				return false;
			}
			LastWonString = "Straight Flush!";
			Debug.Log("Straight Flush");
			return true;
		}

		private bool FourOfAKind(CardInfo[] sortedHand) { //x25
			int topCount = 0;
			int currCount = 0;
			int currValue = 0;
			foreach(CardInfo cardInfo in sortedHand) {
				if(cardInfo.Card.Value != currValue) {
					if(currCount > topCount) {
						topCount = currCount;
					}
					currCount = 1;
					currValue = cardInfo.Card.Value;
				} else {
					currCount++;
				}
			}
			if (currCount > topCount) {
				topCount = currCount;
			}
			if (topCount == 4) {
				LastWonString = "Four Of A Kind!";
				Debug.Log("Four Of A Kind!");
				return true;
			}
			return false;
		}

		private bool FullHouse(CardInfo[] sortedHand) { //x9
			int value1 = 0;
			int value2 = 0;
			foreach(CardInfo cardInfo in sortedHand) {
				if(value1 == 0) {
					value1 = cardInfo.Card.Value; //first value
				} else if(cardInfo.Card.Value != value1){
					if(value2 == 0) {
						value2 = cardInfo.Card.Value; //second value
					} else if(cardInfo.Card.Value != value2) { //third value found
						return false;
					}
				}
			}
			/*int[] values = _currCards.Select(a => a.Card.Value).Distinct() as int[];
			if(values.Length == 2) {
				Debug.Log("Full House!");
				return true;
			}*/
			LastWonString = "Full House!";
			Debug.Log("Full House!");
			return true;
		}

		private bool Flush(CardInfo[] sortedHand) { //x6
			CardSuit suit = sortedHand[0].Card.Suit;
			if(sortedHand.All(a => a.Card.Suit == suit)) {
				LastWonString = "Flush!";
				Debug.Log("Flush!");
				return true;
			}
			return false;
		}

		private bool Straight(CardInfo[] sortedHand) { //x4
			for(int i = 0; i < sortedHand.Length - 1; i++) {
				if(sortedHand[i+1].Card.Value != sortedHand[i].Card.Value + 1) {
					return false;
				}
			}
			LastWonString = "Straight!";
			Debug.Log("Straight!");
			return true;
		}

		private bool ThreeOfAKind(CardInfo[] sortedHand) { //x3
			int topCount = 0;
			int currCount = 0;
			int currValue = 0;
			foreach (CardInfo cardInfo in sortedHand) {
				if (cardInfo.Card.Value != currValue) {
					if (currCount > topCount) {
						topCount = currCount;
					}
					currCount = 1;
					currValue = cardInfo.Card.Value;
				}
				else {
					currCount++;
				}
			}
			if (currCount > topCount) {
				topCount = currCount;
			}
			if (topCount == 3) {
				LastWonString = "Three Of A Kind!";
				Debug.Log("Three Of A Kind!");
				return true;
			}
			return false;
		}

		private bool TwoPair(CardInfo[] sortedHand) { //x2
			int pairCnt = 0;
			int lastPairValue = 0;
			for(int i = 0; i < sortedHand.Length - 1; i++) {
				if(sortedHand[i].Card.Value == sortedHand[i+1].Card.Value && sortedHand[i].Card.Value != lastPairValue) {
					lastPairValue = sortedHand[i].Card.Value;
					pairCnt++;
				}
			}
			if(pairCnt == 2) {
				LastWonString = "Two Pair!";
				Debug.Log("Two Pair!");
				return true;
			}
			return false;
		}

		private bool JacksOrBetter(CardInfo[] sortedHand) { //x1
			for(int i = 0; i < sortedHand.Length - 1; i++) {
				if(sortedHand[i].Card.Value > 1 && sortedHand[i].Card.Value < 11) {
					continue;
				}
				if(sortedHand[i].Card.Value == sortedHand[i + 1].Card.Value) {
					LastWonString = "Jacks Or Better!";
					Debug.Log("Jacks or Better!");
					return true;
				}
			}
			return false;
		}
	}
}