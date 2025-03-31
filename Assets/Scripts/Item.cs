using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    
    /// <summary>
    /// ������ �ѹ�, 0: HP, 1: O2, 2: find, 3: �̵��ӵ�1, 4: �̵��ӵ�2, 5:�νĺҰ�
    /// </summary>
    public int itemNum;
    /// <summary>
    /// ���� �Ŵ���
    /// </summary>
    GameManager gameManager;
    /// <summary>
    /// ������ �̸�
    /// </summary>
    public string itemName;
    /// <summary>
    /// ������ ����
    /// </summary>
    public int num;
    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public int weight;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// �ʱⰪ
    /// </summary>
    /// <param name="itemNum"></param>
    public void SetItem(int itemNum)
    {
        this.itemNum = itemNum;
        itemName = GameData.sName[this.itemNum];
        num = 1;
        weight = GameData.iWeight[this.itemNum];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
