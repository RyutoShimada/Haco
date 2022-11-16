using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Tutorial,
    Solo,
    Local,
    Online
}

public class AppManager : Singleton<AppManager>
{
    public GameMode GameMode { get; set; }
}
