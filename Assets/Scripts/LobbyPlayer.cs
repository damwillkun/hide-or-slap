using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class LobbyPlayer : MonoBehaviour
{
    public Selection Selection { get; private set; } = Selection.CharacterA;
    public bool IsLocked { get; private set; } = false;

    private JamGameManager gm;
    private int slot; // 0 = left, 1 = right
    private float nextNavTime;
    private const float NavCooldown = 0.18f;

    public void Init(JamGameManager gameManager, int assignedSlot)
    {
        gm = gameManager;
        slot = assignedSlot;
    }

    // Hook these via PlayerInput "Invoke Unity Events":
    // Lobby/Navigate -> OnNavigate
    // Lobby/Submit   -> OnSubmit

    public void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (IsLocked) return;
        if (Time.unscaledTime < nextNavTime) return;

        Vector2 v = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(v.x) < 0.5f) return;

        nextNavTime = Time.unscaledTime + NavCooldown;

        // Only 2 chars: left = A, right = B
        Selection = (v.x > 0) ? Selection.CharacterB : Selection.CharacterA;

        gm.RefreshSlotUI(slot);
    }

    public void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (IsLocked) return;

        IsLocked = true;
        gm.RefreshSlotUI(slot);
        gm.NotifyLocked();
    }
}
