using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableAnswer : MonoBehaviour
{
    public bool isDone = false;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.IsPointerOverGameObject(gameObject) && (!isDone))
        {
            Debug.Log("Clicked " + transform.name);
            GameManager._Instance.lastDraggedObject = this;
            GameManager._Instance.lineRenderer.gameObject.GetComponent<DrawLine>().CallWhenMouseDown(gameObject);
        }
    }
}
