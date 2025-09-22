using UnityEngine;

public class Board : MonoBehaviour
{
    public int row = 4;
    public int column = 4;
    [SerializeField] private GameObject tilePrefab;
    
    private BackgroundTile [,] allTiles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allTiles = new BackgroundTile[row, column];
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Setup()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(j, -i, 0), Quaternion.identity);
                tile.transform.parent = this.transform;
                tile.name = "Tile " + i + " " + j;
            }
        }
    }
}
