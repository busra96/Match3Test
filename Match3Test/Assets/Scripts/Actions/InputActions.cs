using Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Match3Test.Actions
{
    public class InputActions
    {
        public static UnityAction<Vector3> OnInputMouseDown;
        public static UnityAction<Vector3> OnInputMouseHold;
        public static UnityAction<Vector3> OnInputMouseUp;
        public static UnityAction OnUpdate;

        public static UnityAction<InputState> GameSetInputState;
    }
}

