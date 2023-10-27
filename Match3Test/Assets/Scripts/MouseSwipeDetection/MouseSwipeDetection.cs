using Enums;
using UnityEngine;

namespace Match3Test
{
    public class MouseSwipeDetection : MonoBehaviour
    {
        private Vector3 mouseDownPosition;
        [SerializeField] private float Distance = 50f;

        public static Direction MouseSwipeDirection = Direction.None;
    
        public Direction GetDirection(Vector3 firstPos, Vector3 mousePos)
        {
            MouseSwipeDirection = Direction.None;
            
            mouseDownPosition = firstPos;
            Vector3 mouseUpPosition = mousePos;
            Vector3 swipeDirection = mouseUpPosition - mouseDownPosition;

            if (swipeDirection.magnitude > Distance)
            {
                if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                {
                    if (swipeDirection.x > 0)
                    {
                        MouseSwipeDirection = Direction.Right;
                        return MouseSwipeDirection;
                    }
                    else
                    {
                        MouseSwipeDirection =  Direction.Left;
                        return MouseSwipeDirection;
                    }
                }
                else
                {
                    if (swipeDirection.y > 0)
                    {
                        MouseSwipeDirection =  Direction.Up;
                        return MouseSwipeDirection;
                    }
                    else
                    {
                        MouseSwipeDirection =  Direction.Down;
                        return MouseSwipeDirection;
                    }
                }
            }

            return MouseSwipeDirection;
        }
    }
}

