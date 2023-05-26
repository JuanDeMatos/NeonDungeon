using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotBossRoomObstacle : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("BossRoomMesh"))
            Destroy(this.gameObject);
    }

}
