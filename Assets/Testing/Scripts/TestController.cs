using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using alexkutepov.Questionnaire;
using UnityEngine.Events;
using TMPro;

namespace alexkutepov.Questionnaire.Controller {
	
    public class TestController : MonoBehaviour {

        public UnityEvent OnTestComplete;

        public TextMeshProUGUI QTextArea;
        //TODO image for 
        public GameObject ImageForQueatione;
        public List<GameObject> ClickButtons;

        protected List<TextMeshProUGUI> tfInButtons;

        [Tooltip("UI tmp button for answers buttons")]
        public GameObject btTemplate;

        public GameObject answerContainter;
        
        [Header("Text the test rezult (pass or not) ")]
        public TextMeshProUGUI ResultTextExam;
        [Header("text number of corect answers")]
        public TextMeshProUGUI ResultTextCountCorectAnswer;
        [Header("Current Time")]
        public TextMeshProUGUI ResultTextTime;
 
        protected alexkutepov.Questionnaire.Question currentQuestion;
        public Color newColorForButtonClick = Color.grey;
        public Color oldButtonColor = new Color(28, 56, 84);
        [Header("Cultire info for Time")]
        [SerializeField] string cultureInfo;
        void Start() {
            onStart();
        }
        
        protected void onStart() {
            btTemplate.SetActive(false);
            if(answerContainter == null) answerContainter = btTemplate.transform.parent.gameObject;
            ClickButtons = new List<GameObject>();
            tfInButtons = new List<TextMeshProUGUI>();
        }

        public void initTest() {
            TestSystem.instance.Reset();
            NextStep(-1);
        }

        public virtual void NextStep(int answerID) {
            currentQuestion = TestSystem.instance.GetNextQuestion(answerID);
       
            if(currentQuestion == null) {
             
                OnTestComplete.Invoke();
            } else {
                CalcButtons();
                QTextArea.text = currentQuestion.textQuestion;
                Debug.Log(currentQuestion.textQuestion);
                Debug.Log(currentQuestion.spriteQuestion);
            }
        }

        protected void CalcButtons() {
            int currentbtnum = ClickButtons.Count;
            int requestBtns = currentQuestion.answers.Length;
            if(currentbtnum < requestBtns) {
                for(int j = currentbtnum; j < requestBtns; j++) {
                    GameObject newgo = AddNewButton(j);
                    ClickButtons.Add(newgo);
                }
            } else if(currentbtnum > requestBtns) {
                for(int i = requestBtns; i < currentbtnum; i++) {
                    ClickButtons[i].SetActive(false);
                }
            }
            for(int k = 0; k < ClickButtons.Count; k++) {
                if(k < requestBtns) {
                    tfInButtons[k].text = currentQuestion.answers[k].text;
                    ClickButtons[k].SetActive(true);
                }
            }
        }

        protected virtual GameObject AddNewButton(int j) {
            GameObject ninstance = Instantiate(btTemplate);
            ninstance.transform.SetParent(answerContainter.transform);
            ninstance.SetActive(true);
            Button btInst = ninstance.GetComponent<Button>();
            if(btInst) {
                btInst.onClick.AddListener(delegate { ButtonAnswerClicked(j); });
            }
            TextMeshProUGUI instText = ninstance.GetComponentInChildren<TextMeshProUGUI>();
            if (instText != null) {
                tfInButtons.Add(instText);
             
            } else {
                Debug.LogWarning("Text Field in " + ninstance + " not found");
            }
            return ninstance;
        }

        void SetRectTranformFromTheLengtOfTheStringQuestion(string question, RectTransform forSet) {
           //TODO button size
        }

        private void ButtonAnswerClicked(int valclick) {
            Debug.Log(valclick);
            if (TestSystem.instance.moreOneAnswers) {
                ClickButtons[valclick].GetComponent<Button>().enabled = false;
                ClickButtons[valclick].GetComponent<Image>().color = newColorForButtonClick;
            }
            else ResetButtonColorAndActive();
            NextStep(valclick);
        }


        private void ResetButtonColorAndActive()  {
            for (int i=0;i< ClickButtons.Count; i++)    {
                ClickButtons[i].GetComponent<Button>().enabled = true;
                ClickButtons[i].GetComponent<Image>().color = oldButtonColor;
            }
        }

        public void OnTestResults(TestResult tresult) { 

            System.DateTime localDate = System.DateTime.Now;
            var culture = new CultureInfo(cultureInfo);
            ResultTextExam.text = tresult.success ? "You passed the test!" : "You did not pass the test!";
            ResultTextCountCorectAnswer.text = "Number of correct answers: " + tresult.correctAnswers.ToString() + " out of " + tresult.numQuestions + ". ";
            ResultTextTime.text = "Current time: " + localDate.ToString(culture);
        }
    }
}