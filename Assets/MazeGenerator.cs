using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell
{

    

    public Cell(Vector2 pos, bool visited, float colorAmount)
    {
        this.Position = pos;
        this.Visited = visited;
        this.ColorAmount = colorAmount;

        this.hasdownWall = true;
        this.hasLeftWall = true;
        this.hasrightWall = true;
        this.hasupWall = true;

        this.isEndTile = false;
    }

    public Vector2 Position { get; set; }
    public bool Visited { get; set; }
    public float ColorAmount { get; set; }

    public bool hasLeftWall { get; set; }
    public bool hasrightWall { get; set; }
    public bool hasupWall { get; set; }
    public bool hasdownWall { get; set; }


    public bool isEndTile { get; set; }



}

public class MazeGenerator : MonoBehaviour {

    List<Cell> cells = new List<Cell>();
    Stack<Cell> pile = new Stack<Cell>();

    int offsetX = -10, offsetY = -10;

    public GameObject cellPrefab;
    public GameObject wallPrefab;
    int visitedCell;
    Cell currentPos;
    float redAmount = 0;

    int mazeSize = 40;

    public Slider sliderX;
    public Slider sliderY;

    public int mazeSizeX, mazeSizeY;

    GameObject parent;
    GameObject wParent;

    


	// Use this for initialization
	void Start () {


        GenerateNewMaze();



	}

    public void GenerateNewMaze()
    {
        Destroy(parent);
        Destroy(wParent);

        parent = new GameObject("parent maze");
        wParent = new GameObject("parent wall");

        pile.Clear();
        cells.Clear();
        visitedCell = 0;

        mazeSizeX = (int)sliderX.value;
        mazeSizeY = (int)sliderY.value;

        initCells(mazeSizeX, mazeSizeY);

        currentPos = getCellsAtPos(0, 0);
        pile.Push(currentPos);
        currentPos.Visited = true;
        visitedCell++;

        GenerateMaze();

        offsetX = - (int) Mathf.Floor(mazeSizeX / 2);
        offsetY = - (int) Mathf.Floor(mazeSizeY / 2);

        renderCells(parent, wParent);


    }


    void initCells(int sizeX, int sizeY)
    {

        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {

                cells.Add(new Cell(new Vector2(i,j), false, 0));

            }
        }

    }

    void renderCells(GameObject parent, GameObject wParent)
    {
        foreach(Cell cellpos in cells)
        {

            GameObject g = Instantiate(cellPrefab, new Vector2(cellpos.Position.x + offsetX, cellpos.Position.y + offsetY), Quaternion.identity);

            generateWall(cellpos, wParent);

            if (cellpos.Visited)
                g.GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0) ;
            else
                g.GetComponent<Renderer>().material.color = Color.blue;

            

            g.transform.parent = parent.transform;

        }

        
    }

    Cell getCellsAtPos(float x, float y)
    {

        Vector2 pos = new Vector2(x, y);

        foreach(Cell cell in cells)
        {
            if (cell.Position == pos)
                return cell;
        }

        return null;

    }

    void GenerateMaze()
    {
        /* On prends la cellule ou on est
         * On regarde les neighbor
         * On en choisis un au hasard qui n'est pas visité
         * On push sa position dans le stack
         * On set la current position
         * On recommence
         */

        //On crée une liste de voisins

        

        if (!pile.Peek().Position.Equals(currentPos.Position))
        {
            pile.Push(currentPos);
            currentPos.Visited = true;
            visitedCell++;
            currentPos.ColorAmount = visitedCell;
        }
        
        List<Cell> neighbors = getNeighbors(currentPos);
        

        //On enlève de la liste les voisins déjà visité
        /*for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].Visited)
                neighbors.RemoveAt(i);
        }*/

        if (neighbors.Count == 0)
        {
            // Si il n'y a pas de neighbor on pop le stack et la current pos est la derniere du stack
            pile.Pop();
            currentPos = pile.Peek();
        }
        else
        {
            //on choisis un random int entre 0 et le count de la liste - 1
            int neighborToChose = Random.Range(0, neighbors.Count);


            removeWall(currentPos, neighbors[neighborToChose]);
            //la nouvelle current pos est cette cellule
            currentPos = neighbors[neighborToChose];
            
        }

        if(visitedCell < (mazeSizeX * mazeSizeY))
        {
            GenerateMaze();
        }
        else
        {
            Debug.Log("Maze Généré " + visitedCell);
            pile.Peek().isEndTile = true;
            
        }

    }

    List<Cell> getNeighbors(Cell cell)
    {
        List<Cell> cells = new List<Cell>();

        if(cell.Position.x > 0)
        {
            if(getCellsAtPos(cell.Position.x - 1, cell.Position.y).Visited == false)
                cells.Add(getCellsAtPos(cell.Position.x -1, cell.Position.y));
        }

        if(cell.Position.x < mazeSizeX -1)
        {
            if (getCellsAtPos(cell.Position.x + 1, cell.Position.y).Visited == false)
                cells.Add(getCellsAtPos(cell.Position.x + 1, cell.Position.y));
        }

        if (cell.Position.y > 0)
        {
            if (getCellsAtPos(cell.Position.x, cell.Position.y - 1).Visited == false)
                cells.Add(getCellsAtPos(cell.Position.x, cell.Position.y - 1));
        }

        if (cell.Position.y < mazeSizeY - 1)
        {
            if (getCellsAtPos(cell.Position.x, cell.Position.y + 1).Visited == false)
                cells.Add(getCellsAtPos(cell.Position.x, cell.Position.y + 1));
        }

        return cells;
        
    }


    void generateWall(Cell cell, GameObject parent)
    {

        Vector2 posCentered = new Vector2(cell.Position.x + offsetX, cell.Position.y + offsetY);

        //Left wall
        if (cell.hasLeftWall)
        {
            GameObject g = Instantiate(wallPrefab, new Vector3(posCentered.x - 0.5f, posCentered.y, -1), Quaternion.identity);
            g.transform.parent = parent.transform;
        }


        //Right wall
        if (cell.hasrightWall)
        {
            GameObject g = Instantiate(wallPrefab, new Vector3(posCentered.x + 0.5f, posCentered.y, -1), Quaternion.identity);
            g.transform.parent = parent.transform;
        }

        //Down wall
        if (cell.hasdownWall)
        {
            GameObject g = Instantiate(wallPrefab, new Vector3(posCentered.x, posCentered.y - 0.5f, -1), Quaternion.Euler(new Vector3(0, 0, 90)));
            g.transform.parent = parent.transform;
        }

        //Up wall
        if (cell.hasupWall)
        {
            GameObject g = Instantiate(wallPrefab, new Vector3(posCentered.x, posCentered.y + 0.5f, -1), Quaternion.Euler(new Vector3(0, 0, 90)));
            g.transform.parent = parent.transform;
        }

    }

    void removeWall(Cell currentCell, Cell neighborCell)
    {
        //Dans quelle direction est la cellule voisine

        //cellule à droite
        if(neighborCell.Position.x  == currentCell.Position.x + 1)
        {
            neighborCell.hasLeftWall = false;
            currentCell.hasrightWall = false;
        }

        //cellule à gauche
        if (neighborCell.Position.x == currentCell.Position.x - 1)
        {
            neighborCell.hasrightWall = false;
            currentCell.hasLeftWall = false;
        }

        //Cellule en haut
        if (neighborCell.Position.y == currentCell.Position.y + 1)
        {
            neighborCell.hasdownWall = false;
            currentCell.hasupWall = false;
        }

        //Cellule en bas
        if (neighborCell.Position.y == currentCell.Position.y - 1)
        {
            neighborCell.hasupWall = false;
            currentCell.hasdownWall = false;
        }


    }
}
