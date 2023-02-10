using UnityEngine;


public class GyroControl : MonoBehaviour
{
    Vector3 recalibrateAngleVector;
    Vector3 recalibratePositionVector;

    [SerializeField] Transform cam;

    [HideInInspector] Quaternion startQuaternion;
    [HideInInspector] public Quaternion offset;

    [HideInInspector] public bool enableGyro;

    Gyroscope gyro;

    Vector3 accel;

    float time = 0;

    void OnEnable()
    {
        EnableGyro();

        recalibrateAngleVector = cam.eulerAngles;
        recalibratePositionVector = cam.position;

        startQuaternion = new Quaternion(0, 0, 0, 0);

        time = 0;

        
    }

    private void OnDisable()
    {
        startQuaternion = new Quaternion(0, 0, 0, 0);
    }


    private void FixedUpdate()
    {
        return;
        if (time < 2)
        {
            time += Time.deltaTime;
        }

        if (time > 2)
        {
            if (startQuaternion.x == 0 && startQuaternion.y == 0 && startQuaternion.z == 0 && startQuaternion.w == 0 && enableGyro)
            {
                startQuaternion = gyro.attitude;

                offset = cam.rotation * Quaternion.Inverse(GyroToUnity(startQuaternion));
            }

            if (enableGyro)
            {
                cam.rotation = Quaternion.Lerp(cam.rotation, offset * GyroToUnity(gyro.attitude), 0.2f);
                //cam.localEulerAngles = new Vector3(Mathf.Clamp(cam.localEulerAngles.x, -50, 75), Mathf.Clamp(cam.localEulerAngles.y, -80, 80), Mathf.Clamp(cam.localEulerAngles.z, -25, 25));
            }
        }
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            enableGyro = true;

            return true;
        }
        return false;
    }

    float gyroScalar = 10;


    Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }


    void RecalibrateGyro()
    {
        transform.eulerAngles = recalibrateAngleVector;

        startQuaternion = gyro.attitude;

        offset = cam.rotation * Quaternion.Inverse(GyroToUnity(startQuaternion));
    }
}
