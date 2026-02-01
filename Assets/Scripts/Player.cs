using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public enum State
	{
		None,
		MaskOn,
		Slap,
        Taunt
	}

    [Header("Animation Triggers Names")]
    public string Idle;
    public string FakingSlap;
    public string FakingMaskOn;
    public string Slap;
    public string MaskOn;
    public string Hit;
    public string Taunt;

    public State StateSelected { get { return stateSelected; } }

    [HideInInspector]
    public PlayerInput playerInput;
	private State stateSelected;
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
            animator.SetTrigger(FakingSlap);
        }
    }

    public void OnProtectSelected(InputValue value)
    {
        if (value.isPressed && canSelectAction)
        {
            Debug.Log("PROTECT");
            stateSelected = State.MaskOn;
            animator.SetTrigger(FakingMaskOn);
        }
    }

    public void OnTauntSelected(InputValue value)
    {
        if (value.isPressed && canSelectAction)
        {
            Debug.Log("TAUNT");
            stateSelected = State.Taunt;
            animator.SetTrigger(Taunt);
        }
    }

    public void OnNothingSelected(InputValue value)
    {
        if (value.isPressed && canSelectAction)
        {
            Debug.Log("IDLE");
            stateSelected = State.None;
            animator.SetTrigger(Idle);
        }
    }

    public void PlayAction()
    {
        switch (stateSelected)
        {
            case State.None:
                break;
            case State.MaskOn:
                animator.SetTrigger(MaskOn);
                break;
            case State.Slap:
                animator.SetTrigger(Slap);
                break;
            case State.Taunt:
                animator.SetTrigger(Taunt);
                break;
            default:
                break;
        }
    }

    public void PlayIdle()
    {
        animator.SetTrigger(Idle);
    }

    public void PlayHit()
    {
        animator.SetTrigger(Hit);
    }

    public void PlayTaunt()
    {
        animator.SetTrigger(Taunt);
    }
}

