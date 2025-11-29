using System.Collections;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    public float duration = 5f;
    private float currentPosition = 0f;
    private bool gointToB = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = pointA.transform.position;

    }

    private void Update()
    {
        if (pointA == null || pointB == null)
            return;
        currentPosition += Time.deltaTime / duration;

        if(gointToB)
        {
            transform.position = Vector3.Lerp(pointA.transform.position, pointB.transform.position, currentPosition);
        }
        else
        {
            transform.position = Vector3.Lerp(pointB.transform.position, pointA.transform.position, currentPosition);
        }

        if(currentPosition >= 1f)
        {
            currentPosition = 0f;
            gointToB = !gointToB;
        }
    }
}
