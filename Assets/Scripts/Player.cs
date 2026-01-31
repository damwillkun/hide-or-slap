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

    private Animator animator;
    private bool canSelectAction = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
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
            animator.SetTrigger("FakingSlap");
        }
    }

    public void OnProtectSelected(InputValue value)
    {
        if (value.isPressed && canSelectAction)
        {
            Debug.Log("PROTECT");
            stateSelected = State.MaskOn;
            animator.SetTrigger("FakingMaskOn");
        }
    }

    public void PlayAction()
    {
        switch (stateSelected)
        {
            case State.None:
                break;
            case State.MaskOn:
                animator.SetTrigger("PutMaskOn");
                break;
            case State.Slap:
                animator.SetTrigger("Slap");
                break;
            default:
                break;
        }
    }

    public void PlayIdle()
    {
        animator.SetTrigger("Idle");
    }
}

