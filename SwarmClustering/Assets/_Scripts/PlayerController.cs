using Unity.Entities;
using UnityEngine;

public class PlayerController : ComponentSystem
{
    public static float pitch = 0;
    public static float yaw = 0;


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
            UpdatePosition();
        }
        else
        {
            Debug.Log("Camera not set!");
        }
    }

    private void UpdateLook()
    {
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");
        Bootstrap.camera.transform.eulerAngles = new Vector3(pitch, yaw, 0f);

    }

    private void UpdatePosition()
    {


        if (Input.GetKey(KeyCode.W))
        {
            Bootstrap.camera.transform.position = Bootstrap.camera.transform.position + Bootstrap.camera.transform.rotation * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Bootstrap.camera.transform.position = Bootstrap.camera.transform.position + Bootstrap.camera.transform.rotation * Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Bootstrap.camera.transform.position = Bootstrap.camera.transform.position + Bootstrap.camera.transform.rotation * Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Bootstrap.camera.transform.position = Bootstrap.camera.transform.position + Bootstrap.camera.transform.rotation * Vector3.right;
        }
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