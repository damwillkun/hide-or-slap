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
    public int ActionsDuration;
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
        UIManager.Instance.DisplayVersus(true);

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

            //foreach (Player player in players)
            //{
            //    player.PlayIdle();
            //}

            UIManager.Instance.SequenceTitlePlayAction.SetActive(true);
            yield return new WaitForSeconds(PlayActionSequenceDuration);
            UIManager.Instance.SequenceTitlePlayAction.SetActive(false);
            yield return StartCoroutine(PlayActions());
            yield return StartCoroutine(ResolveActions());

            yield return new WaitForSeconds(ActionsDuration);

            yield return null;
        }
    }

    IEnumerator WaitingAction()
    {
        Debug.Log("WaitingAction");
        OnNewSequenceEvent?.Invoke(Sequence.StartWaitingAction);

        UIManager.Instance.PrepareActionCountdownTimer.gameObject.SetActive(true);
        UIManager.Instance.PrepareActionCountdownTimer.StartTimer(TimeToSelectActions);
        yield return new WaitForSeconds(TimeToSelectActions + 1f);

        OnNewSequenceEvent?.Invoke(Sequence.EndWaitingAction);

        yield return null;
    }

    IEnumerator PlayActions()
    {
        OnNewSequenceEvent?.Invoke(Sequence.PlayAction);
        Debug.Log("Fight");

        foreach (Player player in players)
        {
            player.PlayAction();
        }

        yield return null;
    }

    IEnumerator ResolveActions()
    {
        yield return new WaitForSeconds(1f); // TODO

        int scoreP1 = 0;
        int scoreP2 = 0;

        ResolveRound(players[0], players[1], out scoreP1, out scoreP2);

        yield return new WaitForSeconds(1f); // TODO

        yield return null;
    }

    private void ResolveRound(Player p1, Player p2, out int scoreP1, out int scoreP2)
    {
        scoreP1 = 0;
        scoreP2 = 0;

        // Same choice 0 / 0
        if (p1 == p2)
            return;

        // Slap beat Taunt
        if (p1.StateSelected == Player.State.Slap && p2.StateSelected == Player.State.Taunt)
        {
            p2.PlayHit();
            scoreP1 = 1;
            return;
        }
        if (p2.StateSelected == Player.State.Slap && p1.StateSelected == Player.State.Taunt)
        {
            p1.PlayHit();
            scoreP2 = 1;
            return;
        }

        // MaskOn beat Slap
        if (p1.StateSelected == Player.State.MaskOn && p2.StateSelected == Player.State.Slap)
        {
            p1.PlayTaunt();
            scoreP1 = 1;
            scoreP2 = -1;
            return;
        }
        if (p2.StateSelected == Player.State.MaskOn && p1.StateSelected == Player.State.Slap)
        {
            p2.PlayTaunt();
            scoreP2 = 1;
            scoreP1 = -1;
            return;
        }

        // Taunt beat MaskOn
        if (p1.StateSelected == Player.State.Taunt && p2.StateSelected == Player.State.MaskOn)
        {
            scoreP1 = 1;
            return;
        }
        if (p2.StateSelected == Player.State.Taunt && p1.StateSelected == Player.State.MaskOn)
        {
            scoreP2 = 1;
            return;
        }
    }
}

