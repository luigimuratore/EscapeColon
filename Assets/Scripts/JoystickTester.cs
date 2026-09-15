using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class JoystickTester : MonoBehaviour
{
    [Header("Console")]
    public float logInterval = 0.20f;
    public float deadZone = 0.10f;

    private float nextLogTime;

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        Gamepad pad = Gamepad.current;

        if (pad == null)
        {
            if (Time.unscaledTime >= nextLogTime)
            {
                Debug.Log("JOYSTICK TEST: nessun Gamepad rilevato dal nuovo Input System.");
                nextLogTime = Time.unscaledTime + 1f;
            }

            return;
        }

        Vector2 left = pad.leftStick.ReadValue();
        Vector2 right = pad.rightStick.ReadValue();

        float l2 = pad.leftTrigger.ReadValue();
        float r2 = pad.rightTrigger.ReadValue();

        bool somethingMoved =
            left.magnitude > deadZone ||
            right.magnitude > deadZone ||
            l2 > deadZone ||
            r2 > deadZone;

        if (somethingMoved && Time.unscaledTime >= nextLogTime)
        {
            Debug.Log(
                $"JOYSTICK | " +
                $"LEFT ({left.x:F2}, {left.y:F2}) | " +
                $"RIGHT ({right.x:F2}, {right.y:F2}) | " +
                $"L2 {l2:F2} | R2 {r2:F2}"
            );

            nextLogTime = Time.unscaledTime + logInterval;
        }

        if (pad.buttonSouth.wasPressedThisFrame)
            Debug.Log("BUTTON: South / A / Cross");

        if (pad.buttonEast.wasPressedThisFrame)
            Debug.Log("BUTTON: East / B / Circle");

        if (pad.buttonWest.wasPressedThisFrame)
            Debug.Log("BUTTON: West / X / Square");

        if (pad.buttonNorth.wasPressedThisFrame)
            Debug.Log("BUTTON: North / Y / Triangle");

        if (pad.leftShoulder.wasPressedThisFrame)
            Debug.Log("BUTTON: L1 / LB");

        if (pad.rightShoulder.wasPressedThisFrame)
            Debug.Log("BUTTON: R1 / RB");

        if (pad.leftStickButton.wasPressedThisFrame)
            Debug.Log("BUTTON: L3");

        if (pad.rightStickButton.wasPressedThisFrame)
            Debug.Log("BUTTON: R3");

        if (pad.startButton.wasPressedThisFrame)
            Debug.Log("BUTTON: Start / Options");

        if (pad.selectButton.wasPressedThisFrame)
            Debug.Log("BUTTON: Select / Back / Create");

#else
        if (Time.unscaledTime >= nextLogTime)
        {
            Debug.LogWarning(
                "JOYSTICK TEST: nuovo Input System non attivo. " +
                "Vai in Project Settings > Player > Active Input Handling e scegli Both."
            );

            nextLogTime = Time.unscaledTime + 2f;
        }
#endif
    }
}
