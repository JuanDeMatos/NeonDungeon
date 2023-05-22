using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared
{
    public static string joinCode;
    public static string username;
    public static GameMode gameMode;
}

public enum GameMode
{
    Singleplayer,Coop
}
