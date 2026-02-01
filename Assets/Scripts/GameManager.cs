using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public List<GameObject> PrefabPlayers;
    [Space]
    public Transform SpawnPointPlayer1;
    public Transform SpawnPointPlayer2;
    [Header("Scoring")]
    public int MaxScore;
    [Header("Round settings")]
    public int MaxRounds; 
    public int RoundCountdownTimer;
    public int TimerReducePerRound;
    public int MinTimerPerRound;
    [Header("Sequences delay")]
    public int DisplayRoundTitleDuration;
    public int PrepareActionSequenceDuration;
    public int PlayActionSequenceDuration;
    public int ActionsDuration;
    public int ScoreUpdateDuration;
    [Header("Audio")]
    public SoundManager AudioManager;

    public Player Player1 { get {return players.Count > 0 ? players[0] : null;} }
    public Player Player2 { get {return players.Count > 1 ? players[1] : null;} }

    public Action<Sequence> OnNewSequenceEvent;

    [HideInInspector]
    public int CurrentRound;

    private int currentRoundCountdownTimer;
    private List<Player> players = new();
    private List<GameObject> playersToLeft = new();
    private int currentScoreP1 = 0;
    private int currentScoreP2 = 0;
    private int scoreWinP1 = 0;
    private int scoreWinP2 = 0;

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
        currentRoundCountdownTimer = RoundCountdownTimer;
        CameraMainMenu.gameObject.SetActive(true);

        playersToLeft = new List<GameObject>(PrefabPlayers);
    }

    public void OnPlayerJoinedEvent(PlayerInput playerInput)
    {
        Debug.Log($"Player Joined [{playerInput.playerIndex}]");

        int randomChar = UnityEngine.Random.Range(0, playersToLeft.Count);
        GameObject charToInstatiate = playersToLeft[randomChar];
        playersToLeft.RemoveAt(randomChar);

        Instantiate(charToInstatiate, playerInput.transform.position, playerInput.transform.rotation, playerInput.transform);

        players.Add(playerInput.GetComponent<Player>());

        playerInput.transform.parent = (playerInput.playerIndex == 0) ? SpawnPointPlayer1 : SpawnPointPlayer2;
        playerInput.transform.localPosition = Vector3.zero;
        playerInput.transform.localRotation = Quaternion.identity;

        playerInput.camera.enabled = false;

        if(players.Count == 2)
        {
            UIManager.Instance.DisplayMainTitle(false);

            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2);
        AudioManager.audioSource.volume = AudioManager.audioSource.volume * 0.5f;

        // Transition to player cameras
        foreach (Player player in players)
        {
            player.playerInput.camera.enabled = true;
        }
        CameraMainMenu.gameObject.SetActive(false);
        UIManager.Instance.DisplayVersus(true);
        while (CurrentRound < MaxRounds)
        {
            CurrentRound++;
            UIManager.Instance.UpdateRound();

            // DISPLAY --- CURRENT ROUND
            UIManager.Instance.DisplayRound(CurrentRound);
            yield return new WaitForSeconds(DisplayRoundTitleDuration);
            UIManager.Instance.HideTitles();

            // DISPLAY --- PREPARE ACTION
            UIManager.Instance.SequenceTitlePrepareAction.SetActive(true);
            yield return new WaitForSeconds(PrepareActionSequenceDuration);
            UIManager.Instance.SequenceTitlePrepareAction.SetActive(false);

            // WAITING FOR PLAYERS ACTIONS
            yield return StartCoroutine(WaitingAction());

            // DISPLAY --- PLAY ACTION
            UIManager.Instance.SequenceTitlePlayAction.SetActive(true);
            yield return new WaitForSeconds(PlayActionSequenceDuration);
            UIManager.Instance.SequenceTitlePlayAction.SetActive(false);

            // RESOLVE ACTIONS
            yield return new WaitForSeconds(.5f);

            yield return StartCoroutine(PlayActions());

            if (CurrentRound % 2 == 0)
            {
                currentRoundCountdownTimer = Mathf.Max(
                    currentRoundCountdownTimer - TimerReducePerRound,
                    MinTimerPerRound
                );
            }

            if(Player1.CurrentScore == MaxScore || Player2.CurrentScore == MaxScore)
            {
                StartCoroutine(EndGame());
                break;
            }
            
            foreach (Player player in players)
            {
                player.PlayIdle();
            }

            yield return null;
        }

        StartCoroutine(EndGame());
        yield return null;
    }

    IEnumerator WaitingAction()
    {
        Debug.Log("WaitingAction");
        OnNewSequenceEvent?.Invoke(Sequence.StartWaitingAction);

        UIManager.Instance.PrepareActionCountdownTimer.gameObject.SetActive(true);
        UIManager.Instance.PrepareActionCountdownTimer.StartTimer(currentRoundCountdownTimer);
        yield return new WaitForSeconds(currentRoundCountdownTimer);

        OnNewSequenceEvent?.Invoke(Sequence.EndWaitingAction);

        yield return new WaitForSeconds(1f);

        yield return null;
    }

    IEnumerator PlayActions()
    {
        OnNewSequenceEvent?.Invoke(Sequence.PlayAction);
        Debug.Log("Fight");

        yield return StartCoroutine(ResolveRound(players[0], players[1]));

        yield return new WaitForSeconds(ActionsDuration);

        if (scoreWinP1 != 0)
        {
            int newScore = players[0].CurrentScore + scoreWinP1;
            players[0].CurrentScore = (newScore <= 0) ? 0 : newScore;
            UIManager.Instance.UpdateScore(1, scoreWinP1);
        }
        if (scoreWinP2 != 0)
        {
            int newScore = players[1].CurrentScore + scoreWinP2;
            players[1].CurrentScore = (newScore <= 0) ? 0 : newScore;
            UIManager.Instance.UpdateScore(2, scoreWinP2);
        }

        UIManager.Instance.Scoring.UpdateScore();

        yield return new WaitForSeconds(ScoreUpdateDuration);

        yield return null;
    }

    IEnumerator ResolveRound(Player p1, Player p2)
    {
        scoreWinP1 = 0;
        scoreWinP2 = 0;

        // Same choice 0 / 0
        if (p1.StateSelected == p2.StateSelected)
        {
            if (p1.StateSelected == Player.State.Slap && p2.StateSelected == Player.State.Slap)
            {
                p1.PlaySlap();
                p2.PlaySlap();
                yield return new WaitForSeconds(.1f);
                p1.PlayHit();
                p2.PlayHit();
                yield break;
            }
            if (p1.StateSelected == Player.State.MaskOn && p2.StateSelected == Player.State.MaskOn)
            {
                p1.PlayMaskOn();
                p2.PlayMaskOn();
                yield break;
            }
        }

        if (p1.StateSelected == Player.State.Slap && p2.StateSelected == Player.State.None)
        {
            p1.PlaySlap();
            p2.PlayHit();
            scoreWinP1 = 1;
            yield break;
        }
        if (p2.StateSelected == Player.State.Slap && p1.StateSelected == Player.State.None)
        {
            p1.PlayHit();
            p2.PlaySlap();
            scoreWinP2 = 1;
            yield break;
        }

        if (p1.StateSelected == Player.State.MaskOn && p2.StateSelected == Player.State.Slap)
        {
            p1.PlayMaskOn();
            p2.PlaySlap();
            scoreWinP1 = 1;
            scoreWinP2 = -1;
            yield break;
        }
        if (p2.StateSelected == Player.State.MaskOn && p1.StateSelected == Player.State.Slap)
        {
            p1.PlaySlap();
            p2.PlayMaskOn();
            scoreWinP2 = 1;
            scoreWinP1 = -1;
            yield break;
        }

        if (p1.StateSelected == Player.State.None && p2.StateSelected == Player.State.MaskOn)
        {
            p1.PlayTaunt();
            p2.PlayMaskOn();
            scoreWinP1 = 1;
            yield break;
        }
        if (p2.StateSelected == Player.State.None && p1.StateSelected == Player.State.MaskOn)
        {
            p1.PlayMaskOn();
            p2.PlayTaunt();
            scoreWinP2 = 1;
            yield break;
        }
    }

    IEnumerator EndGame()
    {
        if(Player1.CurrentScore == MaxScore)
        {
            UIManager.Instance.DisplayWinner(1);
        }
        else if(Player1.CurrentScore == MaxScore)
        {
            UIManager.Instance.DisplayWinner(2);
        }
        else
        {
            if(Player1.CurrentScore > Player2.CurrentScore)
            {
                UIManager.Instance.DisplayWinner(1);
            }
            else if(Player2.CurrentScore > Player1.CurrentScore)
            {
                UIManager.Instance.DisplayWinner(2);
            }
            else
            {
                UIManager.Instance.DisplayWinner(-1);
            }
        }

        yield return null;
    }
}

