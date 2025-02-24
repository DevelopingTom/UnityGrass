using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    public List<Collider> itemsNear { get; private set; } = new List<Collider>();
    public GameObject itemHolded;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float moveDuration = 0.5f; // Duration of the move

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (itemsNear.Count > 0 && Input.GetKeyDown(KeyCode.E) && !itemHolded)
        {
            Hold(itemsNear[0].gameObject); // Pick the first item from the list
        } 
        else if (Input.GetKeyDown(KeyCode.E) && itemHolded)
        {
            Drop();
        }
    }

    private void Drop()
    {
        itemHolded.transform.SetParent(null);
        if (itemHolded.GetComponent<Rigidbody>())
        {
            Rigidbody itemRigidbody = itemHolded.GetComponent<Rigidbody>();
            itemRigidbody.isKinematic = false;
            Vector3 slantedForce = (transform.forward * 1.3f + transform.up).normalized * throwForce; // Modify this line to slant the force
            itemRigidbody.AddForce(slantedForce, ForceMode.VelocityChange);
        }
        itemHolded.GetComponent<Collider>().enabled = true;
        itemHolded = null;
    }

    private void Hold(GameObject item)
    {
        itemHolded = item;
        itemsNear.Remove(item.GetComponent<Collider>());
        if (item.GetComponent<Rigidbody>())
        {
            item.GetComponent<Rigidbody>().isKinematic = true;
        }
        var itemCollider = item.GetComponent<Collider>();
        itemCollider.enabled = false;
        StartCoroutine(MoveItemToHolder(item)); // Start the coroutine to move the item
    }

    private IEnumerator MoveItemToHolder(GameObject item)
    {
        Vector3 startPosition = item.transform.position;
        Quaternion startRotation = item.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            Vector3 endPositionActual = transform.position + transform.up * (1.7f + item.GetComponent<Collider>().bounds.size.y / 2);
            Quaternion endRotationActual = transform.rotation;
            item.transform.position = Vector3.Lerp(startPosition, endPositionActual, elapsedTime / moveDuration);
            item.transform.rotation = Quaternion.Lerp(startRotation, endRotationActual, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 endPosition = transform.position + transform.up * (1.7f + item.GetComponent<Collider>().bounds.size.y / 2);
        Quaternion endRotation = transform.rotation;
        item.transform.position = endPosition;
        item.transform.rotation = endRotation;
        item.transform.SetParent(transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (!itemsNear.Contains(other)) itemsNear.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            itemsNear.Remove(other);
        }
    }
}
