using UnityEngine;

public class ParallaxBG : MonoBehaviour
{

    private float length;
    private float startPos;
    public GameObject Camera;
    public float parallaxEffect;


    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float dist = (Camera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
    }
}
