using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MachingCardGame : MonoBehaviour
{
    /// <summary>
    /// 카드 뒷면에 사용할 스프라이트
    /// </summary>
    public Sprite cardBack;
    /// <summary>
    /// 카드 앞면 스프라이트
    /// </summary>
    public Sprite[] cardFaces;
    /// <summary>
    /// 카드 배열
    /// </summary>
    public Button[] cardButtons;
    /// <summary>
    /// 총 카드 배열
    /// </summary>
    private int[] cardValues;
    /// <summary>
    /// 총 카드 배열 짝을 이룰 카드들
    /// </summary>
    private bool[] matchedCards;
    /// <summary>
    /// 처음 선택
    /// </summary>
    private int firstSelected = -1;
    /// <summary>
    // 두번째 선택
    /// </summary>
    private int secondSelected = -1;
    /// <summary>
    /// 체크 중일때
    /// </summary>
    private bool isCheckingMatch = false;
    /// <summary>
    /// 변경 가능한 짝맞추기 개수
    /// </summary>
    public int totalPairs = 4; 
    /// <summary>
    /// 시간 제한(60초)
    /// </summary>
    public float timeLimit = 60f; 
    /// <summary>
    /// 남은 시간
    /// </summary>
    private float remainingTime;
    /// <summary>
    /// 남은 시간
    /// </summary>
    public Text timerText;
    /// <summary>
    /// 성공한 개수
    /// </summary>
    private int successNum = 0;

    public GameObject objPuzzle;
    public GameObject prefabBackCard;

    /// <summary>
    /// 게임매니저
    /// </summary>
    public GameManager gameManager;

    void Start()
    {
        InitializeGame();
        remainingTime = timeLimit;
        StartCoroutine(Timer());
    }

    


    void InitializeGame()
    {
        // 게임 매니저를 검색해서 넣어준다.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cardFaces = Resources.LoadAll<Sprite>("Puzzle");
        objPuzzle = GameObject.Find("Puzzle");
        prefabBackCard = Resources.Load<GameObject>("Prefabs/BackCard");
        cardButtons = new Button[totalPairs * 2];
        for(int i=0;i< totalPairs * 2;i++)
        {
            cardButtons[i] = Instantiate(prefabBackCard, objPuzzle.transform).GetComponent<Button>();
        }

        // 카드 배열 생성
        cardValues = new int[totalPairs * 2];
        // 짝을 맞출 카드 목록 생성
        matchedCards = new bool[totalPairs * 2];
        // 카드들의 배열
        List<int> values = new List<int>();
        // 카드들 배열 할당(2개씩)
        for (int i = 0; i < totalPairs; i++)
        {
            values.Add(i);
            values.Add(i);
        }
        // 섞기
        Shuffle(values);

        for (int i = 0; i < totalPairs * 2; i++)
        {
            int index = i;
            cardValues[i] = values[i];
            
            
            cardButtons[i].onClick.AddListener(() => OnCardSelected(index));
            cardButtons[i].image.sprite = cardBack;
        }
    }

    /// <summary>
    /// 카드 뒤섞기
    /// </summary>
    /// <param name="list">카드 배열</param>
    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    /// <summary>
    /// 카드 선택
    /// </summary>
    /// <param name="index"></param>
    void OnCardSelected(int index)
    {
        if (matchedCards[index] || index == firstSelected || isCheckingMatch)
            return;

        cardButtons[index].image.sprite = cardFaces[cardValues[index]];

        if (firstSelected == -1)
        {
            firstSelected = index;
        }
        else
        {
            secondSelected = index;
            isCheckingMatch = true;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (cardValues[firstSelected] == cardValues[secondSelected])
        {
            matchedCards[firstSelected] = true;
            matchedCards[secondSelected] = true;
            successNum++;
            if(successNum == totalPairs)
            {
                ShowMenuUI(true);
            }
        }
        else
        {
            cardButtons[firstSelected].image.sprite = cardBack;
            cardButtons[secondSelected].image.sprite = cardBack;
        }
        firstSelected = -1;
        secondSelected = -1;
        isCheckingMatch = false;
    }

    IEnumerator Timer()
    {
        if (!(successNum == totalPairs))
        {
            while (remainingTime > 0)
            {

                remainingTime -= Time.deltaTime;
                timerText.text = "Time: " + Mathf.Ceil(remainingTime);
                yield return null;
            }
        }
        ShowMenuUI(false);
    }

    void ShowMenuUI(bool isWin)
    {
        foreach (var button in cardButtons)
        {
            button.interactable = false;
        }
        gameManager.SetGameClear(isWin);
    }

    
}
