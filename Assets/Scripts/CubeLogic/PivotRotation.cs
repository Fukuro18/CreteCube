using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    // Variables de estado y control
    private List<GameObject> activeSide;
    private Vector3 localForward;
    private Vector3 mouseRef;
    private bool dragging = false;
    private bool autoRotating = false;
    private float sensitivity = 0.4f;
    private float speed = 300f; // Velocidad de rotación automática
    private Vector3 rotation;

    // Variables de alineación
    private Quaternion targetQuaternion;

    // Referencias a otros scripts
    private ReadCube readCube;
    private CubeState cubeState;

    // Influence vector for screen-space rotation mapping
    private Vector2 rotationInfluence;

    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    void LateUpdate()
    {
        if (dragging && !autoRotating)
        {
            SpinSide(activeSide);
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                RotateToRightAngle();
            }
        }
        if (autoRotating)
        {
            AutoRotate();
        }

    }


    // Inicia el proceso de arrastre para un lado del cubo. Llamado por CubeState.PickUp
    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = Input.mousePosition;
        dragging = true;

        // Vector perpendicular a la cara para rotar alrededor de él.
        // This is in the parent's local space (assuming parent is the Cube root)
        localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
    }




    public void SetInfluence(Vector2 influence)
    {
        rotationInfluence = influence;
    }

    public bool IsRotating()
    {
        return dragging || autoRotating;
    }

    // Calcula el giro manual mientras el usuario arrastra.
    private void SpinSide(List<GameObject> side)
    {
        // reset the rotation
        rotation = Vector3.zero;

        // current mouse position minus the last mouse position
        Vector3 mouseOffset = (Input.mousePosition - mouseRef);

        if (side == cubeState.front)
        {
            rotation.x = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.up)
        {
            rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity * 1;
        }
        if (side == cubeState.down)
        {
            rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.left)
        {
            rotation.z = (mouseOffset.x + mouseOffset.y) * sensitivity * 1;
        }
        if (side == cubeState.right)
        {
            rotation.z = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.back)
        {
            rotation.x = (mouseOffset.x + mouseOffset.y) * sensitivity * 1;
        }
        // rotate
        transform.Rotate(rotation, Space.Self);

        // store mouse
        mouseRef = Input.mousePosition;
    }

    // Prepara la rotación automática redondeando el ángulo.

    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.PickUp(side);
        Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        activeSide = side;
        autoRotating = true;

    }


    public void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;

        // Redondea cada eje al múltiplo de 90 grados más cercano
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        targetQuaternion.eulerAngles = vec;
        autoRotating = true; // Inicia el bucle AutoRotate en Update()
    }

    // Rota suavemente hacia el ángulo objetivo (múltiplo de 90).
    private void AutoRotate()
    {
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        // Comprueba si la rotación está completa (dentro de 1 grado).
        if (Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion; // Fija el ángulo exacto

            // FASE DE LIMPIEZA
            UnParentAll(); // Desvincula las piezas
            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState(); // Lee el nuevo estado del cubo

            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
        }
    }

    // Desvincula las 9 piezas giradas de su padre temporal (el pivot)
    private void UnParentAll()
    {
        foreach (GameObject go in activeSide)
        {
            // Asume que el padre raíz de todos los cubitos es 'readCube.transform' 
            // (el transform del objeto donde está el script ReadCube).
            go.transform.parent.transform.parent = readCube.transform;
        }
    }
}