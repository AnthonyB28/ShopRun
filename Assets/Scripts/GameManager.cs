using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> m_ItemSpawns;
    public List<int> m_Rounds;
    public GameObject m_Cashier;

    private List<GameObject> m_CurItemSpawns;
    private int m_CurRound = -1;
    private int m_CurItemsOnField;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        foreach (GameObject item in m_ItemSpawns)
        {
            item.SetActive(false);
        }
        m_CurItemSpawns = new List<GameObject>(m_ItemSpawns);
        BeginRound();
    }

    void BeginRound()
    {
        ++m_CurRound;
        if (m_CurRound <= m_Rounds.Count - 1)
        {
            int numOfItemsToSpawn = m_Rounds[m_CurRound];
            for (int i = 0; i < numOfItemsToSpawn; ++i)
            {
                SpawnItem();
            }
        }
        else
        {
            Debug.Log("End game");
            m_Cashier.GetComponent<CashierScript>().Enable();
        }
    }

    void SpawnItem()
    {
        int item = Random.Range(0, m_CurItemSpawns.Count);
        m_CurItemSpawns[item].SetActive(true);
        m_CurItemSpawns.RemoveAt(item);
        ++m_CurItemsOnField;
        if (m_CurItemSpawns.Count == 0)
        {
            m_CurItemSpawns = new List<GameObject>(m_ItemSpawns);
        }
    }

    public void PickupItem()
    {
        --m_CurItemsOnField;
        // Do something with score
        if (m_CurItemsOnField == 0)
        {
            BeginRound();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
