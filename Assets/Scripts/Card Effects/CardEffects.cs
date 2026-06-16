using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void Execute(CardContext context);
}

public class CardContext
{
    public GameObject player;
    public GameObject target;
    public Card sourceCard;
}