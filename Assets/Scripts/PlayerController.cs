using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SocketManager socketManager;

    public Transform controlledObject;
    public float movementSpeed = 10f;
    public float decayRate = 2f; // Rate at which movement slows down when no input
    public Animator animator;

    private bool isMoving = false;
    private bool isRotatedLeft = false;
    private bool isRotatedRight = false;
    private float currentVelocity = 0f;

    public float minXPosition = -10f; // Minimum x-position
    public float maxXPosition = 10f; // Maximum x-position

    void Update()
    {
        if (!string.IsNullOrEmpty(socketManager.data))
        {
            // Parse received data
            string[] values = socketManager.data.Trim(new char[] { '[', ']' }).Split(',');
            if (values.Length > 0)
            {
                // Use the first value for horizontal movement
                if (int.TryParse(values[0], out int x))
                {
                    // Apply movement speed
                    float horizontalMovement = x * movementSpeed * Time.deltaTime;

                    // Apply position constraints
                    float newHorizontalPosition = Mathf.Clamp(controlledObject.position.x + horizontalMovement, minXPosition, maxXPosition);

                    // Check if movement is within bounds
                    if (newHorizontalPosition != controlledObject.position.x)
                    {
                        // Move the controlled object horizontally on the original x-axis
                        controlledObject.position = new Vector3(newHorizontalPosition, controlledObject.position.y, controlledObject.position.z);
                    }

                    // Check direction and rotate if needed
                    if (horizontalMovement > 0 && !isRotatedRight)
                    {
                        if (isRotatedLeft)
                        {
                            // Rotate to the right
                            controlledObject.eulerAngles = new Vector3(
                                controlledObject.eulerAngles.x,
                                controlledObject.eulerAngles.y - 100,
                                controlledObject.eulerAngles.z
                            );
                            isRotatedRight = true;
                            isRotatedLeft = false;
                        }
                        else
                        {
                            // Rotate to the right
                            controlledObject.eulerAngles = new Vector3(
                                controlledObject.eulerAngles.x,
                                controlledObject.eulerAngles.y - 50,
                                controlledObject.eulerAngles.z
                            );
                            isRotatedRight = true;
                            isRotatedLeft = false;
                        }
                    }
                    else if (horizontalMovement < 0 && !isRotatedLeft)
                    {
                        if (isRotatedRight)
                        {
                            // Rotate to the left
                            controlledObject.eulerAngles = new Vector3(
                                controlledObject.eulerAngles.x,
                                controlledObject.eulerAngles.y + 100,
                                controlledObject.eulerAngles.z
                            );
                            isRotatedLeft = true;
                            isRotatedRight = false;
                        }
                        else
                        {
                            // Rotate to the left
                            controlledObject.eulerAngles = new Vector3(
                                controlledObject.eulerAngles.x,
                                controlledObject.eulerAngles.y + 50,
                                controlledObject.eulerAngles.z
                            );
                            isRotatedLeft = true;
                            isRotatedRight = false;
                        }
                    }

                    // Set movement flag
                    isMoving = Mathf.Abs(horizontalMovement) > 0.01f;

                    // Update current velocity
                    currentVelocity = Mathf.Abs(horizontalMovement);
                }
                else
                {
                    isMoving = false;
                }
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }

        // Decay velocity gradually when no input
        currentVelocity -= decayRate * Time.deltaTime;
        currentVelocity = Mathf.Clamp01(currentVelocity);

        // Apply smoothed movement
        float smoothedMovement = currentVelocity * movementSpeed * Time.deltaTime;
        float newXPosition = Mathf.Clamp(controlledObject.position.x + smoothedMovement, minXPosition, maxXPosition);
        controlledObject.position = new Vector3(newXPosition, controlledObject.position.y, controlledObject.position.z);

        // Update animator based on movement status
        animator.SetBool("Move", isMoving);
    }

}
