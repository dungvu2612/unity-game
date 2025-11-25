using UnityEngine;

public class PickupIdleFX : MonoBehaviour
{
    public float floatAmplitude = 0.1f;   
    public float floatFrequency = 2f;    
    public float scaleAmplitude = 0.1f;   
    public float scaleFrequency = 3f;   

    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    void Update()
    {
        float t = Time.time;

        // Nhún lên xuống
        transform.position = startPos + new Vector3(
            0f,
            Mathf.Sin(t * floatFrequency) * floatAmplitude,
            0f
        );

        // Phóng to thu nhỏ
        float s = 1f + Mathf.Sin(t * scaleFrequency) * scaleAmplitude;
        transform.localScale = startScale * s;
    }
}
