using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "Scriptable Objects/Deck")]
public class Deck : ScriptableObject
{
    // Card lists
    [Header("Card List")]
    public List<Card> cardList = new();

    [Header("Card Piles")]
    public List<Card> hand = new();
    public List<Card> drawPile = new();
    public List<Card> discardPile= new();
    public List<Card> exhaustPile = new();

    // Accessors
    public int DeckSize => cardList.Count;
    public int HandSize => hand.Count;


    public void ShuffleDeck(bool shuffleHand = false)
    {
        // Clear hand only if hand is to be shuffled into the deck as well
        if (shuffleHand)
            hand = new();

        // Clear discard pile
        discardPile = new();

        // Clear and copy card list to draw pile
        drawPile = new(cardList);

        // Shuffle draw pile in place
        for (int i = DeckSize - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
        }
    }

    public void DrawCards(int numCards)
    {
        // Can't draw negative numbers
        if (numCards <= 0)
            return;

        // Draw the specified number of cards
        for (int i = 0; i < numCards; i++)
        {
            // Shuffle deck if there is no card to draw
            if (drawPile.Count == 0)
                ShuffleDeck();

            // Move the card on the top of the draw pile to the hand
            hand.Add(drawPile[drawPile.Count - 1]);
            drawPile.RemoveAt(drawPile.Count - 1);
        }
    }
}