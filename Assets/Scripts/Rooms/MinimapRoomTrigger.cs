using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRoomTrigger : MonoBehaviour
{
    [SerializeField] private MinimapRoom minimap;
    protected bool active;

    // Start is called before the first frame update
    void Start()
    {
        this.Invoke(() => active = true, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        this.gameObject.SetActive(false);
        minimap.VisibleRoom();

    }
}
