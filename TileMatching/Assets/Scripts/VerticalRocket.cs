using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalRocket : Block
{
    public void Fire()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        List<Block> blocksToDestroy = new List<Block>();
        List<Vector2> directions = new List<Vector2>();
        for (int i = 0; i < gameManager.gridHeight; i++)
        {
            directions.Add(Vector2.up * i);
            directions.Add(Vector2.down * i);
        }
        blocksToDestroy.Add(this);

        foreach (var dir in directions)
        {
            int newX = this.gridX + (int)dir.x;
            int newY = this.gridY + (int)dir.y;

            //Yeni koordinatlar ýzgara sýnýrlarýný geçmiyorsa ve bu koordinatlarda bir blok varsa o bloðu listeye ekle.
            if (newX >= 0 && newX < gameManager.gridWidth && newY >= 0 && newY < gameManager.gridHeight)
            {
                if (gameManager.grid[newX, newY] != null)
                {
                    blocksToDestroy.Add(gameManager.grid[newX, newY]);
                }
            }
        }
        gameManager.DestroyBlocks(blocksToDestroy.ToArray());

    }
}
