using System;
using UnityEngine;

public static class RobotActions
{
    public static Action OnToolLoaded;
    public static Action OnToolUnloaded;
    public static Action<GameObject> OnToolGrabbed;
    public static Action<GameObject> OnToolUngrabbed;
    public static Action<float> OnTimeUpdated;
    public static Action<int> OnProgramDurationUpdated;
}
