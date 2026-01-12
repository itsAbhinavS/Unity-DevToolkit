using UnityEngine;


public class PublisherObject : MonoBehaviour
{
    private void Start()
    {
        EventBus<OnPlayerDamage>.Publish(new OnPlayerDamage(150));

        Invoke(nameof(Die), 2);
    }

    private void Die()
    {
        EventBus<OnPlayerDeath>.Publish(new OnPlayerDeath
        {
            PlayerName = "Hero",
            CauseOfDeath = "Goblin",
        });
    }

    private void OnDisable()
    {
        EventBus<OnPlayerDamage>.Clear();
        EventBus<OnPlayerDeath>.Clear();
    }
}


/// <summary>
/// Player death event
/// </summary>
public struct OnPlayerDeath : IEvent
{
    public string PlayerName;
    public string CauseOfDeath;
}


/// <summary>
/// Player damage event
/// </summary>
public class OnPlayerDamage : IEvent
{
    public float damageAmount;
    public OnPlayerDamage(float damageAmount) 
    {
        this.damageAmount = damageAmount;
    }
}