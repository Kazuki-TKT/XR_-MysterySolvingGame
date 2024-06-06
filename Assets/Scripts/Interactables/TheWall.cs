using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteAlways]
public class TheWall : MonoBehaviour
{
    public UnityEvent OnDestroy;

    [SerializeField] int columns;

    [SerializeField] int rows;

    [SerializeField] GameObject wallCubePrefab;

    [SerializeField] GameObject socketWallPrefab;

    [SerializeField] int socketPosition = 1;

    [SerializeField] XRSocketInteractor wallSocket;

    public XRSocketInteractor GetWallSocket => wallSocket;

    [SerializeField] ExplosiveDevice explosiveDevice;

    [SerializeField] List<GenelatedColumn> genelatedColumn;

    [SerializeField] GameObject[] wallCubes;

    [SerializeField] float cubeSpacing = 0.005f;

    Vector3 cubeSize;

    Vector3 spawnPosition;

    [SerializeField] bool buildWall;

    [SerializeField] bool deleteWall;

    [SerializeField] bool destroyWall;

    [SerializeField] int maxPower;

    [SerializeField] AudioClip destroyWallClip;

    public AudioClip GetDestroyClip => destroyWallClip;

    [SerializeField] AudioClip socketClip;

    public AudioClip GetSocketedClip => socketClip;

    void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnSocketEnter);
            wallSocket.selectExited.AddListener(OnSocketExited);
        }

        if (explosiveDevice != null)
        {
            explosiveDevice.OnDetonated.AddListener(OnDestroyWall);
        }
    }

    void BuildWall()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        }

        spawnPosition = transform.position;
        int soketedColumn = Random.Range(0, columns);

        for (int i = 0; i < columns; i++)
        {
            if (i == soketedColumn)
            {
                GeneratoColumn(i, rows, true);
            }
            else
            {
                GeneratoColumn(i, rows, false);
            }
            spawnPosition.x += cubeSize.x + cubeSpacing;
        }
    }

    void GeneratoColumn(int index, int height, bool socketed)
    {
        GenelatedColumn tempColumn = new GenelatedColumn();
        tempColumn.InitializeColumn(transform, index, height, socketed);
        spawnPosition.y = transform.position.y;
        wallCubes = new GameObject[height];

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubePrefab != null)
            {
                wallCubes[i] = Instantiate(wallCubePrefab, spawnPosition, transform.rotation);
                tempColumn.SetCube(wallCubes[i]);
            }
            spawnPosition.y += cubeSize.y + cubeSpacing;
        }

        if (socketed && socketWallPrefab != null)
        {
            if (socketPosition == 0 || socketPosition >= height) socketPosition = 0;
            AddSocketWall(tempColumn);
        }

        genelatedColumn.Add(tempColumn);
    }

    void AddSocketWall(GenelatedColumn socketedColumn)
    {
        if (wallCubes[socketPosition] != null)
        {
            Vector3 position = wallCubes[socketPosition].transform.position;
            DestroyImmediate(wallCubes[socketPosition]);
            wallCubes[socketPosition] = Instantiate(socketWallPrefab, position, transform.rotation);
            socketedColumn.SetCube(wallCubes[socketPosition]);

            if (socketPosition == 0) wallCubes[socketPosition].transform.SetParent(transform);
            else wallCubes[socketPosition].transform.SetParent(wallCubes[0].transform);

            wallSocket = wallCubes[socketPosition].GetComponentInChildren<XRSocketInteractor>();

            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(OnSocketEnter);
                wallSocket.selectExited.AddListener(OnSocketExited);
            }
        }
    }

    private void OnSocketEnter(SelectEnterEventArgs arg0)
    {
        if (genelatedColumn.Count >= 1)
        {
            for (int i = 0; i < genelatedColumn.Count; i++)
            {
                //int power = Random.Range(maxPower / 2, maxPower);
                genelatedColumn[i].AcitivateColumn();
            }
        }
        //OnDestroy?.Invoke();
    }


    private void OnSocketExited(SelectExitEventArgs arg0)
    {
        if (genelatedColumn.Count >= 1)
        {
            for (int i = 0; i < genelatedColumn.Count; i++)
            {
                genelatedColumn[i].ResetColumn();
            }
        }
    }

    void OnDestroyWall()
    {
        if (genelatedColumn.Count >= 1)
        {
            for (int i = 0; i < genelatedColumn.Count; i++)
            {
                int power = Random.Range(maxPower / 2, maxPower);
                genelatedColumn[i].DestroyColumn(power);
            }
        }
        OnDestroy?.Invoke();
    }

    void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            BuildWall();
        }
        if (deleteWall)
        {
            deleteWall = false;
            for (int i = 0; i < genelatedColumn.Count; i++)
            {
                genelatedColumn[i].DeleteColumn();
            }
            if (genelatedColumn.Count >= 1)
            {
                genelatedColumn.Clear();
            }
        }
    }
}

[System.Serializable]
public class GenelatedColumn
{
    [SerializeField] GameObject[] wallCubes;

    bool isParented;

    [SerializeField] bool isSockted;

    [SerializeField] int columnIndex;

    Transform parentObject;

    Transform columnObject;

    const string Column_Name = "column";
    const string Socketed_Column_Name = "socketedcolumn";


    public void InitializeColumn(Transform parent, int index, int rows, bool soketed)
    {
        columnIndex = index;
        parentObject = parent;
        wallCubes = new GameObject[rows];
        isSockted = soketed;
    }

    public void SetCube(GameObject cube)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (!isParented)
            {
                isParented = true;
                SetColumnName(cube, columnIndex);
                cube.transform.SetParent(parentObject);
                columnObject = cube.transform;
            }
            else
            {
                cube.transform.SetParent(columnObject);
            }
            if (wallCubes[i] == null)
            {
                wallCubes[i] = cube;
                break;
            }
        }
    }

    void SetColumnName(GameObject column, int index)
    {
        if (isSockted)
        {
            column.name = Socketed_Column_Name;
        }
        else
        {
            column.name = Column_Name;
        }
        column.name += index.ToString();
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Object.DestroyImmediate(wallCubes[i]);
            }
            wallCubes = new GameObject[0];
        }
    }

    public void DestroyColumn(int power)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
                wallCubes[i].transform.SetParent(parentObject);
                rb.AddRelativeForce(Random.onUnitSphere * power);
            }
        }
    }

    public void ResetColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }
        }
    }

    public void AcitivateColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }
        }
    }
}