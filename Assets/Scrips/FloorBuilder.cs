using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBuilder : MonoBehaviour
{
    GameObject room;
    int habitacionesRestantes;
    bool[,] mapaHabitaciones;
    List<GameObject> placedRooms;

    // Start is called before the first frame update
    void Start()
    {
        habitacionesRestantes = 0;
        mapaHabitaciones = new bool[9,9];

        room = Resources.Load<GameObject>("Rooms/000_defaultRoom");

        mapaHabitaciones[4,4] = true;

        do {
            for (int i = 0; i < mapaHabitaciones.Length; i++)
            {
                for (int j = 0; j < mapaHabitaciones.GetLength(1); j++)
                {
                    if (mapaHabitaciones[i,j] && habitacionesRestantes > 0)
                    {
                        
                    }
                }
            }
        } while (habitacionesRestantes < 10);

        

    }

    Vector3 PlaceRoom(Vector3 lastRoom) {
        return Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
