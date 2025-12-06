using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFace : MonoBehaviour
{
    CubeState cubeState;
    ReadCube readCube;
    int layerMask = 1 << 8;

    // Drag handling
    private bool isDragging = false;
    private Vector3 mouseDownPos;
    private float dragThreshold = 10f;
    private GameObject currentFace;


    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Input.mousePosition;

            // Raycast to find the initial face
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                currentFace = hit.collider.gameObject;
            }
            else
            {
                currentFace = null;
            }
        }
        else if (Input.GetMouseButton(0) && !isDragging && currentFace != null)
        {
            Vector3 mouseCurrentPos = Input.mousePosition;
            if (Vector3.Distance(mouseDownPos, mouseCurrentPos) > dragThreshold)
            {
                // Drag detected!
                isDragging = true;

                PivotRotation[] pivots = FindObjectsOfType<PivotRotation>();
                foreach (var p in pivots)
                {
                    if (p.IsRotating())
                    {
                        isDragging = false; // Cancel drag
                        return;
                    }
                }

                readCube.ReadState(); // Ensure state is fresh

                // Determine which side to rotate based on drag direction
                AttemptPickup(currentFace, mouseCurrentPos - mouseDownPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            currentFace = null;
        }
    }

    public void AttemptPickup(GameObject face, Vector3 dragVector)
    {

        List<List<GameObject>> allSides = new List<List<GameObject>>()
        {
            cubeState.up,
            cubeState.down,
            cubeState.left,
            cubeState.right,
            cubeState.front,
            cubeState.back
        };


        List<GameObject> currentSide = null;
        foreach (var side in allSides)
        {
            if (side.Contains(face))
            {
                currentSide = side;
                break; // Found the side this sticker belongs to
            }
        }

        if (currentSide == null) return;

        Transform sideCenter = currentSide[4].transform.parent;

        // The normal of the side
        Vector3 sideNormal = Vector3.zero;


        sideNormal = face.transform.forward;

        Vector3 colDirection = Vector3.up;
        Vector3 rowDirection = Vector3.right;


        Vector3 localY = face.transform.up;
        Vector3 localX = face.transform.right;

        // Project these to screen space
        Vector3 screenY = Camera.main.WorldToScreenPoint(face.transform.position + localY) - Camera.main.WorldToScreenPoint(face.transform.position);
        Vector3 screenX = Camera.main.WorldToScreenPoint(face.transform.position + localX) - Camera.main.WorldToScreenPoint(face.transform.position);

        // Normalize for direction comparison
        Vector2 dragDir = new Vector2(dragVector.x, dragVector.y).normalized;
        Vector2 screenYDir = new Vector2(screenY.x, screenY.y).normalized;
        Vector2 screenXDir = new Vector2(screenX.x, screenX.y).normalized;

        float dotY = Mathf.Abs(Vector2.Dot(dragDir, screenYDir));
        float dotX = Mathf.Abs(Vector2.Dot(dragDir, screenXDir));

        bool isColDrag = dotY > dotX;



        List<GameObject> sliceToMove = null;



        Vector3 desiredRotationAxis = isColDrag ? localX : localY;



        foreach (var side in allSides)
        {
            if (side.Contains(face))
            {

                Transform center = side[4].transform.parent;

                Vector3 sideAxis = (side[4].transform.parent.transform.position - readCube.transform.position).normalized;

                if (Mathf.Abs(Vector3.Dot(sideAxis, desiredRotationAxis)) > 0.5f)
                {
                    sliceToMove = side;
                    break;
                }
            }
        }

        if (sliceToMove != null)
        {
            cubeState.PickUp(sliceToMove);

            Vector2 influence = isColDrag ? screenYDir : screenXDir;

            sliceToMove[4].transform.parent.GetComponent<PivotRotation>().SetInfluence(influence);
            sliceToMove[4].transform.parent.GetComponent<PivotRotation>().Rotate(sliceToMove);
        }
    }
}