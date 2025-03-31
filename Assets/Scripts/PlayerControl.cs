using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    /// <summary>
    /// �̵��ӵ�
    /// </summary>
    public float moveSpeed = 5.0f;
    /// <summary>
    /// ���� �ӵ� ���
    /// </summary>
    public float originalSpeed;
    /// <summary>
    /// ���ǵ�� ����
    /// </summary>
    public float speedTimer = 5.0f;
    public float speedTiming = 0;
    public bool isSpeedUp = false;
    /// <summary>
    /// ���� ���ǵ�� ����
    /// </summary>
    public float dbSpeedTimer = 5.0f;
    public float dbSpeedTiming = 0;
    public bool isDbSpeedUp = false;
    /// <summary>
    /// �ν� �Ұ�
    /// </summary>
    public float recogTimer = 5.0f;
    public float recogTiming = 0;
    public bool isRecog = false;
    /// <summary>
    /// ���� ȭ��ǥ
    /// </summary>
    //public float arrowTimer = 5.0f;
    //public float arrowTiming = 0;
    //public bool isArrow = true;
    public GameObject ChestArrow;
    public GameObject tempChest;
    /// <summary>
    /// ȸ���ӵ�
    /// </summary>
    public float rotSpeed = 2.0f;
    /// <summary>
    /// ī�޶�
    /// </summary>
    public Transform trCamera;
    /// <summary>
    /// ĳ���� ��Ʈ�ѷ�
    /// </summary>
    private CharacterController characterController;
    /// <summary>
    /// ī�޶��� ���� ȸ��
    /// </summary>
    private float rotationX = 0f;
    /// <summary>
    /// 
    /// </summary>
    private float rotationY = 0f;
    /// <summary>
    /// �ٰŸ�, ���Ÿ� ���� ����
    /// </summary>
    public AttackType attackType = AttackType.Melee;
    /// <summary>
    /// ������ ������ ��������
    /// </summary>
    private GameObject prefabKnife;
    /// <summary>
    /// ������ ������Ʈ
    /// </summary>
    private GameObject knife;
    /// <summary>
    /// ������ ���� ��ġ
    /// </summary>
    private Vector3 knifeOriginalPos;
    /// <summary>
    /// ������ ���� ȸ��
    /// </summary>
    private Quaternion knifeOriginalRot;
    /// <summary>
    /// ��ҷ�
    /// </summary>
    public int O2 = 60;
    /// <summary>
    /// ü�·�
    /// </summary>
    public int HP = 10;
    /// <summary>
    /// ���ӸŴ���
    /// </summary>
    public GameManager gameManager;
    /// <summary>
    /// �κ��丮 ��Ʈ�ѷ�
    /// </summary>
    public InventoryControl inventoryControl;
    /// <summary>
    /// // ��ҷ� ����
    /// </summary>
    public float O2Timer = 10;
    public float O2Timing = 0;
    public bool isO2 = true;
    

    public void SetPlayer()
    {
        originalSpeed = moveSpeed;
        // ���� �Ŵ����� �˻��ؼ� �־��ش�.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // �������� �ѹ��� ���� ���� ��ġ ����
        this.transform.position = GameData.StagePos[GameData.Instance.stageNum];
        // ī�޶� �̸����� �˻��ؼ� ����ֱ�
        trCamera = GameObject.Find("Main Camera").transform;
        // ü��Ʈ �˷��ִ� ȭ��ǥ
        ChestArrow = Resources.Load<GameObject>("Prefabs/ChestArrow");
        // ���� ĳ���Ϳ� ĳ���� ��Ʈ�� �߰�
        characterController = this.AddComponent<CharacterController>();
        // ���� ĳ���Ϳ� �κ��丮 ��Ʈ�� �߰�
        inventoryControl = this.AddComponent<InventoryControl>();
        // ������ �� ����
        CreateKnife();
    }

    void Move()
    {
        // w,s,a,d ��ȭ ��
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        // �̵��� ����
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        // ĳ���� ��Ʈ�ѷ� Move�� ���� �̵�
        characterController.Move(move * moveSpeed * Time.deltaTime);
        trCamera.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    void LookAround()
    {
        // ���콺 ��ȭ ��
        float mouseX = Input.GetAxis("Mouse X") * rotSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotSpeed;
        // ���콺�� ���� ����
        rotationX -= mouseY;
        // ī�޶� 90�� �̻� �������� �ʵ��� ����
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        // ���콺�� ����
        rotationY += mouseX;
        // ĳ���� ȸ��
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        //transform.Rotate(Vector3.up * mouseX);
        // ī�޶� ȸ��
        trCamera.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        // �ʱ� ĳ���� ����
        SetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameData.Instance.isMouse)
        {
            Move();
            LookAround();
            Attack();
            SetO2();
            if (isSpeedUp)
            {
                if (speedTiming > speedTimer)
                {
                    isSpeedUp = false;
                    moveSpeed = originalSpeed;
                    speedTiming = 0;
                }
                else
                {
                    speedTiming += Time.deltaTime;
                }
            }
            if (isDbSpeedUp)
            {
                if (dbSpeedTiming > dbSpeedTimer)
                {
                    isDbSpeedUp = false;
                    moveSpeed = originalSpeed;
                    dbSpeedTiming = 0;
                }
                else
                {
                    dbSpeedTiming += Time.deltaTime;
                }
            }
            if (isRecog)
            {
                if(recogTiming > recogTimer)
                {
                    isRecog = false;
                    recogTiming = 0;
                }
                else
                {
                    recogTiming += Time.deltaTime;
                }
            }
            GameData.Instance.gameTime += Time.deltaTime;
            gameManager.totalTime.text = GameData.Instance.gameTime.ToString("#");
        }


        #region ġƮ
        if (Input.GetKeyDown(KeyCode.F1))
        {
            HP = GameData.MaxHp;
            O2 = GameData.MaxO2[GameData.Instance.bagO2];
        }
        // �������� ���
        //if (Input.GetKeyDown(KeyCode.F2))
        //{

        //}
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SetGameInit();
            SceneManager.LoadScene("Game");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            int num = SceneManager.sceneCount - 2;
            if(num <= GameData.maxStageNum)
            {
                SceneManager.LoadScene(SceneManager.sceneCount + 1);
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if(Time.timeScale < 1)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
            
        }
        #endregion
    }

    public void SetGameInit()
    {
        GameData.Instance.isGame = false;
        GameData.Instance.gameTime = 0;
        GameData.Instance.bagData.Clear();
        GameData.Instance.bagO2 = 0;
        GameData.Instance.bagUpgradeNum = 0;
        GameData.Instance.cost = 0;
        
        GameData.Instance.stageNum = 0;
        GameData.Instance.bagUpgradeNum = 0;
        GameData.Instance.bagData.Clear();
        GameData.Instance.chestData.Clear();

        for(int i = 0; i < GameData.Instance.isBuyItem.Length; i++)
        {
            GameData.Instance.isBuyItem[i] = true;
        }
        GameData.Instance.stageNum = 0;
        
    }


    public Transform FindChecst()
    {
        float distance = 100000;
        Transform returnTr = null;
        for(int i = 0;i< gameManager.mapControl.chests.Count; i++)
        {
            if (gameManager.mapControl.chests[i].activeInHierarchy)
            {
                float tempDistance = Vector3.Distance(this.transform.position, gameManager.mapControl.chests[i].transform.position);
                if (distance > tempDistance)
                {
                    distance = tempDistance;
                    returnTr = gameManager.mapControl.chests[i].transform;
                }
            }
        }
        return returnTr;
    }


    /// <summary>
    /// ���� ����(���Ÿ� ������ص� �ȴٸ� ������ ���)
    /// </summary>
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            if (attackType == AttackType.Melee)
            {
                DetectObjectInFront();
                StartCoroutine(AnimateKnifeAttack());

            }
            //else if (attackType == AttackType.Ranged)
            //{

            //}
        }
    }

    void CreateKnife()
    {
        prefabKnife = Resources.Load<GameObject>("Prefabs/Knife");
        knife = Instantiate(prefabKnife);
        
        knife.transform.SetParent(trCamera);
        knife.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
        knifeOriginalPos = knife.transform.localPosition;
        knifeOriginalRot = knife.transform.localRotation;
    }

    /// <summary>
    /// ������ ���� �ִϸ��̼�
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateKnifeAttack()
    {
        Vector3 attackPos = knifeOriginalPos + new Vector3(-0.2f, 0, 0.2f);
        Quaternion attackRot = Quaternion.Euler(knifeOriginalRot.eulerAngles + new Vector3(0, -20, 0));

        float attackTime = 0.1f;
        float elapsedTime = 0f;
        while (elapsedTime < attackTime)
        {
            knife.transform.localPosition = Vector3.Lerp(knifeOriginalPos, attackPos, elapsedTime / attackTime);
            knife.transform.localRotation = Quaternion.Lerp(knifeOriginalRot, attackRot, elapsedTime / attackTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < attackTime)
        {
            knife.transform.localPosition = Vector3.Lerp(attackPos, knifeOriginalPos, elapsedTime / attackTime);
            knife.transform.localRotation = Quaternion.Lerp(attackRot, knifeOriginalRot, elapsedTime / attackTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void DetectObjectInFront()
    {
        RaycastHit hit;
        Vector3 forward = this.transform.forward;
        Vector3 rayOrigin = this.transform.position;
        // 1.5f �Ÿ� ���� ��ü ����
        if (Physics.BoxCast(rayOrigin, new Vector3(0.5f,1.0f,0.5f), forward, out hit, Quaternion.identity,1.0f)) 
        {
            Debug.Log("Detected Object: " + hit.collider.gameObject.name);
            
            
            if (hit.collider.gameObject.CompareTag("Chest"))
            {
                int num = gameManager.mapControl.chests.FindIndex(a => a == hit.collider.gameObject);
                if (num > -1)
                {
                    GameData.Instance.chestData[num] = false;
                    GameData.Instance.cost += GameData.chestCost[GameData.Instance.stageNum];
                    hit.collider.gameObject.SetActive(false);
                }
                gameManager.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
            }
            else if (hit.collider.gameObject.CompareTag("Obstacle"))
            {
                hit.collider.gameObject.SetActive(false);
            }
            else if (hit.collider.gameObject.CompareTag("Item"))
            {
                if (inventoryControl.GetInventoryEmpty(hit.collider.GetComponent<Item>()))
                {
                    hit.collider.gameObject.SetActive(false);
                }
            }
            else if(hit.collider.gameObject.CompareTag("Start"))
            {
                SceneManager.LoadScene("Robby");
                gameManager.GameModeChange();
            }
            else if (hit.collider.gameObject.CompareTag("End"))
            {
                if(GameData.Instance.chestData.FindAll(a => a).Count <= 0)
                {
                    gameManager.GameModeChange();
                    gameManager.gameCard.gameObject.SetActive(true);
                }
                else
                {
                    gameManager.delayText.SetText("������ �� ã�ƾ� �̵� �����մϴ�.");
                }
            }
        }
    }
    public void UseItem(int itemNum)
    {
        Debug.Log("ItemNum" + itemNum + " ���");
        // { "FIND", "HP", "O2", "NOTRECOG", "DOUBLEFAST", "FAST", };
        switch (itemNum)
        {
            case 0:
                {
                    Vector3 pos = FindChecst().position;
                    tempChest = Instantiate(ChestArrow, pos, ChestArrow.transform.rotation);
                }
                break;
            case 1:
                {
                    HP = GameData.MaxHp;
                    gameManager.HPBar.value = (float)HP / (float)GameData.MaxHp;
                }
                break;
            case 2:
                {
                    O2 = GameData.MaxO2[GameData.Instance.bagO2];
                    gameManager.O2Bar.value = (float)O2 / (float)GameData.MaxO2[GameData.Instance.bagO2];
                }
                break;
            case 3:
                {
                    isRecog = true;
                    recogTiming = 0;
                }
                break;
            case 4:
                {
                    isDbSpeedUp = true;
                    moveSpeed = originalSpeed * 1.4f;
                    speedTiming = 0;
                }
                break;
            case 5:
                {
                    isSpeedUp = true;
                    moveSpeed = originalSpeed * 1.2f;
                    speedTiming = 0;
                }
                break;
            
            
            
        }
    }

    public void SetDamage(int damage)
    {
        if (!isRecog)
        {
            HP -= damage;
            Debug.Log("ü�� ����");
            if (HP <= 0)
            {
                gameManager.HPBar.value = 0;
                gameManager.SetGameClear(false);

            }
            else
            {
                gameManager.HPBar.value = (float)HP / (float)GameData.MaxHp;
            }
        }
    }  

    public void SetO2()
    {
        if (isO2)
        {
            if (O2Timing > O2Timer)
            {
                O2Timing = 0;
                O2 -= 1;
                if (O2 < 0)
                {
                    gameManager.O2Bar.value = 0;
                    gameManager.SetGameClear(false);
                }
                else
                {
                    gameManager.O2Bar.value = (float)O2 / (float)GameData.MaxO2[GameData.Instance.bagO2];
                }
            }
            else
            {
                O2Timing += Time.deltaTime;
            }
            
        }
    }
}
