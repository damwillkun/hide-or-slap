using UnityEngine;

public class JamFightSpawner : MonoBehaviour
{
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform p2Spawn;

    private void Start()
    {
        var data = JamGameManager.PendingFight;

        if (data.p1Prefab == null || data.p2Prefab == null)
        {
            Debug.LogError("No fight setup. Start the game from the title screen.");
            return;
        }

        Instantiate(data.p1Prefab, p1Spawn.position, p1Spawn.rotation);
        Instantiate(data.p2Prefab, p2Spawn.position, p2Spawn.rotation);
    }
}
