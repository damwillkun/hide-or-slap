using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class JamGameManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject selectPanel;

    [Header("UI Left/Right")]
    [SerializeField] private Image leftPortrait;
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private Image rightPortrait;
    [SerializeField] private TMP_Text rightText;

    [Header("Character Visuals (2 persos)")]
    [SerializeField] private Sprite characterASprite;
    [SerializeField] private Sprite characterBSprite;

    [Header("Fight")]
    [SerializeField] private string fightSceneName = "FightScene";
    [SerializeField] private GameObject characterAPrefab;
    [SerializeField] private GameObject characterBPrefab;

    [Header("Player Joining")]
    [SerializeField] private PlayerInputManager playerInputManager;

    // Saved for fight scene
    public static FightSetup PendingFight;

    private bool inLobby = false;
    private readonly List<LobbyPlayer> players = new();

    private void Awake()
    {
        if (playerInputManager == null)
            playerInputManager = FindFirstObjectByType<PlayerInputManager>();

        // Start in title
        titlePanel.SetActive(true);
        selectPanel.SetActive(false);

        // Disable joining until game started (so random button press on title doesn’t create players yet)
        playerInputManager.enabled = false;

        // Clear right UI
        rightPortrait.enabled = false;
        rightText.text = "Press A/Start on gamepad to join";
    }

    private void OnEnable()
    {
        if (playerInputManager != null)
            playerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        if (playerInputManager != null)
            playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void Update()
    {
        if (inLobby) return;

        // Title press Enter or Gamepad Start/South
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            StartLobby();

        if (Gamepad.current != null &&
            (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame))
            StartLobby();
    }

    private void StartLobby()
    {
        inLobby = true;
        titlePanel.SetActive(false);
        selectPanel.SetActive(true);

        // Enable join and auto-create P1 from keyboard (simple jam flow)
        playerInputManager.enabled = true;

        // If no player yet, create P1 using keyboard
        if (players.Count == 0 && Keyboard.current != null)
        {
            // Join with keyboard using default control scheme
            playerInputManager.JoinPlayer(playerIndex: 0, controlScheme: null, pairWithDevice: Keyboard.current);
        }
    }

    private void OnPlayerJoined(PlayerInput input)
    {
        var lp = input.GetComponent<LobbyPlayer>();
        if (lp == null)
        {
            Debug.LogError("LobbyPlayer prefab must have JamLobbyPlayer.");
            return;
        }

        // Assign slot: first join = left, second = right
        int slot = players.Count; // 0 or 1
        if (slot > 1)
        {
            Destroy(input.gameObject);
            return;
        }

        players.Add(lp);

        lp.Init(this, slot);

        // Ensure the PlayerInput uses Lobby map
        input.SwitchCurrentActionMap("Lobby");

        // Show UI for that slot
        RefreshSlotUI(slot);

        // If second joined, show right portrait
        if (slot == 1)
        {
            rightPortrait.enabled = true;
        }
    }

    public void RefreshSlotUI(int slot)
    {
        if (slot < 0 || slot >= players.Count) return;

        var p = players[slot];
        var sprite = (p.Selection == Selection.CharacterA) ? characterASprite : characterBSprite;
        var label = (p.Selection == Selection.CharacterA) ? "Character A" : "Character B";
        if (p.IsLocked) label += "  ?";

        if (slot == 0)
        {
            leftPortrait.sprite = sprite;
            leftPortrait.enabled = true;
            leftText.text = $"P1 (Keyboard)\n{label}\n? ? to switch\nEnter to select";
        }
        else
        {
            rightPortrait.sprite = sprite;
            rightPortrait.enabled = true;
            rightText.text = $"P2\n{label}\nDpad/Stick to switch\nA to select";
        }
    }

    public void NotifyLocked()
    {
        if (players.Count < 2) return;

        if (players[0].IsLocked && players[1].IsLocked)
            StartFight();
    }

    private void StartFight()
    {
        PendingFight = new FightSetup
        {
            p1Prefab = PrefabFromSelection(players[0].Selection),
            p2Prefab = PrefabFromSelection(players[1].Selection),
        };

        SceneManager.LoadScene(fightSceneName);
    }

    private GameObject PrefabFromSelection(Selection s)
        => (s == Selection.CharacterA) ? characterAPrefab : characterBPrefab;
}

public enum Selection { CharacterA, CharacterB }

public struct FightSetup
{
    public GameObject p1Prefab;
    public GameObject p2Prefab;
}
