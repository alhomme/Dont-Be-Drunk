using UnityEngine;

[CreateAssetMenu(fileName = "RSO_InputSource", menuName = "RSO/RSO Input Source")]
public class RSO_InputSource : RuntimeScriptableObject<InputSource>
{
}

public enum InputSource
{
    Keyboard,
    Gamepad
}
