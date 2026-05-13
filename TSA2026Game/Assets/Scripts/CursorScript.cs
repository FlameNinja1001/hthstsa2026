using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RawImage cursorImage;

    [Header("Snap Positions")]
    [SerializeField] private float positionA = 100f;
    [SerializeField] private float positionB = -100f;

    private PlayerInputActions _inputActions;
    private Vector2 _moveInput;
    private bool _inputHeld = false;
    private bool _isOnA = true;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    void Start()
    {
        cursorImage.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled  += OnMoveCanceled;
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled  -= OnMoveCanceled;
        _inputActions.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();

        if (!_inputHeld && !TitleScreenScript.hasChosen && Mathf.Abs(_moveInput.y) > 0.5f)
        {
            bool pressingDown = _moveInput.y < 0;
            bool pressingUp   = _moveInput.y > 0;

            bool blocked = (_isOnA && pressingUp) || (!_isOnA && pressingDown);

            if (!blocked)
                SnapToOther();
        }

        _inputHeld = true;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
        _inputHeld = false;
    }

    private void SnapToOther()
    {
        if (cursorImage == null) return;

        _isOnA = !_isOnA;
        TitleScreenScript.isHard = !_isOnA;

        RectTransform rt = cursorImage.rectTransform;
        Vector2 pos = rt.anchoredPosition;
        pos.y = _isOnA ? positionA : positionB;
        rt.anchoredPosition = pos;
    }
}