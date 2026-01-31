using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	enum State
	{
		None,
		MaskOn,
		Slap
	}

	private State stateSelected;
    [HideInInspector]
    public PlayerInput playerInput;

    private bool canSelectAction = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        GameManager.Instance.OnNewSequenceEvent += GameManager_OnNewSequenceEvent;
    }

    private void GameManager_OnNewSequenceEvent(GameManager.Sequence sequence)
    {
        canSelectAction = sequence == GameManager.Sequence.StartWaitingAction;
    }

    public void OnSlapSelected(InputValue value)
    {
        if(value.isPressed && canSelectAction)
        {
            Debug.Log("SLAP");
            stateSelected = State.Slap;
        }
    }

    public void OnProtectSelected(InputValue value)
    {
        if (value.isPressed && canSelectAction)
        {
            Debug.Log("PROTECT");
            stateSelected = State.MaskOn;
        }
    }

}

