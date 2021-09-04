﻿/*
Unity UI Extensions Line Renderer demo for drawing lines

Script has several operation modes:

1: Drag and Draw
In this mode the Drag events are used to drag a line between two points, when the next drag occurs the line continues

2: Click and Draw
In this mode, each click starts a new line with the new line followng the cursor until the next click. Pressing Escape or Right-Clicking stops drawing.
The next click will continue drawing the line (to create separate lines, you will need to update to using segments rather than just points

3: Follow
In this mode, the selected object will follow the drawn line

*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class DrawLine : MonoBehaviour, IDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool weCanDrag;
    public bool weCanEndDrag;
    public enum DemoMode { DragDraw, ClickDraw, Follow };
    GameObject LastCaller = null;
    public UILineRenderer lineRenderer;
    public DemoMode SceneDemoMode = DemoMode.DragDraw;

    private RectTransform RT;
    private Vector2 rectPos;
    public List<Vector2> points = new List<Vector2>();
    private int CurrentLine = 0;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        RT = GetComponent<RectTransform>();
        rectPos = RT.position;
    }

    #region Drag and Draw mode

    public Vector2 DragStartPos = Vector2.zero;

    public void CallWhenMouseDown(GameObject Caller)
    {
        weCanDrag = true;
        weCanEndDrag = false;
        Debug.Log("We Can Drag");
        LastCaller = Caller;
    }

    /// <summary>
    /// EventData/MousePosition updated every frame.  Grab the first drag start point as the beginning as the first point
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (SceneDemoMode == DemoMode.DragDraw && weCanDrag)
        {
            if (DragStartPos == Vector2.zero) // New click/drag
            {
                DragStartPos = eventData.position;
                if (points.Count < 1)
                {
                    //New line, add origin
                    points.Add(new Vector2(DragStartPos.x - rectPos.x, DragStartPos.y - rectPos.y));
                    CurrentLine += 1;
                }
                points.Add(new Vector2(DragStartPos.x - rectPos.x, DragStartPos.y - rectPos.y));
            }
            else
            {
                DrawLineToPoint(eventData.position);
            }
        }
    }

    /// <summary>
    /// When the user has finished clicking, add the end point and draw the line
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (SceneDemoMode == DemoMode.DragDraw && weCanDrag)
        {
            points[CurrentLine] = new Vector2(eventData.position.x - rectPos.x, eventData.position.y - rectPos.y);

            //Use the gathered points and update the line renderer
            RefreshLine();

            DragStartPos = Vector2.zero;
            for(int i = 0; i<3; i++)
            {
                weCanEndDrag = GameManager.IsPointerOverGameObject(GameManager._Instance.answerText[i].gameObject);
                if (weCanEndDrag)
                {
                    Debug.Log("Ending drag UwU");
                    CurrentLine = 0;
                    GameManager._Instance.lastDraggedObject.isDone = true;
                    GameManager._Instance.CreateNewLR();
                    weCanDrag = false;
                    if(GameManager._Instance.answerText[i].text == LastCaller.GetComponent<SpriteAnswer>().AnswerText)
                    {
                        GameManager._Instance.correctAnswerCount++;
                        Debug.Log("Correct answer");
                    } else
                    {
                        Debug.Log("Wrong answer");
                    }
                    
                    enabled = false;
                    break;
                }
            }
            if(!weCanEndDrag)
            {
                Debug.Log("wtf");
                CurrentLine = 0;
                points = new List<Vector2>(new Vector2[] { new Vector2(0,0)});
                RefreshLine();
                Destroy(GetComponent<UILineRenderer>());
                GameManager._Instance.StartCoroutine(GameManager._Instance.addAfterFew(gameObject));
                weCanDrag = false;
            }
        }
    }

    #endregion Drag and Draw mode

    #region Click and Draw mode

    private bool drawing = false;
    private bool mouseDown = false;

    void Update()
    {
        // If in Click Draw mode, update will continue to move the last line to the current mouse position until Esc or Right-Click is pressed
        if (SceneDemoMode == DemoMode.ClickDraw)
        {
            if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
            {
                drawing = false;
            }
            if (drawing)
            {
                DrawLineToPoint(Input.mousePosition);
            }
        }
    }

    /// <summary>
    /// For continuous lines, finish the last and create a new line for each click.
    /// 
    /// </summary>
    /// <remarks>
    /// I have used the Pointer Up and Pointer Down handlers here, as the generic Pointer Handler blocks the OnDragEnd handler.
    /// If you only intend to use click, you can use the IPointerClickHandler instead and drop the mouseDown properties.
    /// </remarks>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!drawing)
        {
            drawing = true;
        }

        if (!mouseDown && SceneDemoMode == DemoMode.ClickDraw && drawing)
        {
            if (points.Count < 1)
            {
                points.Add(new Vector2(eventData.position.x - rectPos.x, eventData.position.y - rectPos.y));
            }
            points.Add(new Vector2(eventData.position.x - rectPos.x, eventData.position.y - rectPos.y));
            RefreshLine();
            CurrentLine += 1;
        }
        mouseDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseDown = false;
    }

    #endregion Click and Draw mode

    #region Common Functions

    public void RefreshLine()
    {
        lineRenderer.Points = points.ToArray();
        lineRenderer.SetAllDirty();
    }

    public void DrawLineToPoint(Vector3 position)
    {
        if (points.Count > CurrentLine)
        {
            points[CurrentLine] = new Vector2(position.x - rectPos.x, position.y - rectPos.y);
            RefreshLine();
        }
    }

    #endregion  Common Functions
}