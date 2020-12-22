using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class TutorialSystem : MonoBehaviour
{
    //message box
    public GameObject mainWindow;
    //sub box
    public GameObject subWindow;
    //If you want to move the window to display
    public Animator mainAnim;
    public Animator subAnim;
    //Message Box Text
    public Text mainText;
    //Sub  Box Text
    public Text subText;
    //The actual Text to process
    public Text utilityText;
    //Image for Sub Box
    public Image tagetImage;
    //Sprites to be used. If there are many, make an array.
    private Sprite[] targets = new Sprite[2];
    //Flags to be used between additive scenes
    private int tipsChecker = 0;
    //Next button for the main box
    public Button nextButton;
    //The Next button, which is actually used.
    public Button utilityButton;
    //Class for storing text
    public tutorialTexts tutoText;
    //Current message number
    private int textCount;
    //The character number you just displayed.
    private int nowTextNum = 0;
    //Whether you have displayed one message.
    private bool isOneMessage = false;
    //Whether you have displayed all the messages.
    private bool isEndMessage = true;


    // Start is called before the first frame update
    void Start()
    {
        //Get token.
        var token = this.GetCancellationTokenOnDestroy();
        //If there are any buttons you want to disable in the tutorial, add them to the process.

        //If you want to include an Image in the subbox.
        SetTargets();
        //Starting a task
        ScenarioRegenerater(token).Forget();
    }

    public async UniTaskVoid ScenarioRegenerater(CancellationToken token)
    {
        //============================ //
        //                                                         //
        //　Here are the steps of the tutorial.      //
        //                                                         //
        //============================ //

        //Example

        await MainTipsRegenerater(tutoText.mainsentencesA, token);
        await MainTipsRegenerater(tutoText.mainsentencesA, token);
        await MainTipsRegenerater(tutoText.mainsentencesA, token);

        await SubTipsRegenerater(tutoText.subsentencesA[0], 0, token);

        await UniTask.Delay(2000, false, PlayerLoopTiming.Update, token);

        await EndSubTips(token);

        await MainTipsRegenerater(tutoText.mainsentencesB, token);
        await MainTipsRegenerater(tutoText.mainsentencesB, token);
        await MainTipsRegenerater(tutoText.mainsentencesB, token);

        await SubTipsRegenerater(tutoText.subsentencesB[0], 1, token);

        await UniTask.Delay(2000, false, PlayerLoopTiming.Update, token);

        await EndSubTips(token);

        await SubTipsRegenerater(tutoText.subsentencesB[1], 1, token);

        await UniTask.Delay(2000, false, PlayerLoopTiming.Update, token);

        await EndSubTips(token);

    }

    async UniTask MainTipsRegenerater(string[] sentences, CancellationToken token)
    {
        while (isEndMessage || sentences == null)
        {
            //If you want to use it in an additive scene, set tipsChecker to 1 
            //and set utilityText or utilityButton to Start() of the loaded scene.
            switch (tipsChecker)
            {
                case 0:
                    if (!mainWindow.activeSelf)
                    {
                        mainWindow.SetActive(true);
                        await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);

                        //For animation
                        mainAnim.SetBool("onAir",true);
                        await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);
                        utilityText = mainText;
                        utilityButton = nextButton;

                        //When tipsChecker is set to 0, isEndMessage is set to false and the main part works.
                        isEndMessage = false;
                    }
                    break;
                case 1:
                    tipsChecker = 2;
                    isEndMessage = false;
                    break;
                case 2:
                    break;
            }


            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        while (!isEndMessage)
        {
            //No message to be displayed at one time
            if (!isOneMessage)
            {
                //If all messages are displayed, the game objects are hidden
                if (textCount >= sentences.Length)
                {
                    textCount = 0;
                    
                    switch (tipsChecker)
                    {
                        case 0:
                            //For animation
                            mainAnim.SetBool("onAir", false);
                            await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);
                            mainWindow.SetActive(false);
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                    }
                    isOneMessage = false;
                    isEndMessage = true;
                    return;
                }
                //Otherwise, initialize the text processing related items and display them from the next character.

                //Add one character after the text display time has elapsed.
                utilityText.text += sentences[textCount][nowTextNum];
                nowTextNum++;

                //The full message was displayed, or the maximum number of lines were displayed.
                if (nowTextNum >= sentences[textCount].Length)
                {
                    isOneMessage = true;
                }

                await UniTask.Delay(1, false, PlayerLoopTiming.Update, token);
            }
            else
            {
                //Message to be displayed at one time.
                await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);

                //Press the Next button.
                utilityButton.interactable = true;
                isEndMessage = true;
            }
        }
    }

    async UniTask SubTipsRegenerater(string sentence, int tar, CancellationToken token)
    {
        
        //Show subboxes.
        subWindow.SetActive(true);
        tagetImage.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        tagetImage.sprite = targets[tar];
        await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);

        //For animation
        subAnim.SetBool("onAir", true);
        await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);

        isEndMessage = false;

        while (!isEndMessage)
        {
            //No message to be displayed at one time	
            if (!isOneMessage)
            {
                //Add one character after the text display time has elapsed.
                subText.text += sentence[nowTextNum];
                nowTextNum++;

                //The full message was displayed, or the maximum number of lines were displayed.
                if (nowTextNum >= sentence.Length)
                {
                    isOneMessage = true;
                }

                await UniTask.Delay(1, false, PlayerLoopTiming.Update, token);
            }
            //Message to be displayed at one time.
            else
            {
                await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);

                nowTextNum = 0;
                isEndMessage = true;
                return;
            }
        }
    }

    async UniTask EndSubTips(CancellationToken token)
    {
        subText.text = "";
        isOneMessage = false;

        //For animation
        subAnim.SetBool("onAir", false);
        await UniTask.Delay(500, false, PlayerLoopTiming.Update, token);
        subWindow.SetActive(false);
    }

    public void NextButton()
    {
        //Initialize the message function when you press the Next button.
        utilityButton.interactable = false;
        utilityText.text = "";
        nowTextNum = 0;
        //When displaying multiple times in a row, the number of Text is counted.
        textCount++;
        isOneMessage = false;
        isEndMessage = false;
    }

    void SetTargets()
    {
        targets[0] = Resources.Load<Sprite>("TutorialSprite/example1");
        targets[1] = Resources.Load<Sprite>("TutorialSprite/example2");
    }

}
