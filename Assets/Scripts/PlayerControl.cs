using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    LEFT = 0,
    RIGHT,
    UP,
    DOWN,
    STOPPED
}

public class PlayerControl : MonoBehaviour
{

    public Direction m_Direction;
    public bool m_MovementEnabled = true;
    public float m_Speed = 1.0f;
    public float m_SpeedUp = 0.001f;
    public float m_SpeedUpPenalty = 7;
    public float m_CurSpeedUp = 0f;

    public bool m_LeftBlock;
    public bool m_RightBlock;
    public bool m_UpBlock;
    public bool m_DownBlock;

    public float minSwipeLength = 5f;

    private Direction m_PreviousDir;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    Vector2 firstClickPos;
    Vector2 secondClickPos;

    DirectionEnabler m_CurrentPathTrigger;

    // Use this for initialization
    void Start()
    {
        m_SpeedUpPenalty = 7 * m_SpeedUp * 50;
    }

    // Update is called once per frame
    void Update()
    {
        DetectSwipe();

        // Handle movement control
        if (m_MovementEnabled)
        {
            Vector3 direction = Vector3.zero;

            if (m_Direction != Direction.STOPPED)
            {
                switch (m_Direction)
                {
                    case Direction.LEFT: direction = Vector3.left; break;
                    case Direction.RIGHT: direction = Vector3.right ; break;
                    case Direction.UP: direction = Vector3.forward; break;
                    case Direction.DOWN: direction = Vector3.back; break;
                }

                // Handle positioning in lanes using saved trigger
                if (m_CurrentPathTrigger)
                {
                    // Disable the direction we're moving if disabled on exit flagged
                    if (m_Direction == m_CurrentPathTrigger.m_DirToEnable)
                    {
                        SetDirBlocks(m_CurrentPathTrigger.m_DirToDisableOnExit, true);
                    }

                    // Teleport into lane, must fix the jerky anim
                    if (m_Direction != m_PreviousDir)
                    {
                        transform.position = m_CurrentPathTrigger.transform.GetChild(0).position;
                    }
                }

                // Oppsite direction movement penalties and fixes.
                if(m_PreviousDir == Direction.LEFT && m_Direction == Direction.RIGHT ||
                    m_PreviousDir == Direction.RIGHT && m_Direction == Direction.LEFT ||
                    m_PreviousDir == Direction.UP && m_Direction == Direction.DOWN ||
                    m_PreviousDir == Direction.DOWN && m_Direction == Direction.UP)
                {
                    SetDirBlocks(m_PreviousDir, false); // Allow player to reverse dir
                    if(m_CurSpeedUp > m_SpeedUpPenalty)
                    {
                        m_CurSpeedUp -= m_SpeedUpPenalty;
                    }
                    else
                    {
                        m_CurSpeedUp = m_SpeedUp;
                    }
                }

                transform.Translate(direction * (m_Speed + m_CurSpeedUp) * Time.deltaTime);
                m_CurSpeedUp += m_SpeedUp;
                m_PreviousDir = m_Direction;
            }
            else
            {
                // Penalty for hitting a wall.
                m_CurSpeedUp = m_SpeedUp;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Stop the player if it hits a wall, push back
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
            transform.Translate(direction * 40 * Time.deltaTime);
            m_Direction = Direction.STOPPED;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "DirectionEnabler")
        {
            // Unset blocks from previous enabler
            if (m_CurrentPathTrigger)
            {
                SetDirBlocks(m_CurrentPathTrigger.m_DirToDisableOnExit, false);
            }

            // Disable direction blocks from trigger
            DirectionEnabler component = col.gameObject.GetComponent<DirectionEnabler>();
            m_CurrentPathTrigger = component;
            SetDirBlocks(component.m_DirToEnable, false);

            // Player will stop if entering from set direction
            if(component.m_ForceOppsiteDir)
            {
                if(m_Direction == component.m_ForceDirection)
                {
                    m_Direction = Direction.STOPPED;
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "DirectionEnabler")
        {
            DirectionEnabler component = col.gameObject.GetComponent<DirectionEnabler>();

            // Block dir if exited
            if (component.m_DisableDirOnExit)
            {
                SetDirBlocks(component.m_DirToEnable, true);
            }

            // Unset previous component if the same
            if(component == m_CurrentPathTrigger)
            {
                m_CurrentPathTrigger = null;
            }
        }
    }

    void SetDirBlocks(List<Direction> directions, bool setTrue)
    {
        foreach (Direction dir in directions)
        {
            SetDirBlocks(dir, setTrue);
        }
    }

    void SetDirBlocks(Direction dir, bool setTrue)
    {
        switch (dir)
        {
            case Direction.UP: m_UpBlock = setTrue; break;
            case Direction.DOWN: m_DownBlock = setTrue; break;
            case Direction.LEFT: m_LeftBlock = setTrue; break;
            case Direction.RIGHT: m_RightBlock = setTrue; break;
        }
    }


    // http://forum.unity3d.com/threads/swipe-in-all-directions-touch-and-mouse.165416/
    void DetectSwipe()
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
}
