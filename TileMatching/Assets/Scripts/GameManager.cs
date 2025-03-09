using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class GameManager : MonoBehaviour
{
    //Matris deðerleri
    public int gridWidth = 6;
    public int gridHeight = 6;

    //Blok gruplarý için gereken deðerler
    public int minMatch = 2;
    public int A = 3, B = 5, C = 7;

    public float movingDelay = 0.1f;

    //Oyundaki objelerin prefableri
    public GameObject blockPrefab;
    public GameObject bombPrefab;
    public GameObject rocketHorizontalPrefab;
    public GameObject rocketVerticalPrefab;

    //Oyundaki objelerin sprite'larý
    public Sprite[] blockSprites;
    public Sprite bombSprite;
    public Sprite rocketHorizontalSprite;
    public Sprite rocketVerticalSprite;

    public Block[,] grid;//matris

    private BlockSelector blockSelector;

    public Sprite[] blockOfFirstStates;
    public Sprite[] blockOfSecondStates;
    public Sprite[] blockOfThirdStates;

    void Start()
    {
        grid = new Block[gridWidth, gridHeight];
        blockSelector = FindObjectOfType<BlockSelector>();
        InitializeGrid();

    }
    private void Update()
    {
        if(IsDeadlock()) ShuffleBoard();
    }
    void InitializeGrid()//Belirtilen sayý kadar blok oluþturulur.
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                SpawnBlock(x, y, ObjectTypes.Block);
            }
        }
    }
    //Blok oluþturucu metot
    public void SpawnBlock(int x, int y, ObjectTypes objectType)
    {
        ObjectTypes selectedType = objectType;
        switch (selectedType)
        {
            case ObjectTypes.Block:
                GameObject blockObj = Instantiate(blockPrefab, new Vector2(x, y), Quaternion.identity);
                Block block = blockObj.GetComponent<Block>();
                block.Initialize(x, y, blockSprites[Random.Range(0, blockSprites.Length)]);
                grid[x, y] = block;
                break;
            case ObjectTypes.Bomb:
                GameObject bombObj = Instantiate(bombPrefab, new Vector2(x, y), Quaternion.identity);
                Block bomb = bombObj.GetComponent<Block>();
                bomb.Initialize(x, y, bombSprite);
                grid[x, y] = bomb;
                break;
            case ObjectTypes.Rocket_Horizontal:
                GameObject rocketHObj = Instantiate(rocketHorizontalPrefab, new Vector2(x, y), Quaternion.identity);
                Block rocketH = rocketHObj.GetComponent<Block>();
                rocketH.Initialize(x, y, rocketHorizontalSprite);
                grid[x, y] = rocketH;
                break;
            case ObjectTypes.Rocket_Vertical:
                GameObject rocketVObj = Instantiate(rocketVerticalPrefab, new Vector2(x, y), Quaternion.identity);
                Block rocketV = rocketVObj.GetComponent<Block>();
                rocketV.Initialize(x, y, rocketVerticalSprite);
                grid[x, y] = rocketV;
                break;
            default:
                Debug.Log("Obje seçim hatasý");
                break;
        }

    }
    //Blok yok edici metot
    public void DestroyBlocks(Block[] blocks)
    {
        foreach (Block block in blocks)
        {
            grid[block.gridX, block.gridY] = null;
            Destroy(block.gameObject);
        }
        StartCoroutine(DropBlocks());
    }
    //Yok edilen bloklarýn konumlarýna üstteki bloklarý getirten metot
    IEnumerator DropBlocks()
    {
        yield return new WaitForSeconds(0.25f);
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    for (int k = y + 1; k < gridHeight; k++)
                    {

                        if (grid[x, k] != null)
                        {
                            grid[x, k].MoveTo(x, y);
                            yield return new WaitForSeconds(movingDelay);
                            grid[x, y] = grid[x, k];
                            grid[x, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        //Blok hareketiyle boþ kalan kýsýmlara yeni bloklar oluþturulur.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    SpawnBlock(x, y, ObjectTypes.Block);
                }
            }
        }
    }
    private bool IsDeadlock()//Patlatýlabilir blok grup sayýsý kontrolü
    {
        foreach (Block block in grid)
        {
            if (block != null)
            {
                Block[] connectedBlocks = blockSelector.FindConnectedBlocks(block);
                if (connectedBlocks.Length >= minMatch) return false;
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        List<Block> blocksList = new List<Block>();

        foreach (Block block in grid)
        {
            if (block != null)
                blocksList.Add(block);
        }

        Shuffle(blocksList.ToArray()); // Listeyi karýþtýr

        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y].SetSprite(blocksList[index].blockSprite);
                    index++;
                }
            }
        }
    }
    private static void Shuffle<T>(T[] array)//Dizi elemanlarýný rastgele karmaya yarayan algoritma metodu
    {
        int rng = (int)Random.Range(0f, array.Length);
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng;
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
    public enum ObjectTypes
    {
        Block,
        Bomb,
        Rocket_Horizontal,
        Rocket_Vertical
    }
    //-----------------------Çalýþmayan ikon deðiþtirme metodu----------------------------------------------------
    /*
    public void UpdateBlockSrpites()
    {
        HashSet<Block> checkedBlocks = new HashSet<Block>();

        foreach (Block block in grid)
        {
            if (block != null && !checkedBlocks.Contains(block))
            {
                Block[] connectedBlocks = blockSelector.FindConnectedBlocks(block);
                foreach (Block b in connectedBlocks)
                {
                    checkedBlocks.Add(b);
                }
                SpriteChanger(connectedBlocks);
            }
        }

    }
    void SpriteChanger(Block[] blocks)
    {
        int groupSize = blocks.Length;

        Sprite[] selectedSprites = null;
        if (groupSize > A && groupSize <= B) selectedSprites = blockOfFirstStates;
        else if (groupSize > B && groupSize <= C) selectedSprites = blockOfSecondStates;
        else if (groupSize > C) selectedSprites = blockOfThirdStates;
        else selectedSprites = blockSprites;

        if (selectedSprites == null) return;

        foreach (Block block in blocks)
        {
            if(block is not Bomb && block is not HorizontalRocket && block is not VerticalRocket)
            {
                string currentSpriteName = block.blockSprite.name;
                char colorName = currentSpriteName[0];

                // Yeni sprite'ý bul
                foreach (var sprite in selectedSprites)
                {
                    if (sprite.name.StartsWith(colorName)) // Örneðin "Mavi" ile baþlayanlarý bul
                    {
                        block.SetSprite(sprite);
                        break;
                    }
                }
            }

        }
    }*/
    //----------------------------------------------------------------------

}
