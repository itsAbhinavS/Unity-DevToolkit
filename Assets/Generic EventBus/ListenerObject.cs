using UnityEngine;

public class ListenerObject : MonoBehaviour
{
    private EventBinding<OnPlayerDamage> damageBinding;
    private EventBinding<OnPlayerDeath> deathBinding;
    private EventBinding<OnPlayerDeath> deathBindingNoArgs;

    private void OnEnable()
    {
        damageBinding = new EventBinding<OnPlayerDamage>(PlayerDamage);
        EventBus<OnPlayerDamage>.Subscribe(new EventBinding<OnPlayerDamage>(PlayerDamage));

        deathBinding = new EventBinding<OnPlayerDeath>(PlayerDeath);
        EventBus<OnPlayerDeath>.Subscribe(new EventBinding<OnPlayerDeath>(PlayerDeath));

        deathBindingNoArgs = new EventBinding<OnPlayerDeath>(() => PlayerDeathNoArgs());
        EventBus<OnPlayerDeath>.Subscribe(new EventBinding<OnPlayerDeath>(() => PlayerDeathNoArgs()));
    }

    private void OnDisable()
    {
        EventBus<OnPlayerDamage>.Unsubscribe(damageBinding);
        EventBus<OnPlayerDeath>.Unsubscribe(deathBinding);
        EventBus<OnPlayerDeath>.Unsubscribe(deathBindingNoArgs);
    }

    private void PlayerDamage(OnPlayerDamage damageData)
    {
        Debug.Log($"The player take {damageData.damageAmount} damage.");
    }
    private void PlayerDeath(OnPlayerDeath deathData)
    {
        Debug.Log($"The player {deathData.PlayerName} died, The cause of death is {deathData.CauseOfDeath}.");
    }
    private void PlayerDeathNoArgs() 
    {
        Debug.Log("The player died, The cause of death is unknown!");
    }
}
