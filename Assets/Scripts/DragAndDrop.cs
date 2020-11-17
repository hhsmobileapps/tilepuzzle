using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AudioClip pickSFX, dropSFX;

    Vector3 initialPos;
    int initalIndex;
    
    void Start()
    {
        initalIndex = gameObject.transform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Drag Begin");
        SoundManager.Instance.PlaySFX(pickSFX);

        initialPos = transform.localPosition;
        gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");

        //Create a ray going from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        //Calculate the distance between the Camera and the GameObject, and go this distance along the ray
        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(transform.position, Camera.main.transform.position));
        //Move the GameObject when you drag it
        transform.position = rayPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("Drag Ended");

        SoundManager.Instance.PlaySFX(dropSFX);
        // Send a ray to detect collider, if it is detected interchange the two tiles' positions
        RaycastHit2D hitedNext = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(eventData.position));
        if (hitedNext.collider != null)
        {
            //print("picked tile: " + gameObject.name + " hit collider: " + hitedNext.collider.gameObject.name);
            Vector3 posOfNewPuzzle = hitedNext.transform.localPosition;
            hitedNext.transform.localPosition = initialPos;
            transform.localPosition = posOfNewPuzzle;
        }
        else
        {
            //print("cant hit any collider");
            transform.localPosition = initialPos;
        }
        
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.transform.SetSiblingIndex(initalIndex);

        // Check whether all tiles set up correctly after each drag
        FindObjectOfType<LevelManager>().CheckIsFinished();
    }
}
