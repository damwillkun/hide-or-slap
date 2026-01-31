using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Sequence
    {
        StartWaitingAction,
        EndWaitingAction,
        PlayAction
    }

    public GameObject CameraMainMenu;
    public PlayerInputManager PlayerInputManager;
    [Space]
    public Transform SpawnPointPlayer1;
    public Transform SpawnPointPlayer2;
    [Header("Sequences delay")]
    public int DisplayRoundTitleDuration;
    public int PrepareActionSequenceDuration;
    public int PlayActionSequenceDuration;
    [Space]
    public int TimeToSelectActions;

    public Action<Sequence> OnNewSequenceEvent;

    private int currentRound;
    private List<Player> players = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        CameraMainMenu.gameObject.SetActive(true);
    }

    public void OnPlayerJoinedEvent(PlayerInput playerInput)
    {
        Debug.Log($"Player Joined [{playerInput.playerIndex}]");
        players.Add(playerInput.GetComponent<Player>());

        playerInput.transform.parent = (playerInput.playerIndex == 0) ? SpawnPointPlayer1 : SpawnPointPlayer2;
        playerInput.transform.localPosition = Vector3.zero;
        playerInput.transform.localRotation = Quaternion.identity;

        playerInput.camera.enabled = false;

        if(players.Count == 2)
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2);

        // Transition to player cameras
        foreach (Player player in players)
        {
            player.playerInput.camera.enabled = true;
        }
        CameraMainMenu.gameObject.SetActive(false);

        // Boucle GamePlay > tant que les deux joueurs sont alive
        // Waiting for input
        while (true)
        {
            currentRound++;

            // TODO: Display "ROUND X !!!!" + voice
            UIManager.Instance.DisplayRound(currentRound);
            yield return new WaitForSeconds(DisplayRoundTitleDuration);
            UIManager.Instance.HideTitles();

            UIManager.Instance.SequenceTitlePrepareAction.SetActive(true);
            yield return new WaitForSeconds(PrepareActionSequenceDuration);
            UIManager.Instance.SequenceTitlePrepareAction.SetActive(false);
            yield return StartCoroutine(WaitingAction());

            UIManager.Instance.SequenceTitlePlayAction.SetActive(true);
            yield return new WaitForSeconds(PlayActionSequenceDuration);
            UIManager.Instance.SequenceTitlePlayAction.SetActive(false);
            yield return StartCoroutine(PlayActions());

            yield return null;
        }
    }

    IEnumerator WaitingAction()
    {
        Debug.Log("WaitingAction");
        OnNewSequenceEvent?.Invoke(Sequence.StartWaitingAction);

        UIManager.Instance.PrepareActionCountdownTimer.StartTimer(TimeToSelectActions);
        yield return new WaitForSeconds(TimeToSelectActions + 1f);

        OnNewSequenceEvent?.Invoke(Sequence.EndWaitingAction);

        yield return null;
    }

    IEnumerator PlayActions()
    {
        OnNewSequenceEvent?.Invoke(Sequence.PlayAction);
        Debug.Log("Fight");
        yield return new WaitForSeconds(4f); // TEMP

        yield return null;
    }
}

