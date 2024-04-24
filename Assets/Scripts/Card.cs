using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int value;
    public float moveSpeed = 5f;
    public GameObject nextCardObject;
    public Transform nextCardPos;

    void Start()
    {
        nextCardObject = transform.GetChild(0).gameObject;
        nextCardPos = nextCardObject.transform;
    }

    public IEnumerator MoveToTarget(Transform targetTransform)
    {
        float initialDistance = Vector3.Distance(transform.position, targetTransform.position);

        while (initialDistance > 0.01f)
        {
            float distanceToMove = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, distanceToMove);
            initialDistance = Vector3.Distance(transform.position, targetTransform.position);
            yield return null;
        }
    }

    public void FlipCard()
    {
        StartCoroutine(MoveAndFlip());
    }

    IEnumerator MoveAndFlip()
    {
        float moveDistance = 0.1f;
        float moveDuration = 0.5f;
        float flipDuration = 1.0f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * moveDistance;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0);

        // Move up
        float elapsedTime = 0.0f;
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;

        // Flip
        elapsedTime = 0.0f;
        while (elapsedTime < flipDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;

        // Move back down
        elapsedTime = 0.0f;
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(endPosition, startPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }
}
