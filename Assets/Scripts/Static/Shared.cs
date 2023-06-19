using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared
{
    public static string joinCode;
    public static System.Random criticalRandomGenerator;
    public static string username = "Player";
    public static GameMode gameMode;
    public static ConnectionState connectionState = ConnectionState.DEFAULT;
    public static Controller controller = Controller.KeyboardAndMouse;
    public static bool inCombat;
    public static bool logged;
    public static int dailyRunSeed = 0;

}

public enum GameMode
{
    Singleplayer,Coop,DailyRun
}

public enum ConnectionState
{
    YES,NO,DEFAULT
}

public enum Controller
{
    Gamepad, KeyboardAndMouse
}
