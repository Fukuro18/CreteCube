using System.Collections.Generic;
using UnityEngine;

public class SelectFaceLeap : MonoBehaviour
{
    CubeState cubeState;
    ReadCube readCube;

    public float dragThreshold = 0.03f;

    private bool isDragging = false;
    private Vector3 fingerStartPos;
    private GameObject currentFace;
    private Collider currentFinger;

    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Detecta cuando el dedo entra al collider
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Finger")) return;

        fingerStartPos = other.transform.position;
        currentFinger = other;
        currentFace = gameObject;
        isDragging = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other == currentFinger)
        {
            isDragging = false;
            currentFace = null;
            currentFinger = null;
        }
    }

    void Update()
    {
        if (currentFace == null || currentFinger == null) return;

        // Calcula el delta de movimiento del dedo
        Vector3 fingerDelta = currentFinger.transform.position - fingerStartPos;

        if (!isDragging && fingerDelta.magnitude > dragThreshold)
        {
            // Bloquea si ya hay rotación en curso
            PivotRotation[] pivots = FindObjectsOfType<PivotRotation>();
            foreach (var p in pivots)
            {
                if (p.IsRotating()) return;
            }

            readCube.ReadState();
            isDragging = true;

            // Convierte el movimiento 3D en "drag" plano
            Vector3 projectedDrag = new Vector3(fingerDelta.x, fingerDelta.y, 0);

            AttemptPickup(currentFace, projectedDrag);
        }
    }

    // Mismo AttemptPickup de tu SelectFace original
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
                break;
            }
        }

        if (currentSide == null) return;

        Transform sideCenter = currentSide[4].transform.parent;

        Vector3 localY = face.transform.up;
        Vector3 localX = face.transform.right;

        Vector2 dragDir = new Vector2(dragVector.x, dragVector.y).normalized;
        Vector2 screenYDir = new Vector2(localY.x, localY.y).normalized;
        Vector2 screenXDir = new Vector2(localX.x, localX.y).normalized;

        float dotY = Mathf.Abs(Vector2.Dot(dragDir, screenYDir));
        float dotX = Mathf.Abs(Vector2.Dot(dragDir, screenXDir));

        bool isColDrag = dotY > dotX;

        List<GameObject> sliceToMove = null;
        Vector3 desiredRotationAxis = isColDrag ? localX : localY;

        foreach (var side in allSides)
        {
            if (side.Contains(face))
            {
                Vector3 sideAxis =
                    (side[4].transform.parent.position - readCube.transform.position).normalized;

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
            sliceToMove[4].transform.parent
                .GetComponent<PivotRotation>()
                .SetInfluence(influence);

            sliceToMove[4].transform.parent
                .GetComponent<PivotRotation>()
                .Rotate(sliceToMove);
        }
    }
}
