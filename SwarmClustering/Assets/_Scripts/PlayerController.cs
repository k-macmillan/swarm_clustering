using Unity.Entities;
using UnityEngine;

public class PlayerController : ComponentSystem
{
    public static float pitch = 0;
    public static float yaw = 0;
    public static double timer = 0d;
    public const double delay = 0.25d;
    public const float speed = 0.5f;


    // Start is called before the first frame update
    public void Start()
    {
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUpdate()
    {
        if (Bootstrap.camera != null)
        {
            UpdateLook();
            HandleKeyboard();
        }
    }

    private void UpdateLook()
    {
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");
        Bootstrap.camera.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void HandleKeyboard()
    {
        UpdatePosition();
        HandleSpeedChange();
        HandleExit();
        HandleAntToggle();
    }

    private void HandleAntToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwarmMechanics.meshToggle = true;
        }
    }

    private void UpdatePosition()
    {
        // Movement Handling
        if (Input.GetKey(KeyCode.W))
        {
            CameraClamp(new Vector3(0, 0, speed));
        }
        if (Input.GetKey(KeyCode.S))
        {
            CameraClamp(new Vector3(0, 0, -speed));
        }
        if (Input.GetKey(KeyCode.A))
        {
            CameraClamp(new Vector3(-speed, 0, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            CameraClamp(new Vector3(speed, 0, 0));
        }
    }

    private void CameraClamp(Vector3 newPos)
    {
        Vector3 pos = Bootstrap.camera.transform.position + Bootstrap.camera.transform.rotation * newPos;
        if (pos.y < 1f)
        {
            pos.y = 1f;
        }
        Bootstrap.camera.transform.position = pos;
    }

    private void HandleSpeedChange()
    {
        timer += Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (timer > delay)
            {
                Common.Delay -= 0.015625f;
                if (Common.Delay < 0f)
                {
                    Common.Delay = 0f;
                }
                timer = 0;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (timer > delay)
            {
                Common.Delay += 0.015625f;
                if (Common.Delay > 0.3125f)
                {
                    Common.Delay = 0.3125f;
                }
                timer = 0;
            }
        }
    }

    private void HandleExit()
    {
        // Exit handling
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_STANDALONE
            Application.Quit(0);
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}