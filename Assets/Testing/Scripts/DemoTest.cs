using UnityEngine;
using System.Collections;
using alexkutepov.Questionnaire;

namespace alexkutepov.Questionnaire.Demo {

    public class DemoTest : MonoBehaviour {

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void getNextQ(int answerid) {
            Question q = TestSystem.instance.GetNextQuestion(answerid);
            if(q != null) {
                Debug.Log(q.textQuestion);
                foreach(answer item in q.answers) {
                    Debug.Log(item.isCorrect.ToString() + " -- " + item.text);
                }
            }
        }
    }
}