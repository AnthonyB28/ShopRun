using UnityEngine;
using System.Collections.Generic;

public class DirectionEnabler : MonoBehaviour
{
    public Direction m_DirToEnable;
    public bool m_ForceOppsiteDir = false;
    public Direction m_ForceDirection;
    public bool m_DisableDirOnExit = true;
    public List<Direction> m_DirToDisableOnExit; // Only on the DirToEnable

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
