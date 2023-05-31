using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared
{
    public static string joinCode;
    public static System.Random criticalRandomGenerator;
    public static string username;
    public static GameMode gameMode;
    public static bool inCombat;
    public static string playerID;
}

public enum GameMode
{
    Singleplayer,Coop
}
