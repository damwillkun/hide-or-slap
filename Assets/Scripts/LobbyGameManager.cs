//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.SceneManagement;

//public class LobbyGameManager : MonoBehaviour
//{
//    [Header("Player Joining")]
//    [SerializeField] private PlayerInputManager playerInputManager;

//    [Header("Fight Scene")]
//    [SerializeField] private string fightSceneName = "FightScene";

//    [Header("Character Prefabs (2 persos jouables)")]
//    [SerializeField] private GameObject characterA;
//    [SerializeField] private GameObject characterB;

//    // Lobby state
//    private readonly List<LobbyPlayer> players = new();

//    // Persist selections into the fight scene
//    public static FightSetupData PendingFightSetup;

//    private void Reset()
//    {
//        playerInputManager = FindFirstObjectByType<PlayerInputManager>();
//    }

//    private void Awake()
//    {
//        if (playerInputManager == null)
//            playerInputManager = FindFirstObjectByType<PlayerInputManager>();

//        // Hook join callback
//        playerInputManager.onPlayerJoined += HandlePlayerJoined;
//        playerInputManager.onPlayerLeft += HandlePlayerLeft;
//    }

//    private void OnDestroy()
//    {
//        if (playerInputManager == null) return;
//        playerInputManager.onPlayerJoined -= HandlePlayerJoined;
//        playerInputManager.onPlayerLeft -= HandlePlayerLeft;
//    }

//    private void HandlePlayerJoined(PlayerInput pi)
//    {
//        var lobbyPlayer = pi.GetComponent<LobbyPlayer>();
//        if (lobbyPlayer == null)
//        {
//            Debug.LogError("Player prefab must have LobbyPlayer component.");
//            return;
//        }

//        lobbyPlayer.Initialize(this);

//        players.Add(lobbyPlayer);
//        ReindexPlayers();

//        Debug.Log($"Player joined: index={pi.playerIndex}, scheme={pi.currentControlScheme}");
//    }

//    private void HandlePlayerLeft(PlayerInput pi)
//    {
//        var lobbyPlayer = pi.GetComponent<LobbyPlayer>();
//        if (lobbyPlayer != null)
//            players.Remove(lobbyPlayer);

//        ReindexPlayers();
//        Debug.Log("Player left");
//    }

//    private void ReindexPlayers()
//    {
//        for (int i = 0; i < players.Count; i++)
//            players[i].SetLobbyIndex(i);
//    }

//    public void NotifyPlayerStateChanged()
//    {
//        if (players.Count < 2) return;

//        // Check if both ready
//        if (players[0].IsReady && players[1].IsReady)
//        {
//            StartFight();
//        }
//    }

//    private void StartFight()
//    {
//        // Prepare data for fight scene
//        PendingFightSetup = new FightSetupData
//        {
//            playerCount = players.Count,
//            player1CharacterPrefab = GetCharacterPrefab(players[0].SelectedCharacter),
//            player2CharacterPrefab = GetCharacterPrefab(players[1].SelectedCharacter),
//            // You can also store device/scheme if needed
//        };

//        Debug.Log($"Starting fight: P1={players[0].SelectedCharacter}, P2={players[1].SelectedCharacter}");
//        SceneManager.LoadScene(fightSceneName);
//    }

//    private GameObject GetCharacterPrefab(LobbyCharacterSelection selection)
//    {
//        return selection == LobbyCharacterSelection.CharacterA ? characterA : characterB;
//    }
//}

//public enum LobbyCharacterSelection
//{
//    CharacterA = 0,
//    CharacterB = 1
//}

//public struct FightSetupData
//{
//    public int playerCount;
//    public GameObject player1CharacterPrefab;
//    public GameObject player2CharacterPrefab;
//}
