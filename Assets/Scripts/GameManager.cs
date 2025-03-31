using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 플레이어 컨트롤러
    /// </summary>
    public PlayerControl playerControl;
    /// <summary>
    /// 맵 컨트롤러
    /// </summary>
    public MapControl mapControl;
    /// <summary>
    /// 몬스터 컨트롤러
    /// </summary>
    public MonsterControl monsterControl;
    /// <summary>
    /// 메세지
    /// </summary>
    public DelayText delayText;
    /// <summary>
    /// 보물상자 개수 UI
    /// </summary>
    public Text ChestNum;
    /// <summary>
    /// 체력 UI
    /// </summary>
    public Slider HPBar;
    /// <summary>
    /// 산소 UI
    /// </summary>
    public Slider O2Bar;
    /// <summary>
    /// 퍼즐게임
    /// </summary>
    public MachingCardGame gameCard;
    /// <summary>
    /// 메뉴 UI
    /// </summary>
    public GameObject MenuUI;
    /// <summary>
    /// 전체 시간
    /// </summary>
    public Text totalTime;
    
    public void InitGame()
    {
        if (GameObject.Find("GameData") == null)
        {
            delayText.SetText("GameData가 생성되지 않았습니다. 메인에서부터 실행하세요.");
            return;
        }
        // 플레이어의 빈오브젝트를 생성하고 플레이어 컨트롤 연결(몬스터와 맵보다 먼저 생성되어야함)
        playerControl = new GameObject("Player").AddComponent<PlayerControl>();
        // 몬스터의 빈오브젝트를 생성하고 몬스터 컨트롤 연결(맵보다는 먼저 생성되어야 함)
        monsterControl = new GameObject("MonsterControl").AddComponent<MonsterControl>();
        // 맵의 빈오브젝트를 생성하고 맵 커트롤 연결
        mapControl = new GameObject("MapControl").AddComponent<MapControl>();
        // 맵의 시작 지점을 받아온다.
        // 보물상자 텍스트
        ChestNum = GameObject.Find("ChestNum").GetComponent<Text>();
        
        // 마우스 잠금 상태
        Cursor.lockState = CursorLockMode.Locked;
        GameData.Instance.isMouse = false;
        // 체력 바
        HPBar = GameObject.Find("HPBar").GetComponent<Slider>();
        // 산소 바
        O2Bar = GameObject.Find("O2Bar").GetComponent<Slider>();

        
    }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    GameModeChange();


        //}
    }

    public void GameModeChange()
    {
        if (!GameData.Instance.isMouse)
        {
            Cursor.lockState = CursorLockMode.None;
            GameData.Instance.isMouse = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameData.Instance.isMouse = false;
        }
    }

    public void SetChest(int chestNum)
    {
        ChestNum.text = "X" + chestNum;
    }

    /// <summary>
    /// 게임 메뉴띄우기
    /// </summary>
    /// <param name="isGame"></param>
    public void SetGameClear(bool isGame)
    {
        MenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GameData.Instance.isMouse = true;
        if (isGame)
        {
            MenuUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Clear";
        }
        else
        {
            MenuUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Over";
        }
    }

    /// <summary>
    /// 메뉴 버튼
    /// </summary>
    /// <param name="num"></param>
    public void BtnMenu(int num)
    {
        switch (num)
        {
            case 0:
                {
                    GameData.Instance.isGame = false;
                    SceneManager.LoadScene("Main");
                }
                break;
            case 1:
                {
                    GameData.Instance.isGame = false;
                    SceneManager.LoadScene("Game");
                }
                break;
            case 2:
                {
                    
                    Application.Quit();
                }
                break;
        }
    }

}
