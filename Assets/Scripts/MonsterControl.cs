using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : MonoBehaviour
{
    /// <summary>
    /// 패트롤 적
    /// </summary>
    public GameObject patroMonster;
    /// <summary>
    /// AI 적
    /// </summary>
    public GameObject AIMonster;


    private void Start()
    {
        patroMonster = Resources.Load<GameObject>("Prefabs/PatrolMonster");
        AIMonster = Resources.Load<GameObject>("Prefabs/AIMonster");
    }

    private void Update()
    {
        
    }

    public void CreateMonster(Vector3 pos)
    {
        Instantiate(patroMonster, pos, patroMonster.transform.rotation);
    }

}
