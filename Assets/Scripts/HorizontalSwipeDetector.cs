using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSwipeDetector : MonoBehaviour
{
    public float minDragDistance;
    private Vector2 startPos;
    private Vector2 direction;
    private bool swipeAccepted;
    private float xDis;
    private float yDis;
    private float lowestNumX;
    private float highestNumX;
    private float lowestNumY;
    private float highestNumY;
    public GameObject swipeArea;
    private Touch touch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            // If there is just one touch on screen, set touch to that one
            if (Input.touchCount == 1)
            {
                touch = Input.touches[0];
            }
            // If there is already a touch on the screen, set touch to the next one 
            else if (Input.touchCount == 2)
            {
                touch = Input.touches[1];
            }
            //  If there is already TWO touches on the screen, set touch to the next one 
            else if (Input.touchCount == 3)
            {
                touch = Input.touches[2];
            }

            // Check if the user touched the swipe area
            if (RectTransformUtility.RectangleContainsScreenPoint(swipeArea.GetComponent<RectTransform>(), touch.position))
            {
                // Handle finger movements based on touch phsae
                switch (touch.phase)
                {
                    // record initial touch position
                    case TouchPhase.Began:
                        startPos = touch.position;
                        break;

                    // Determine direction by comparing the current touch position with the initial one.
                    case TouchPhase.Moved:
                        direction = touch.position - startPos;

                        // Determine the largest number of the x and y values and set it to a value
                        if (touch.position.x > startPos.x)
                        {
                            highestNumX = touch.position.x;
                            lowestNumX = startPos.x;
                        }
                        else
                        {
                            highestNumX = startPos.x;
                            lowestNumX = touch.position.x;
                        }

                        xDis = highestNumX - lowestNumX;

                        if (touch.position.y > startPos.y)
                        {
                            highestNumY = touch.position.y;
                            lowestNumY = startPos.y;
                        }
                        else
                        {
                            highestNumY = startPos.y;
                            lowestNumY = touch.position.y;
                        }

                        yDis = highestNumY - lowestNumY;

                        // Determine if the drag was long enough
                        if (xDis >= minDragDistance)
                        {
                            //alreadyattemped = true;

                            // Determine if the drag was horizontal or not
                            if (xDis > yDis * 2)
                            {
                                swipeAccepted = true;
                            }
                        }
                        else
                        {
                            swipeAccepted = false;
                        }
                        break;

                    // Report that a direction has been chosen when the finger is lifted.
                    case TouchPhase.Ended:
                        if (swipeAccepted)
                        {
                            if (startPos.x > touch.position.x)
                            {
                                //Do something
                            }
                            else
                            {
                                //Do something else
                            }

                        }
                        else
                        {
                        }
                        break;
                }
            }
        }
    }
}
