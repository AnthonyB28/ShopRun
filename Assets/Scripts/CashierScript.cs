using UnityEngine;
using System.Collections;

public class CashierScript : MonoBehaviour {

    private bool m_GoalEnabled = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Enable()
    {
        m_GoalEnabled = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if(m_GoalEnabled)
        {
            Debug.Log("Win");
        }
    }
}
