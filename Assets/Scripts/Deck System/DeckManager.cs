using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public Deck deck;

    public void OnCombatStarted()
    {
        // Shuffle deck
        deck.ShuffleDeck();

        // Draw hand (4 cards)
        deck.DrawCards(4);
    }
}