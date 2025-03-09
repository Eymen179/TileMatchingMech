using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Bomb : Block
{
    public void Explode()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        List<Block> blocksToDestroy = new List<Block>();

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right,
                                Vector2.one, (-Vector2.one), (Vector2.left + Vector2.up),
                                (Vector2.right + Vector2.down)};
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
