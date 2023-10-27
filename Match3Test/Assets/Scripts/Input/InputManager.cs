using Enums;
using Match3Test.Actions;
using UnityEngine;

namespace Match3Test
{
    public class InputManager : MonoBehaviour
    {
        public InputState InputState;
        private bool timerActive;
        [SerializeField] private float DefaultTimer = 1.5f;
        private float timer;

        private void OnEnable()
        {
            InputActions.GameSetInputState += GameSetInputState;
            TileMatchAction.DropItemIsFalling += DropItemFalling;
        }
        private void OnDisable()
        {
            InputActions.GameSetInputState -= GameSetInputState;
            TileMatchAction.DropItemIsFalling -= DropItemFalling;
        }
        private void Start()
        {
            timer = DefaultTimer;
            Input.multiTouchEnabled = false;
        }
        private void Update()
        {
            Timer();
            
            InputActions.OnUpdate?.Invoke();
            if (Input.GetMouseButtonDown(0))
            {
                if (InputState == InputState.None)
                {
                    InputState = InputState.Swipe;
                    InputActions.OnInputMouseDown?.Invoke(Input.mousePosition);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (InputState == InputState.Swipe)
                {
                    InputActions.OnInputMouseHold?.Invoke(Input.mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (InputState == InputState.Swipe)
                {
                    InputActions.OnInputMouseUp?.Invoke(Input.mousePosition);
                }
            }
        }

        private void Timer()
        {
            if (timerActive)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timerActive = false;
                    timer = DefaultTimer;
                    
                    if (InputState == InputState.Match)
                    {
                        InputState = InputState.None;
                    }
                }
            }
        }

        private void DropItemFalling()
        {
            if (!timerActive)
            {
                timerActive = true;
                timer = DefaultTimer;
            }
            else
            {
                timer = DefaultTimer;
            }
        }

        private void GameSetInputState(InputState _inputState)
        {
            InputState = _inputState;
        }
    }
}

