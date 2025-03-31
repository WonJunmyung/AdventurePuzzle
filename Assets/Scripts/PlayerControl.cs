using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    /// <summary>
    /// 이동속도
    /// </summary>
    public float moveSpeed = 5.0f;
    /// <summary>
    /// 원본 속도 기록
    /// </summary>
    public float originalSpeed;
    /// <summary>
    /// 스피드업 관련
    /// </summary>
    public float speedTimer = 5.0f;
    public float speedTiming = 0;
    public bool isSpeedUp = false;
    /// <summary>
    /// 더블 스피드업 관련
    /// </summary>
    public float dbSpeedTimer = 5.0f;
    public float dbSpeedTiming = 0;
    public bool isDbSpeedUp = false;
    /// <summary>
    /// 인식 불가
    /// </summary>
    public float recogTimer = 5.0f;
    public float recogTiming = 0;
    public bool isRecog = false;
    /// <summary>
    /// 보물 화살표
    /// </summary>
    //public float arrowTimer = 5.0f;
    //public float arrowTiming = 0;
    //public bool isArrow = true;
    public GameObject ChestArrow;
    public GameObject tempChest;
    /// <summary>
    /// 회전속도
    /// </summary>
    public float rotSpeed = 2.0f;
    /// <summary>
    /// 카메라
    /// </summary>
    public Transform trCamera;
    /// <summary>
    /// 캐릭터 컨트롤러
    /// </summary>
    private CharacterController characterController;
    /// <summary>
    /// 카메라의 상하 회전
    /// </summary>
    private float rotationX = 0f;
    /// <summary>
    /// 
    /// </summary>
    private float rotationY = 0f;
    /// <summary>
    /// 근거리, 원거리 공격 가능
    /// </summary>
    public AttackType attackType = AttackType.Melee;
    /// <summary>
    /// 나이프 프리팹 가져오기
    /// </summary>
    private GameObject prefabKnife;
    /// <summary>
    /// 나이프 오브젝트
    /// </summary>
    private GameObject knife;
    /// <summary>
    /// 나이프 생성 위치
    /// </summary>
    private Vector3 knifeOriginalPos;
    /// <summary>
    /// 나이프 생성 회전
    /// </summary>
    private Quaternion knifeOriginalRot;
    /// <summary>
    /// 산소량
    /// </summary>
    public int O2 = 60;
    /// <summary>
    /// 체력량
    /// </summary>
    public int HP = 10;
    /// <summary>
    /// 게임매니저
    /// </summary>
    public GameManager gameManager;
    /// <summary>
    /// 인벤토리 컨트롤러
    /// </summary>
    public InventoryControl inventoryControl;
    /// <summary>
    /// // 산소량 감소
    /// </summary>
    public float O2Timer = 10;
    public float O2Timing = 0;
    public bool isO2 = true;
    

    public void SetPlayer()
    {
        originalSpeed = moveSpeed;
        // 게임 매니저를 검색해서 넣어준다.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // 스테이지 넘버에 따라 시작 위치 변경
        this.transform.position = GameData.StagePos[GameData.Instance.stageNum];
        // 카메라 이름으로 검색해서 집어넣기
        trCamera = GameObject.Find("Main Camera").transform;
        // 체스트 알려주는 화살표
        ChestArrow = Resources.Load<GameObject>("Prefabs/ChestArrow");
        // 현재 캐릭터에 캐릭터 컨트롤 추가
        characterController = this.AddComponent<CharacterController>();
        // 현재 캐릭터에 인벤토리 컨트롤 추가
        inventoryControl = this.AddComponent<InventoryControl>();
        // 나이프 모델 생성
        CreateKnife();
    }

    void Move()
    {
        // w,s,a,d 변화 값
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        // 이동할 방향
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        // 캐릭터 컨트롤러 Move를 통한 이동
        characterController.Move(move * moveSpeed * Time.deltaTime);
        trCamera.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    void LookAround()
    {
        // 마우스 변화 값
        float mouseX = Input.GetAxis("Mouse X") * rotSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotSpeed;
        // 마우스의 상하 적용
        rotationX -= mouseY;
        // 카메라가 90도 이상 뒤집히지 않도록 설정
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        // 마우스의 상하
        rotationY += mouseX;
        // 캐릭터 회전
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        //transform.Rotate(Vector3.up * mouseX);
        // 카메라 회전
        trCamera.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 초기 캐릭터 세팅
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


        #region 치트
        if (Input.GetKeyDown(KeyCode.F1))
        {
            HP = GameData.MaxHp;
            O2 = GameData.MaxO2[GameData.Instance.bagO2];
        }
        // 상점에서 사용
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
    /// 어택 적용(원거리 적용안해도 된다면 근접만 사용)
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
    /// 나이프 어택 애니메이션
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
        // 1.5f 거리 내의 물체 감지
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
                    gameManager.delayText.SetText("보물을 다 찾아야 이동 가능합니다.");
                }
            }
        }
    }
    public void UseItem(int itemNum)
    {
        Debug.Log("ItemNum" + itemNum + " 사용");
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
            Debug.Log("체력 감소");
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
