using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public int attemptsNumber = 0;
    public DrawLine DrawLineScript;
    public int roundsNumber = 10;
    public Answer[] AllPossibleAnswers;
    public GameObject LineRenderingObject;
    public List<Round> rounds;
    public int currentRound = -1;
    public int correctAnswerCount = 0;
    public float secondsToWait = 60;
    bool waitingToRecount;
    //UI PART
    public GameObject PanelLose;
    public GameObject PanelWin;
    public GameObject centalPanel;
    public Image[] answerSprite;
    public TextMeshProUGUI[] answerText;
    public UnityEngine.UI.Extensions.UILineRenderer lineRenderer;
    public GameObject rendererParent;
    public GameObject rendererPrefab;
    public Image[] theStars;
    public Sprite spriteLit;
    public Sprite spriteUnlit;
    public Slider ProgressBar;
    int CoinNumber;
    public TextMeshProUGUI CoinText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI Currentlevel;
    public ClickableAnswer lastDraggedObject;
    public GameObject EndGameUI;
    //
    void Start()
    {
        if (_Instance == null)
            _Instance = this;
        rounds = new List<Round>(10);
        List<Answer> shuffledAnswers = AllPossibleAnswers.ToList();

        var rnd = new System.Random();
        List<Answer> randomizedAnswerList = shuffledAnswers.OrderBy(item => rnd.Next()).ToList();
        int currentAnswerI = 0;
        for (int i = 0; i< roundsNumber; i++)
        {
            Round r = new Round();
            r.answerList = new List<Answer>(3);
            for(int j = 0; j<3; j++)
            {
                Answer a = new Answer(randomizedAnswerList[currentAnswerI].AnswerText, randomizedAnswerList[currentAnswerI].AnswerSprite);
                r.answerList.Add(a);
                currentAnswerI++;
            } 
            rounds.Add(r);
        }
        attemptsNumber++;
        StartNextRound();
        Currentlevel.text = currentRound * 10 + " % progression";

    }

    public void backToMainMenu()
    {
        EndGameUI.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public IEnumerator addAfterFew(GameObject theObject)
    {
        yield return new WaitForSeconds(0.2f);
        UnityEngine.UI.Extensions.UILineRenderer uilr = theObject.AddComponent<UnityEngine.UI.Extensions.UILineRenderer>();
        uilr.color = Color.cyan;
        lineRenderer = uilr;
        Vector2[] points = { new Vector2(0, 0) };
        uilr.Points = points;
        lineRenderer.gameObject.GetComponent<DrawLine>().lineRenderer = lineRenderer;
        lineRenderer.lineThickness = 10;
    }

    public void resetLineRenderer()
    {
        for (int i=0;i< answerSprite.Length; i++)
        {
            answerSprite[i].GetComponent<ClickableAnswer>().isDone = false;
        }
    }
    public void StartNextRound()
    {
        currentRound++;
        if (currentRound == 10)
        {
            EndGameUI.SetActive(true);
            PanelLose.SetActive(false);
            PanelWin.SetActive(false);
            centalPanel.SetActive(false);
            return;
        }
            correctAnswerCount = 0;
        attemptsNumber = 1;
        if (currentRound < rounds.Count)
        {
            Debug.Log("Current round incremented to : " + currentRound);
            ProgressBar.value = currentRound+1;
            waitingToRecount = false;
            secondsToWait = 60;
            PanelLose.SetActive(false);
            PanelWin.SetActive(false);
            var rnd = new System.Random();
            Image[] UIAnswerSPs = answerSprite;
            Debug.Log("array length "+UIAnswerSPs.Length);
            TextMeshProUGUI[] UIAnswerTxts = answerText;
            UIAnswerSPs = UIAnswerSPs.OrderBy(item => rnd.Next()).ToArray();
            UIAnswerTxts = UIAnswerTxts.OrderBy(item => rnd.Next()).ToArray();
            for (int j = 0; j < 3; j++)
            {
                UIAnswerSPs[j].sprite = rounds[currentRound].answerList[j].AnswerSprite;
                UIAnswerTxts[j].text = rounds[currentRound].answerList[j].AnswerText;
                UIAnswerSPs[j].gameObject.GetComponent<SpriteAnswer>().AnswerText = UIAnswerTxts[j].text;
            }
            UnityEngine.UI.Extensions.UILineRenderer[] rList = FindObjectsOfType<UnityEngine.UI.Extensions.UILineRenderer>();
            foreach (UnityEngine.UI.Extensions.UILineRenderer rI in rList)
            {
                Destroy(rI.gameObject);
            }
            lineRenderer = Instantiate(rendererPrefab, rendererParent.transform).GetComponent<UnityEngine.UI.Extensions.UILineRenderer>();
            LineRenderingObject = lineRenderer.gameObject;
        } else
        {
            PanelWin.SetActive(true);
            PanelWin.transform.GetChild(4).gameObject.SetActive(false);
            PanelWin.transform.GetChild(5).gameObject.SetActive(false);
        }
    }

    public void RestartRound(bool RestartAfterWin =false)
    {
        correctAnswerCount = 0;
        if (RestartAfterWin)
        {
            attemptsNumber = 0;
        }
        PanelWin.SetActive(false);
        waitingToRecount = false;
        secondsToWait = 60;
        var rnd = new System.Random();
        Image[] UIAnswerSPs = answerSprite;
        TextMeshProUGUI[] UIAnswerTxts = answerText;
        UIAnswerSPs = UIAnswerSPs.OrderBy(item => rnd.Next()).ToArray();
        UIAnswerTxts = UIAnswerTxts.OrderBy(item => rnd.Next()).ToArray();
        for (int j = 0; j < 3; j++)
        {
            UIAnswerSPs[j].sprite = rounds[currentRound].answerList[j].AnswerSprite;
            UIAnswerTxts[j].text = rounds[currentRound].answerList[j].AnswerText;
            UIAnswerSPs[j].gameObject.GetComponent<SpriteAnswer>().AnswerText = UIAnswerTxts[j].text;
        }
        UnityEngine.UI.Extensions.UILineRenderer[] rList = FindObjectsOfType<UnityEngine.UI.Extensions.UILineRenderer>();
        foreach (UnityEngine.UI.Extensions.UILineRenderer rI in rList)
        {
            Destroy(rI.gameObject);
        }
        lineRenderer = Instantiate(rendererPrefab, rendererParent.transform).GetComponent<UnityEngine.UI.Extensions.UILineRenderer>();
        LineRenderingObject = lineRenderer.gameObject;
        
        attemptsNumber++;
        PanelWin.SetActive(false);
        PanelLose.SetActive(false);

    }

    public void CreateNewLR()
    {
        GameObject temp = Instantiate(LineRenderingObject, LineRenderingObject.transform.parent);
        DrawLine dl = temp.GetComponent<DrawLine>();
        dl.points = new List<Vector2>();
        dl.RefreshLine();
        dl.enabled = true;
        lineRenderer = temp.GetComponent<UnityEngine.UI.Extensions.UILineRenderer>();
    }

    public static bool IsPointerOverGameObject(GameObject gameObject)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        Debug.Log(raysastResults.Any(x => x.gameObject == gameObject));
        return raysastResults.Any(x => x.gameObject == gameObject);
    }


    public void DragStoppedOnAnswerText(GameObject g)
    {
        Debug.Log("Drag ended on " + g.name);
        lineRenderer.gameObject.GetComponent<DrawLine>().weCanEndDrag = true;
    }

    public void CheckMyAnswers()
    {
        waitingToRecount = true;
        Debug.LogError(correctAnswerCount);
        if (correctAnswerCount < 3)
        {
            Debug.Log("Game Over");
            PanelLose.SetActive(true);
        }
        else
        {
            Debug.Log("Correct");
            Debug.Log("attempts = " + attemptsNumber);
            PanelWin.SetActive(true);
            if (attemptsNumber == 1)
            {
                for(int i = 0; i<3; i++)
                {
                    theStars[i].sprite = spriteLit;
                }
                CoinNumber += 100;
                CoinText.text = CoinNumber + "";
                
            } else if(attemptsNumber > 1 && attemptsNumber < 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    theStars[i].sprite = spriteLit;
                }
                theStars[2].sprite = spriteUnlit;
                CoinNumber += 60;
                CoinText.text = CoinNumber + "";
            }
            else if (attemptsNumber > 3 && attemptsNumber < 6)
            {
                Debug.Log("nejma");
                theStars[0].sprite = spriteLit;
                theStars[1].sprite = spriteUnlit;
                theStars[2].sprite = spriteUnlit;
                CoinNumber += 30;
                CoinText.text = CoinNumber + "";
            } else
            {
                theStars[0].sprite = spriteUnlit;
                theStars[1].sprite = spriteUnlit;
                theStars[2].sprite = spriteUnlit;
                CoinNumber += 10;
                CoinText.text = CoinNumber + "";
            }
        }
        resetLineRenderer();
    }

    private void Update()
    {
        if(!waitingToRecount)
        {
            secondsToWait -= Time.deltaTime;
            if (secondsToWait <= 0 && !waitingToRecount)
            {
                waitingToRecount = true;
                secondsToWait = 0;
                Debug.Log("Game Over");
                PanelLose.SetActive(true);
            }
            TimeText.text = "Time : " + secondsToWait.ToString("0.00"); 
        }


        Currentlevel.text = currentRound * 10 + " % progression";


    }
}
