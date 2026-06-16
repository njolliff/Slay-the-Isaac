using UnityEngine;

public enum CardType{ Attack, Skill, Power, Curse, Status }
public enum Rarity{ Common, Uncommon, Rare }

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string description;
    public Sprite artwork;
    public CardType type;
    public Rarity rarity;

    [Header("Stats")]
    public int energyCost;
    public float castTime;

    [Header("Effects")]
    public CardEffect[] cardEffects;
}