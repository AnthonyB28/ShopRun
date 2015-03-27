using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{

    public enum Direction
    {
        LEFT = 0,
        RIGHT,
        UP,
        DOWN,
        STOPPED
    }

    public Direction m_Direction;
    public bool m_MovementEnabled = true;
    public float m_Speed = 1.0f;
    public float m_SpeedUp = 0.001f;
    private float m_CurSpeedUp = 0f;

    public bool m_LeftBlock;
    public bool m_RightBlock;
    public bool m_UpBlock;
    public bool m_DownBlock;

    public float minSwipeLength = 5f;

    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    Vector2 firstClickPos;
    Vector2 secondClickPos;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        DetectSwipe();

        // Perform movement
        if (m_MovementEnabled)
        {
            Vector3 direction = Vector3.zero;

            if (m_Direction != Direction.STOPPED)
            {
                // Might be able to bring this switch case out of Update()
                switch (m_Direction)
                {
                    case Direction.LEFT: direction = Vector3.left; break;
                    case Direction.RIGHT: direction = Vector3.right ; break;
                    case Direction.UP: direction = Vector3.forward; break;
                    case Direction.DOWN: direction = Vector3.back; break;
                }

                transform.Translate(direction * (m_Speed + m_CurSpeedUp) * Time.deltaTime);
                m_CurSpeedUp += m_SpeedUp;
            }
            else
            {
                m_CurSpeedUp = m_SpeedUp;
            }
        }
    }

    // http://forum.unity3d.com/threads/swipe-in-all-directions-touch-and-mouse.165416/
    public void DetectSwipe()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                SetDirectionFromGesture(currentSwipe);
            }
        }
        else
        {

            if (Input.GetMouseButtonDown(0))
            {
                firstClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                secondClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                currentSwipe = new Vector3(secondClickPos.x - firstClickPos.x, secondClickPos.y - firstClickPos.y);

                SetDirectionFromGesture(currentSwipe);
            }
        }
    }

    void SetDirectionFromGesture(Vector3 currentSwipe)
    {
        // Make sure it was a legit swipe, not a tap
        if (currentSwipe.magnitude < minSwipeLength)
        {
            return;
        }

        currentSwipe.Normalize();

        if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f && !m_UpBlock)
        {
            m_Direction = Direction.UP;
        }
        else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f && !m_DownBlock)
        {
            m_Direction = Direction.DOWN;
        }
        else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f && !m_LeftBlock)
        {
            m_Direction = Direction.LEFT;
        }
        else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f && !m_RightBlock)
        {
            m_Direction = Direction.RIGHT;
        }
    }

    void OnCollisionEnter(Collision col)
    {

        if(col.gameObject.tag == "Wall")
        {
            Vector3 direction = Vector3.zero;
            switch (m_Direction)
            {
                case Direction.LEFT: direction = Vector3.right; break;
                case Direction.RIGHT: direction = Vector3.left; break;
                case Direction.UP: direction = Vector3.back; break;
                case Direction.DOWN: direction = Vector3.forward; break;
            }
            transform.Translate(direction * 15 * Time.deltaTime);
            m_Direction = Direction.STOPPED;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "RightDetector")
        {
            m_LeftBlock = true;
        }
        else if (col.gameObject.tag == "LeftDetector")
        {
            m_RightBlock = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "RightDetector")
        {
            m_LeftBlock = false;
        }
        else if (col.gameObject.tag == "LeftDetector")
        {
            m_RightBlock = false;
        }
    }
}
