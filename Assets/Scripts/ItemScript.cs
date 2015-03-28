using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).Rotate(Vector3.left, 1);
    }

    void OnTriggerEnter(Collider col)
    {
        gameObject.SetActive(false);
        GameManager.Instance.PickupItem();
    }
}
