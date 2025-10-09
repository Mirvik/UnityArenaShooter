using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActionMapType
{
    OnFoot,
    UI
}

public enum ActionPhase
{
    Started,
    Performed,
    Canceled
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameInput _input;
    public GameInput Input => _input;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("InputManager");
        go.AddComponent<InputManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _input = new GameInput();
        _input.OnFoot.Enable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            _input.Disable();
            _input = null;
            Instance = null;
        }
    }

    public void SwitchActionMap(ActionMapType mapType)
    {

        _input.Disable();

        switch (mapType)
        {
            case ActionMapType.OnFoot:
                _input.OnFoot.Enable();
                break;

            case ActionMapType.UI:
                _input.UI.Enable();
                break;
        }
    }

    public void Subscribe(InputAction action, ActionPhase actionPhase, Action<InputAction.CallbackContext> callback)
    {
        if (action != null)
        {
            switch(actionPhase)
            {
                case ActionPhase.Started:
                    action.started += callback;
                    break;
                case ActionPhase.Performed:
                    action.performed += callback;
                    break;
                case ActionPhase.Canceled:
                    action.canceled += callback;
                    break;
            }
        }
    }

    public void Unsubscribe(InputAction action, ActionPhase actionPhase, Action<InputAction.CallbackContext> callback)
    {
        if (action != null)
        {
            switch (actionPhase)
            {
                case ActionPhase.Started:
                    action.started -= callback;
                    break;
                case ActionPhase.Performed:
                    action.performed -= callback;
                    break;
                case ActionPhase.Canceled:
                    action.canceled -= callback;
                    break;
            }
        }
    }

    public Vector2 GetMovementVector()
    {
        return _input.OnFoot.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookVector()
    {
        return _input.OnFoot.Look.ReadValue<Vector2>();
    }

}
