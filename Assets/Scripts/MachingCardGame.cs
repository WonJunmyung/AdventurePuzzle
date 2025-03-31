using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MachingCardGame : MonoBehaviour
{
    /// <summary>
    /// ī�� �޸鿡 ����� ��������Ʈ
    /// </summary>
    public Sprite cardBack;
    /// <summary>
    /// ī�� �ո� ��������Ʈ
    /// </summary>
    public Sprite[] cardFaces;
    /// <summary>
    /// ī�� �迭
    /// </summary>
    public Button[] cardButtons;
    /// <summary>
    /// �� ī�� �迭
    /// </summary>
    private int[] cardValues;
    /// <summary>
    /// �� ī�� �迭 ¦�� �̷� ī���
    /// </summary>
    private bool[] matchedCards;
    /// <summary>
    /// ó�� ����
    /// </summary>
    private int firstSelected = -1;
    /// <summary>
    // �ι�° ����
    /// </summary>
    private int secondSelected = -1;
    /// <summary>
    /// üũ ���϶�
    /// </summary>
    private bool isCheckingMatch = false;
    /// <summary>
    /// ���� ������ ¦���߱� ����
    /// </summary>
    public int totalPairs = 4; 
    /// <summary>
    /// �ð� ����(60��)
    /// </summary>
    public float timeLimit = 60f; 
    /// <summary>
    /// ���� �ð�
    /// </summary>
    private float remainingTime;
    /// <summary>
    /// ���� �ð�
    /// </summary>
    public Text timerText;
    /// <summary>
    /// ������ ����
    /// </summary>
    private int successNum = 0;

    public GameObject objPuzzle;
    public GameObject prefabBackCard;

    /// <summary>
    /// ���ӸŴ���
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
        // ���� �Ŵ����� �˻��ؼ� �־��ش�.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cardFaces = Resources.LoadAll<Sprite>("Puzzle");
        objPuzzle = GameObject.Find("Puzzle");
        prefabBackCard = Resources.Load<GameObject>("Prefabs/BackCard");
        cardButtons = new Button[totalPairs * 2];
        for(int i=0;i< totalPairs * 2;i++)
        {
            cardButtons[i] = Instantiate(prefabBackCard, objPuzzle.transform).GetComponent<Button>();
        }

        // ī�� �迭 ����
        cardValues = new int[totalPairs * 2];
        // ¦�� ���� ī�� ��� ����
        matchedCards = new bool[totalPairs * 2];
        // ī����� �迭
        List<int> values = new List<int>();
        // ī��� �迭 �Ҵ�(2����)
        for (int i = 0; i < totalPairs; i++)
        {
            values.Add(i);
            values.Add(i);
        }
        // ����
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
    /// ī�� �ڼ���
    /// </summary>
    /// <param name="list">ī�� �迭</param>
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
    /// ī�� ����
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
