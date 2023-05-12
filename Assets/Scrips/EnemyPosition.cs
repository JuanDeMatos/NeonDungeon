using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosition : MonoBehaviour
{
    public EnemyType enemyType;
}

public enum EnemyType { Chaser, Creeper, Divisor, Teleporter, Turret}

