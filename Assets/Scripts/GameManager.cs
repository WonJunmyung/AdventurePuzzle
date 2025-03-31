using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// �÷��̾� ��Ʈ�ѷ�
    /// </summary>
    public PlayerControl playerControl;
    /// <summary>
    /// �� ��Ʈ�ѷ�
    /// </summary>
    public MapControl mapControl;
    /// <summary>
    /// ���� ��Ʈ�ѷ�
    /// </summary>
    public MonsterControl monsterControl;
    /// <summary>
    /// �޼���
    /// </summary>
    public DelayText delayText;
    /// <summary>
    /// �������� ���� UI
    /// </summary>
    public Text ChestNum;
    /// <summary>
    /// ü�� UI
    /// </summary>
    public Slider HPBar;
    /// <summary>
    /// ��� UI
    /// </summary>
    public Slider O2Bar;
    /// <summary>
    /// �������
    /// </summary>
    public MachingCardGame gameCard;
    /// <summary>
    /// �޴� UI
    /// </summary>
    public GameObject MenuUI;
    /// <summary>
    /// ��ü �ð�
    /// </summary>
    public Text totalTime;
    
    public void InitGame()
    {
        if (GameObject.Find("GameData") == null)
        {
            delayText.SetText("GameData�� �������� �ʾҽ��ϴ�. ���ο������� �����ϼ���.");
            return;
        }
        // �÷��̾��� �������Ʈ�� �����ϰ� �÷��̾� ��Ʈ�� ����(���Ϳ� �ʺ��� ���� �����Ǿ����)
        playerControl = new GameObject("Player").AddComponent<PlayerControl>();
        // ������ �������Ʈ�� �����ϰ� ���� ��Ʈ�� ����(�ʺ��ٴ� ���� �����Ǿ�� ��)
        monsterControl = new GameObject("MonsterControl").AddComponent<MonsterControl>();
        // ���� �������Ʈ�� �����ϰ� �� ĿƮ�� ����
        mapControl = new GameObject("MapControl").AddComponent<MapControl>();
        // ���� ���� ������ �޾ƿ´�.
        // �������� �ؽ�Ʈ
        ChestNum = GameObject.Find("ChestNum").GetComponent<Text>();
        
        // ���콺 ��� ����
        Cursor.lockState = CursorLockMode.Locked;
        GameData.Instance.isMouse = false;
        // ü�� ��
        HPBar = GameObject.Find("HPBar").GetComponent<Slider>();
        // ��� ��
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
    /// ���� �޴�����
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
    /// �޴� ��ư
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
