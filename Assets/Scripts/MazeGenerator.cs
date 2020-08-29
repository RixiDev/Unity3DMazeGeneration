using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    #region Variables
    [Header("Maze Variables")]
    [Range(3, 60)] public int columsCount = 3;
    [Range(3, 60)] public int rowsCount = 3;
    public bool instantGenerate;
    public static int cols, rows;

    [Header("Cell Variables")]
    public GameObject cellPrefab;
    public static List<Cell> grid = new List<Cell>();
    public static List<Cell> stack = new List<Cell>();
    public static List<Cell> correctPath = new List<Cell>();
    public static List<GameObject> deleteCell = new List<GameObject>();
    private Cell current;
    private Cell next;
    private int pathIndex = 0;
    private bool isPath = false;

    [Header("Materials Variables")]
    public Material visitedMaterial;
    public Material currentMaterial;
    public Material pathMaterial;
    public Material endMaterial;

    [Header("Camera Variables")]
    public Camera mainCamera;
    private float camX, camZ;
    #endregion

    #region Methods
    private void Start()
    {
        SetupGrid();

        if (instantGenerate == true)
        {
            while (true)
            {
                MazeLogic();
                ColorCell();
                if (stack.Count == 0)
                {
                    break;
                }
            }
        }

        CamSettings();
    }
    private void Update()
    {
        if (instantGenerate == false)
        {
            MazeLogic();
            CorrectPath();
        }
    }

    //Basic Methods
    private void SetupGrid()
    {
        cols = columsCount;
        rows = rowsCount;

        for (int q = 0; q < rows; q++)
        {
            for (int i = 0; i < cols; i++)
            {
                Cell cell = new Cell();
                cell.x = i;
                cell.z = q;

                grid.Add(cell);
            }
        }
        for (int i = 0; i < grid.Count; i++)
        {
            grid[i].prefab = Instantiate(cellPrefab, new Vector3(grid[i].x, 0, grid[i].z), Quaternion.identity, transform);
        }
        current = grid[0];
    }
    private void MazeLogic()
    {
        current.visited = true;
        //Step 1
        next = current.CheckNeighbors();
        current.neighbors = new List<Cell>();
        if (next != null)
        {
            next.visited = true;
            //Step 2
            stack.Add(current);
            //Step 3
            RemoveWalls(current, next);
            //Step 4
            current = next;
            //Extra Color
            ColorCell();
        }
        else if (stack.Count > 0)
        {
            current = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            //Extra Color
            ColorCell();
        }
        else
        {
            foreach (GameObject go in deleteCell)
            {
                Destroy(go);
            }
            if (pathIndex < correctPath.Count - 1)
            {
                pathIndex++;
                correctPath[pathIndex].prefab.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = pathMaterial;
                Debug.Log(pathIndex);
            }
        }
    }
    private void RemoveWalls(Cell a, Cell b)
    {
        int x = a.x - b.x;
        if (x == 1)
        {
            a.walls[0] = false;
            b.walls[2] = false;
        }
        else if (x == -1)
        {
            a.walls[2] = false;
            b.walls[0] = false;
        }

        int z = a.z - b.z;
        if (z == 1)
        {
            a.walls[3] = false;
            b.walls[1] = false;
        }
        else if (z == -1)
        {
            a.walls[1] = false;
            b.walls[3] = false;
        }

        a.CalculateWalls();
        b.CalculateWalls();
    }

    //Extra Methods
    private void CamSettings()
    {
        if (columsCount % 2 == 0)
        {
            camX = (columsCount / 2) - 0.5f;
        }
        else if (columsCount % 2 == 1)
        {
            camX = columsCount / 2;
        }

        if (rowsCount % 2 == 0)
        {
            camZ = (rowsCount / 2) - 0.5f;
        }
        else if (rowsCount % 2 == 1)
        {
            camZ = rowsCount / 2;
        }

        mainCamera.transform.position = new Vector3(camX, 30, camZ);
        if ((float)rows * 1.77f >= (float)cols)
        {
            mainCamera.orthographicSize = rows / 2 + 1;
        }
        else
        {
            mainCamera.orthographicSize = Mathf.Round(cols / (1.77f * 2)) + 1;
        }
    }
    private void ColorCell()
    {
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i] == current)
            {
                grid[i].prefab.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
            }
            else if (grid[i].x == cols - 1 && grid[i].z == rows - 1)
            {
                grid[i].prefab.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = endMaterial;
            }
            else if (grid[i].visited == true)
            {
                grid[i].prefab.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = visitedMaterial;
            }
        }
    }
    private void CorrectPath()
    {
        if (isPath == false && current.x == cols - 1 && current.z == rows - 1)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                correctPath.Add(stack[i]);
            }
            isPath = true;
        }
    }
    #endregion
}