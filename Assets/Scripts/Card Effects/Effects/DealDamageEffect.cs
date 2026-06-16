using UnityEngine;

[CreateAssetMenu(fileName = "Damage", menuName = "Scriptable Objects/Card Effects/Deal Damage Effect")]
public class DealDamageEffect : CardEffect
{
    public int damage;

    public override void Execute(CardContext context)
    {
        Debug.Log($"{context.sourceCard.id} did {damage} damage to {context.target}.");
    }
}