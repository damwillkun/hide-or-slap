using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public enum State
	{
		None,
		MaskOn,
		Slap
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
    [HideInInspector]
    public int CurrentScore;

    private CameraShake cameraShake;
	private State stateSelected;
    private Animator animator;
    private bool canSelectAction = false;
    private bool isTaunt = true;

    private void Start()
    {
        GameManager.Instance.OnNewSequenceEvent += GameManager_OnNewSequenceEvent;

        cameraShake = GetComponentInChildren<CameraShake>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
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
            stateSelected = State.None;

            isTaunt = !isTaunt;
            animator.SetTrigger(isTaunt ? Idle : Taunt);
        }
    }

    public void PlayAction()
    {
        switch (stateSelected)
        {
            case State.None:
                animator.SetTrigger(isTaunt ? Taunt : Idle);
                break;
            case State.MaskOn:
                animator.SetTrigger(MaskOn);
                break;
            case State.Slap:
                animator.SetTrigger(Slap);
                break;
            default:
                break;
        }
    }

    public void PlaySlap()
    {
        GameManager.Instance.AudioManager.PlaySlapSound();
        animator.SetTrigger(Slap);
    }

    public void PlayMaskOn()
    {
        animator.SetTrigger(MaskOn);
    }

    public void PlayIdle()
    {
        animator.SetTrigger(Idle);
    }

    public void PlayHit()
    {
        cameraShake.Shake();
        animator.SetTrigger(Hit);
    }

    public void PlayTaunt()
    {
        animator.SetTrigger(Taunt);
    }
}

