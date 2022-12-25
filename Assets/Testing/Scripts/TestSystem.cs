using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Xml;
using System.IO;

namespace alexkutepov.Questionnaire {

    public class TestSystem : MonoBehaviour {

        const string testingFile = "tests.xml";

        public static TestSystem instance;
        public static UnityEvent OnResetTest = new UnityEvent();

        public UnityEvent OnTestComplete = new UnityEvent();
        public TestResultEvent OnTestResult = new TestResultEvent();

        [Tooltip("Questions")]
        public Question[] Questions;
        [Space]
        [Header("Test setup")]
        [Tooltip("Nubmer of questions in the test")]
        public int numQuestionOnTesting = 3;
        [Tooltip("Min percentage of correct answers")]
        public float ThresholdValue = 0.6f;

        [Tooltip("Shuffle questions")]
        public bool mixQuesions = true;
        [Tooltip("Shuffle answers to questions")]
        public bool mixAnswers = true;
        [Tooltip("Show correct answer first  (the flag is igrored if mixAnswers = true)")]
        public bool showCorrectFirst = false;

        [HideInInspector]
        public bool moreOneAnswers = false;
        [HideInInspector]
        public bool moreOneAnswersForDisableGO = false;
        [HideInInspector]
        public int countAnswersCorrect = 1;

        int currentPos = -1;
        [Header("Test result")]
        [Tooltip("Test result. Is created before will invoke OnTestComplete")]
        public TestResult TestResults;

        void Start() {
            instance = this;
            Reset();
        }

        private void calcResult() {
            TestResult tres = new TestResult();
            tres.correctAnswers = 0.0f;
            tres.correctPercents = 0.0f;
            tres.numQuestions = numQuestionOnTesting;
            tres.success = false;
            int maxq = Mathf.Min(numQuestionOnTesting, Questions.Length);
            tres.correctAnswers = (float) Math.Round(summCorrectAnswers, 2);
            Debug.Log(tres.correctAnswers);
            tres.correctPercents = (float) tres.correctAnswers /tres.numQuestions;
            tres.success = (tres.correctPercents >= ThresholdValue);
            TestResults = tres;
            OnTestResult.Invoke(tres);
        }

        float summCorrectAnswers = 0.0f;

        public Question GetNextQuestion(int answerid) {
            if(currentPos != -1 && currentPos < Questions.Length) {
     
                Questions[currentPos].answeredID.Add(answerid);
                summCorrectAnswers = summCorrectAnswers + Questions[currentPos].answers[answerid].weight;
                Debug.Log(summCorrectAnswers);
            }
            return GetNextQuestion();
        }

        public Question GetNextQuestion(int[] answerid) {
            if(currentPos != -1 && currentPos < Questions.Length) {
                Questions[currentPos].answeredID.AddRange(answerid);

            }
            return GetNextQuestion();
        }

        private Question GetNextQuestion() {
            if (!moreOneAnswers) currentPos++;
            else moreOneAnswersForDisableGO = true;
            if (currentPos < numQuestionOnTesting 
                && currentPos < Questions.Length) {
                if (Questions[currentPos].GetCountCorrectNum() > 1 
                    && countAnswersCorrect != 
                    Questions[currentPos].GetCountCorrectNum()) { 
                    countAnswersCorrect++;
                    moreOneAnswers = true;
                } else {
                    moreOneAnswers = false;
                    moreOneAnswersForDisableGO = false;
                    countAnswersCorrect = 1;
                }
                Question cq = Questions[currentPos].Clone();
                return cq;
            } else {
                //TODO prepare test results
                calcResult();
                OnTestComplete.Invoke();
                return null;
            }
        }

        public void Reset() {
            currentPos = -1;
            foreach (Question item in Questions) {
                item.Reset();
                if(mixAnswers) {
                    item.answers = (answer[])mixArray(item.answers);
                    
                } else if(showCorrectFirst) {
                    item.SetCorrectFirst();
                }
            }
            for (int i = 0; i < Questions.Length; i++) {
                Questions[i].CheckQuestionsOnMoreOneCorrectAnswers();
            }
            if (mixQuesions) {
                Questions = (Question[])mixArray(Questions);
            }
        }

        [ContextMenu("Read Questions xml file")]
        public void loadQuestionsFromFile() {
            List<Question> tlist = new List<Question>();
            FileStream fs = new FileStream(Application.dataPath + "\\" + testingFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            //Debug.Log(fs.Name);
            XmlReader xmlReader = XmlReader.Create(fs);
           
            Question cq = new Question();
            while(xmlReader.Read()) {
                //Base question
                if(xmlReader.NodeType == XmlNodeType.Element) {
                    if(xmlReader.Name == "question") {
                        cq = new Question();
                        tlist.Add(cq);
                    }
                    if(xmlReader.Name == "text") {
                        // load questions text
                        XmlReader tReader = xmlReader.ReadSubtree();
                        while(tReader.Read()) {
                            if(tReader.NodeType == XmlNodeType.CDATA || tReader.NodeType == XmlNodeType.Text) {
                                cq.textQuestion = tReader.Value;
                            }
                        }
                    }
                    if(xmlReader.Name == "answer") {
                        //add answer
                        string isCorrStr = xmlReader.GetAttribute("correct");
                        bool isCorrect = (isCorrStr == "1");
                        XmlReader aReader = xmlReader.ReadSubtree();
                        while(aReader.Read()) {
                            if(aReader.NodeType == XmlNodeType.CDATA || aReader.NodeType == XmlNodeType.Text) {
                                cq.addAnswer(aReader.Value, isCorrect);
                            }
                        }
                    }

                    if(xmlReader.Name == "settings") {
                        //TODO Load settings
                        XmlReader sReader = xmlReader.ReadSubtree();
                        while(sReader.Read()) {
                          
                            if(sReader.NodeType == XmlNodeType.Element && sReader.Name == "amountquestions") {
                                numQuestionOnTesting = sReader.ReadElementContentAsInt();
                            }
                        
                            if(sReader.NodeType == XmlNodeType.Element && sReader.Name == "threshold") {
                                ThresholdValue = sReader.ReadElementContentAsFloat();
                            }
                            if(sReader.NodeType == XmlNodeType.Element && sReader.Name == "mixquestions") {
                                mixQuesions = sReader.ReadElementContentAsBoolean();
                            }
                            if(sReader.NodeType == XmlNodeType.Element && sReader.Name == "mixanswers") {
                                mixAnswers = sReader.ReadElementContentAsBoolean();
                            }
                            if(sReader.NodeType == XmlNodeType.Element && sReader.Name == "correctfirst") {
                                showCorrectFirst = sReader.ReadElementContentAsBoolean(); 
                            }
                        }
                    }
                }
            }
            Questions = tlist.ToArray();
            tlist = null;
           
            Debug.Log("Questions loaded from " + testingFile);
        }

        [ContextMenu("Save questions to XML File")]
        public void saveQuestionsToFile() {
            XmlWriterSettings xsett = new XmlWriterSettings();
            xsett.Encoding = System.Text.Encoding.Unicode;
            xsett.OmitXmlDeclaration = true;
            xsett.Indent = true;
            xsett.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(Application.dataPath + "\\" + testingFile, xsett);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("data");
            //xmlWriter.WriteAttributeString("usecdata", "1");
            for(int i = 0; i < Questions.Length; i++) {
                //TODO
                xmlWriter.WriteStartElement("question");
                // xmlWriter.WriteAttributeString("keyword", Questions[i].constString);
                //Запись тела вопроса
                xmlWriter.WriteStartElement("text");
                xmlWriter.WriteCData(Questions[i].textQuestion);
                xmlWriter.WriteEndElement();
                //Ответы
                for(int j = 0; j < Questions[i].answers.Length; j++) {
                    xmlWriter.WriteStartElement("answer");
                    string correct = (Questions[i].answers[j].isCorrect) ? "1" : "";
                    xmlWriter.WriteAttributeString("correct", correct);
                    xmlWriter.WriteCData(Questions[i].answers[j].text);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            //TODO Write Test settings
            xmlWriter.WriteStartElement("settings");
            // numQuestionOnTesting
            xmlWriter.WriteStartElement("amountquestions");
            xmlWriter.WriteString(numQuestionOnTesting.ToString());
            xmlWriter.WriteEndElement();

            //ThresholdValue
            xmlWriter.WriteStartElement("threshold");
            xmlWriter.WriteString(ThresholdValue.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("mixquestions");
            xmlWriter.WriteString(mixQuesions? "1" : "0");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("mixanswers");
            xmlWriter.WriteString(mixAnswers ? "1" : "0");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("correctfirst");
            xmlWriter.WriteString(showCorrectFirst ? "1" : "0");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            Debug.Log("Questions Saved to " + testingFile);/**/
        }

        static System.Random rnd = new System.Random();

        public static Array mixArray(Array items) {

            Array clone = (Array)items.Clone();
            Type itemtype = items.GetType().GetElementType();
            SortedList localdict = new SortedList();
            
            for(int i = 0; i < clone.Length; i++) {
                localdict.Add(rnd.Next(), items.GetValue(i));
            }

            int indexelm = 0;
            
            foreach(DictionaryEntry item in localdict) {
                clone.SetValue(item.Value, indexelm);
                indexelm++;
            }
            return clone;

        }
    }

    [System.Serializable]
    public class Question {

        public string textQuestion;
        public Sprite spriteQuestion;
        public answer[] answers;
        [HideInInspector]
        public List<int> answeredID;

        public Question() {
            Reset();
        }

        public void addAnswer(string text, bool isCorrect) {
            answer[] newa;
            if(answers != null) {
                newa = new answer[answers.Length + 1];
                answers.CopyTo(newa, 0);
            } else {
                newa = new answer[1];
            }
            answer additem = new answer();
            additem.text = text;
            additem.isCorrect = isCorrect;
            newa[newa.Length - 1] = additem;
            answers = newa;
            
        }

        public void CheckQuestionsOnMoreOneCorrectAnswers() {
            if (GetCountCorrectNum() != 0)
                for (int i = 0; i < answers.Length; i++) {
                    if (answers[i].isCorrect) {
                        answers[i].weight = 1.0f / GetCountCorrectNum();
                    }
                }
        }

        public int GetCountCorrectNum() {
            int countCorrectNum = 0;
            for (int i = 0; i < answers.Length; i++)  {
                if (answers[i].isCorrect) countCorrectNum++;
            }
            return countCorrectNum;
        }

        public float GetWeight() {
            float rez=0;
            for (int i = 0; i < answers.Length; i++) {
                if (answers[i].isCorrect) rez+= answers[i].weight;
            }
            return rez;
        }

        public void Reset() {
            answeredID = new List<int>();
        }

        public bool Check() {
            int correctNum = 0;
            for(int i = 0; i < answeredID.Count; i++) {
                int ida = answeredID[i];
                if (answers[ida].isCorrect) {
                    correctNum++;
                }
            }
            int realCorrectNum = 0;
            foreach (answer item in answers) {
                if (item.isCorrect) {
                    realCorrectNum++;
                }
            }
            if(correctNum != realCorrectNum) {
                return false;
            } else {
                return true;
            }
        }

        public Question Clone() {
            Question copy = new Question();
            copy.textQuestion = textQuestion;
            copy.answers = new answer[answers.Length];
            answers.CopyTo(copy.answers, 0);
            return copy;
        }

        public void SetCorrectFirst() {
            List<answer> correctElements = new List<answer>();
            
            foreach(answer item in answers) {
                int indx = 0;
                if(!item.isCorrect) {
                    indx = correctElements.Count;
                }
                correctElements.Insert(indx, item);
            }
            answers = correctElements.ToArray();
        }

    }

    [Serializable]
    public struct answer {
        public string text;
        public Sprite sprite;
        public bool isCorrect;
        [HideInInspector]
        public float weight;
    }

    [Serializable]
    public struct TestResult {
        public int numQuestions;
        public float correctAnswers;
        public float correctPercents;
        public bool success;
    }
    
    [Serializable]
    public class TestResultEvent:UnityEvent<TestResult> {

    }
    
}


