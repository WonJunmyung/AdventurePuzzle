using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapControl : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float noiseScale = 5f;
    public float heightMultiplier = 2f;

    public GameObject[] obstacles; // ����, ������ ���� ��ֹ� ������
    public float obstacleSpawnChance = 0.1f;
    MakeTexture makeTexture;
    /// <summary>
    /// �� ���͸���
    /// </summary>
    Material sandMaterial;
    /// <summary>
    /// �� �ؽ���
    /// </summary>
    private Texture2D sandTexture;
    /// <summary>
    /// �ܵ� �ؽ���
    /// </summary>
    private Texture2D glassTexture;

    
    // �� ���� ��ġ
    public Color ColorResponse = new Color(64, 128, 128);

    public Transform Terrain;
    public Texture2D[] MapInfo;
    public float tileSize = 1.0f;
    private int mapWidth;
    private int mapHeight;
    
    /// <summary>
    /// ��������
    /// </summary>
    public GameObject PrefabChest;
    /// <summary>
    /// �÷��̾�
    /// </summary>
    Transform player;
    /// <summary>
    /// ������
    /// </summary>
    List<GameObject> traps = new List<GameObject>();
    /// <summary>
    /// ���ӸŴ���
    /// </summary>
    public GameManager gameManager;
    /// <summary>
    /// ������
    /// </summary>
    public List<GameObject> chests = new List<GameObject>();
    /// <summary>
    /// �����۵�
    /// </summary>
    public GameObject[] prefabItems;
    /// <summary>
    /// �κ�, ������ ����
    /// </summary>
    public GameObject prefabDoor;
    /// <summary>
    /// ��ŸƮ����
    /// </summary>
    public Vector3 startPos;
    
    
    


    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
    }

    public void CreateMap()
    {

        // ���� �Ŵ����� �˻��ؼ� �־��ش�.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // �÷��̾ ã�Ƽ� �־��ش�.
        player = GameObject.Find("Player").transform;
        // ��������
        PrefabChest = Resources.Load<GameObject>("Prefabs/Chest_Closed");
        // ������
        prefabItems = Resources.LoadAll<GameObject>("Prefabs/Items");
        // ��
        prefabDoor = Resources.Load<GameObject>("Prefabs/Door");
        // �� �ؽ���
        sandTexture = GenerateTexture(GameData.baseSandColor, GameData.mixSandColor);
        // �ܵ� �ؽ���
        glassTexture = GenerateTexture(GameData.baseGlassColor, GameData.mixGlassColor);
        // �� ����
        MapInfo = Resources.LoadAll<Texture2D>("MapData");
       
        GenerateDesertTerrain();
        if (!GameData.Instance.isGame)
        {

            GameData.Instance.stageNum = 0;
            GameData.Instance.bagUpgradeNum = 0;
            GameData.Instance.bagData.Clear();
            GameData.Instance.chestData.Clear();
            foreach(GameObject obj in chests)
            {
                // ���� �ȵ�
                GameData.Instance.chestData.Add(true);
            }
        }
        else
        {
            for(int i=0;i< GameData.Instance.chestData.Count;i++)
            {
                // ������ ������ ���α�
                chests[i].SetActive(GameData.Instance.chestData[i]);
            }
        }
        // �������� ����
        gameManager.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
        GameData.Instance.isGame = true;
    }

    

    // Update is called once per frame
    void Update()
    {
        LimitPlayerMove();
    }

    void LimitPlayerMove()
    {
        if (player == null) return;
        
        float minX = 0;
        float maxX = mapWidth;
        float minZ = 0;
        float maxZ = mapHeight;

        Vector3 clampedPosition = player.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minZ, maxZ);
        player.position = clampedPosition;
    }

    

    /// <summary>
    /// ������ �����ؼ� �ؽ��ĸ� ����
    /// </summary>
    /// <param name="baseColor"></param>
    /// <param name="mixColor"></param>
    /// <returns></returns>
    Texture2D GenerateTexture(Color baseColor, Color mixColor)
    {
        // �ؽ���(��ȭ�� ����)
        Texture2D texture = new Texture2D(GameData.textureWidth, GameData.textureHeight);
        
        for (int x = 0; x < GameData.textureWidth; x++)
        {
            for (int y = 0; y < GameData.textureHeight; y++)
            {
                // 0 ~ 1 ������ ���� ����, ������ ����
                float noise = Mathf.PerlinNoise(x / noiseScale, y / noiseScale);
                // �⺻ ���򿡼� �ͽ� �÷��� ���� ����
                Color pixelColor = Color.Lerp(baseColor, mixColor, noise);
                // �ؽ��Ŀ� ���� ĥ�ϱ�
                texture.SetPixel(x, y, pixelColor);
            }
        }
        // �ؽ��� ����
        texture.Apply();
        return texture;
    }


    void GenerateDesertTerrain()
    {
        GameObject Map = new GameObject("Map");
        mapWidth = MapInfo[GameData.Instance.stageNum].width;   
        mapHeight = MapInfo[GameData.Instance.stageNum].height;
        GameData.Instance.stageSize = mapWidth;
        Color[] pixels = MapInfo[GameData.Instance.stageNum].GetPixels();

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                Color pixelColor = pixels[i * mapHeight + j];       // 0
                // �ٴ�
                if (pixelColor == GameData.ColorNoneSandBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;
                }
                else if(pixelColor == Color.cyan)
                {
                    GeneratePyramid(new Vector3(j * tileSize, -1f, i * tileSize));
                }
                // �ܵ�
                else if(pixelColor == GameData.ColorNoneGlassBlock)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = glassTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.tag = "Ground";
                    bottomBlock.transform.parent = Map.transform;
                }// ��������
                else if (pixelColor == GameData.ColorTreasure)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject chest = Instantiate(PrefabChest, new Vector3(j * tileSize, -1f, i * tileSize), PrefabChest.transform.rotation);
                    chest.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    chest.tag = "Chest";
                    chests.Add(chest);
                }
                else if (pixelColor == GameData.ColorBlock)   // ��ֹ�
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject Block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    Block.transform.position = new Vector3(j * tileSize, 0f, i * tileSize);
                    Block.transform.parent = Map.transform;
                    Block.tag = "Obstacle";
                }
                else if(pixelColor == GameData.ColorTrap) // Ʈ��
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject trap = GenerateTraps();
                    trap.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    trap.transform.parent = Map.transform;
                }
                else if(pixelColor == GameData.ColorPatrolEnemyTop)
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    gameManager.monsterControl.CreateMonster(new Vector3(j * tileSize, 0f, i * tileSize));
                }
                else if(pixelColor == GameData.colorRandom) // �������� �����Ǵ� ������
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    int randNum = Random.Range(0, prefabItems.Length);
                    GameObject item = Instantiate(prefabItems[randNum], new Vector3(j * tileSize, 0f, i * tileSize), prefabItems[randNum].transform.rotation);
                    item.transform.position = new Vector3(j * tileSize, 0.0f, i * tileSize);
                    item.GetComponent<Item>().SetItem(randNum);
                    item.tag = "Item";
                }
                else if (pixelColor == GameData.colorRobby) // �κ� ����Ʈ
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject robby = Instantiate(prefabDoor, new Vector3(j * tileSize, -1f, i * tileSize), prefabDoor.transform.rotation);
                    robby.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    robby.tag = "Start";
                    
                }
                else if (pixelColor == GameData.colorEND) // ������ ���� ���
                {
                    GameObject bottomBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bottomBlock.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    bottomBlock.transform.position = new Vector3(j * tileSize, -1f, i * tileSize);
                    bottomBlock.transform.parent = Map.transform;

                    GameObject robby = Instantiate(prefabDoor, new Vector3(j * tileSize, -1f, i * tileSize), prefabDoor.transform.rotation);
                    robby.transform.position = new Vector3(j * tileSize, -0.5f, i * tileSize);
                    robby.tag = "End";
                }
            }
        }
        
    }

    GameObject GenerateTraps()
    {
        GameObject trap = new GameObject("trap");
        for (int i = 0; i < 5; i++)
        {
            GameObject thorn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thorn.transform.position = new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));
            thorn.transform.localScale = new Vector3(0.1f, 1.1f, 0.1f);
            thorn.GetComponent<Renderer>().material.color = Color.white;
            thorn.transform.parent = trap.transform;
            Destroy(thorn.GetComponent<BoxCollider>());
        }
        trap.AddComponent<Trap>();
        traps.Add(trap);
        
        return trap;
    }

    void GeneratePyramid(Vector3 pos)
    {
        GameObject pyramid = new GameObject("Pyramid");
        int baseSize = 8;
        float blockSize = 1f;

        for (int y = 0; y < baseSize; y++)
        {
            for (int x = 0; x < baseSize - y; x++)
            {
                for (int z = 0; z < baseSize - y; z++)
                {
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.transform.position = new Vector3(x - (baseSize - y) / 2f, y * blockSize, z - (baseSize - y) / 2f);
                    block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
                    block.GetComponent<Renderer>().material.mainTexture = sandTexture;
                    block.transform.parent = pyramid.transform;
                }
            }
        }

        GameObject entrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entrance.transform.position = new Vector3(0, 0.5f, -2);
        entrance.transform.localScale = new Vector3(1.5f, 1, 0.2f);

        Texture2D entranceTexture = new Texture2D(128, 128);
        Color baseColor = new Color(0.2f, 0.1f, 0.05f);
        Color xColor = Color.red;
        for (int y = 0; y < entranceTexture.height; y++)
        {
            for (int x = 0; x < entranceTexture.width; x++)
            {
                if (Mathf.Abs(x - y) < 5 || Mathf.Abs(x + y - 128) < 5) // X�� �׸���
                    entranceTexture.SetPixel(x, y, xColor);
                else
                    entranceTexture.SetPixel(x, y, baseColor);
            }
        }
        entranceTexture.Apply();
        

        Material entranceMaterial = new Material(Shader.Find("Standard"));
        entranceMaterial.mainTexture = entranceTexture;
        entranceMaterial.mainTexture = null;
        entranceMaterial.color = new Color(0.2f, 0.1f, 0.05f);
        entrance.GetComponent<Renderer>().material = entranceMaterial;


        pyramid.transform.position = pos;
    }


    
}
