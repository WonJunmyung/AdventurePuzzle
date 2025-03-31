using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RobbyControl : MonoBehaviour
{
    public Text[] itemText;
    public Text Cost;
    public DelayText delayText;

    bool isNotCost = false;
    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("GameData") == null)
        {
            delayText.SetText("GameData�� �������� �ʾҽ��ϴ�. ���ο������� �����ϼ���.");
            return;
        }
        for(int i = 0; i < GameData.Instance.isBuyItem.Length; i++)
        {
            if (GameData.Instance.isBuyItem[i])
            {
                itemText[i].text = GameData.itemText[i];
            }
            else
            {
                itemText[i].text = GameData.buyItemText;
            }
        }
        Cost.text = GameData.Instance.cost.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            isNotCost = !isNotCost;
        }
    }

    /// <summary>
    /// �κ� ������ ����
    /// </summary>
    /// <param name="num"></param>
    public void BuyItem(int num)
    {
        if (GameData.itemCost[num] <= GameData.Instance.cost)
        {
            GameData.Instance.isBuyItem[num] = false;
            itemText[num].text = GameData.buyItemText;
            GameData.Instance.cost -= GameData.itemCost[num];
            Cost.text = GameData.Instance.cost.ToString();
            if(num < 3)
            {
                if (GameData.Instance.bagO2 <= num + 1) {
                    GameData.Instance.bagO2 = num + 1;
                }
            }
            else
            {
                if(GameData.Instance.bagUpgradeNum <= num - 2)
                {
                    GameData.Instance.bagUpgradeNum = num - 2;
                }
            }
        }
        else if(isNotCost)
        {
            GameData.Instance.isBuyItem[num] = false;
            itemText[num].text = GameData.buyItemText;
            GameData.Instance.cost -= GameData.itemCost[num];
            Cost.text = GameData.Instance.cost.ToString();
            if (num < 3)
            {
                if (GameData.Instance.bagO2 <= num + 1)
                {
                    GameData.Instance.bagO2 = num + 1;
                }
            }
            else
            {
                if (GameData.Instance.bagUpgradeNum <= num - 2)
                {
                    GameData.Instance.bagUpgradeNum = num - 2;
                }
            }
            
        }
        else
        {
            delayText.SetText("���Ű� �Ұ����մϴ�.");
        }
    }

    public void InGame()
    {
        SceneManager.LoadScene("Game");
    }

    
}
