using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    private GameManager gameManager;


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); //GridManager metotlar�n� kullanmak i�in 
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Mouse'un t�klad��� yerin konumu al�n�r.
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

            //T�klan�lan yerde bir blok varsa o blo�un biti�i�indeki ayn� renkli bloklar bulunur.
            if (hit.collider != null && hit.collider.CompareTag("Block"))
            {
                Block clickedBlock = hit.collider.GetComponent<Block>();
                if (clickedBlock != null)
                {
                    if (clickedBlock is Bomb) ((Bomb)clickedBlock).Explode();
                    else if (clickedBlock is HorizontalRocket) ((HorizontalRocket)clickedBlock).Fire();
                    else if (clickedBlock is VerticalRocket) ((VerticalRocket)clickedBlock).Fire();
                    else
                    {
                        Block[] connectedBlocks = FindConnectedBlocks(clickedBlock);

                        if (connectedBlocks.Length >= gameManager.minMatch)
                        {
                            gameManager.DestroyBlocks(connectedBlocks);//Se�ili bloklar yok edilir.
                        }
                        if (connectedBlocks.Length > gameManager.A && connectedBlocks.Length <= gameManager.B)
                        {
                            gameManager.SpawnBlock(clickedBlock.gridX, clickedBlock.gridY, GameManager.ObjectTypes.Bomb);
                        }
                        else if (connectedBlocks.Length > gameManager.B && connectedBlocks.Length <= gameManager.C)
                        {
                            gameManager.SpawnBlock(clickedBlock.gridX, clickedBlock.gridY, GameManager.ObjectTypes.Rocket_Vertical);
                        }
                        else if (connectedBlocks.Length > gameManager.C)
                        {
                            gameManager.SpawnBlock(clickedBlock.gridX, clickedBlock.gridY, GameManager.ObjectTypes.Rocket_Horizontal);
                        }
                    }
                }
            }
        }
    }
    public Block[] FindConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();
        Queue<Block> toCheck = new Queue<Block>();
        HashSet<Block> checkedBlocks = new HashSet<Block>();

        toCheck.Enqueue(startBlock);

        while (toCheck.Count > 0)//Kuyru�a konacak spesifik bir blok kalmayana kadar �al��an bir d�ng�
        {
            Block current = toCheck.Dequeue();
            if (!checkedBlocks.Contains(current))
            {
                connectedBlocks.Add(current);
                checkedBlocks.Add(current);

                foreach (Block neighbor in GetNeighbors(current))
                {
                    if (neighbor.blockSprite == startBlock.blockSprite && !checkedBlocks.Contains(neighbor))
                    {
                        toCheck.Enqueue(neighbor);
                    }
                }
            }
        }
        return connectedBlocks.ToArray();
    }
    List<Block> GetNeighbors(Block block)
    {
        List<Block> neighborblocks = new List<Block>();

        //Se�ili blo�un �evre koordinatlar�na bak�l�r.
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var dir in directions)
        {
            int newX = block.gridX + (int)dir.x;
            int newY = block.gridY + (int)dir.y;

            //Yeni koordinatlar �zgara s�n�rlar�n� ge�miyorsa ve bu koordinatlarda bir blok varsa o blo�u listeye ekle.
            if (newX >= 0 && newX < gameManager.gridWidth && newY >= 0 && newY < gameManager.gridHeight)
            {
                if (gameManager.grid[newX, newY] != null)
                {
                    neighborblocks.Add(gameManager.grid[newX, newY]);//de�i�iklik
                }
            }
        }
        return neighborblocks;
    }
}
