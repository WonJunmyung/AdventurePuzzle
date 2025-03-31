using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    Transform player;
    Vector3 originPos;
    /// <summary>
    /// 함정 데미지 관련
    /// </summary>
    public float attackTimer = 5.0f;
    public float attackTiming = 0;
    public bool isAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        originPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 pos = player.position;
            pos.y = this.transform.position.y;
            if (isAttack)
            {
                attackTiming += Time.deltaTime;
                if (attackTiming > attackTimer)
                {
                    isAttack = false;
                    attackTiming = 0;
                }
            }

            
            if (Vector3.Distance(pos, this.transform.position) < 0.5f)
            {
                this.transform.position = originPos + new Vector3(0, 0.5f, 0);
                if (!isAttack)
                {
                    isAttack = true;
                    player.GetComponent<PlayerControl>().SetDamage(5);

                }

            }
            else
            {
                this.transform.position = originPos;
            }

        }
    }
}
