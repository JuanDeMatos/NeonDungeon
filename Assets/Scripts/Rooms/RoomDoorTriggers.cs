using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoorTriggers : MonoBehaviour
{
    [SerializeField] protected Room room;
    protected bool active;

    // Start is called before the first frame update
    void Start()
    {
        this.Invoke(() => active = true,1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("StartRoom");
        room.StartRoom();
    }
}
