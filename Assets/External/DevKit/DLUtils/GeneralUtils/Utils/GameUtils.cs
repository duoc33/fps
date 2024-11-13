using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils
{
    internal static void Mute(bool mute)
    {
        AudioListener.pause = mute;
    }

    public static void PauseRPGGame()
    {
        Time.timeScale = 0.0f;
        Mute(true);

    }

    public static void ResumeRPGGame()
    {
        Time.timeScale = 1.0f;
        Mute(false);
    }
}
