using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    
    /// <summary>
    /// 아이템 넘버, 0: HP, 1: O2, 2: find, 3: 이동속도1, 4: 이동속도2, 5:인식불가
    /// </summary>
    public int itemNum;
    /// <summary>
    /// 게임 매니저
    /// </summary>
    GameManager gameManager;
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string itemName;
    /// <summary>
    /// 아이템 개수
    /// </summary>
    public int num;
    /// <summary>
    /// 아이템 개당 무게
    /// </summary>
    public int weight;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// 초기값
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
