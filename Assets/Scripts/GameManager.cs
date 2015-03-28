using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> m_ItemSpawns;
    public List<int> m_Rounds;
    public GameObject m_Cashier;
    public GameObject m_CashierFX;
    public GameObject m_WinFX;
    public Text m_UIRound;
    public Text m_UITime;
    public Text m_Win;
    public float m_Time = 900f;

    private bool m_GameOver;
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
        m_UIRound.text = (m_CurRound+1).ToString();
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
            m_CashierFX.SetActive(true);
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

    public void EndGame(bool win = true)
    {
        m_GameOver = true;
        GameObject.Find("Player").GetComponent<PlayerControl>().m_MovementEnabled = false;
        m_WinFX.SetActive(true);
        if (win)
        {
            m_Win.text = "YOU WIN LULZ";
        }
        else
        {
            m_Win.text = "YOU LOSE LULZ";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_GameOver)
        {
            m_Time -= Time.deltaTime;
            m_UITime.text = m_Time.ToString();
            if(m_Time <= 0)
            {
                EndGame(false);
            }
        }
    }
}
