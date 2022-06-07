using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Card
{
	public CardSuit Suit;
	public int Value;

	public Card(CardSuit suit, int value) {
		Suit = suit;
		Value = value;
	}
}

public enum CardSuit {
	Heart,
	Spade,
	Diamond,
	Club,
}