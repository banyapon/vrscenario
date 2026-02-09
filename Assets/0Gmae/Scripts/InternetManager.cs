using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

namespace Boy
{
    public class InternetManager : MonoBehaviour
    {
        public static InternetManager Instance;
        public string apiUrl = "";

        [Header("Status")]
        [SerializeField] private bool internetStatus;
        [SerializeField] private bool googleTest = true, apiStatus = true;
        public bool InternetStatus { get => internetStatus; set => internetStatus = value; }

        [Header("UI")]
        public Text[] netText;

        private void Awake()
        {
            Instance = this;
            SetText("");
            StartCoroutine(CheckAPI());
            StartCoroutine(CheckInternetConnection());
        }

        List<string> status = new List<string>();
        void Update()
        {
            status.Clear();
            if (!googleTest) status.Add("I");
            if (!apiStatus) status.Add("A");
            //if (!SocketIOController.Instance.connectStatus) status.Add("S");

            internetStatus = status.Count == 0;

            string text = "";
            if (!internetStatus) text = $"Disconnected {String.Join(", ", status)}";
            SetText(text);
        }

        void SetText(string text)
        {
            foreach (Text t in netText) t.text = text;
        }

        public IEnumerator CheckAPI()
        {
            yield return new WaitForSeconds(5);
            if (string.IsNullOrEmpty(apiUrl)) yield break;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                yield return webRequest.SendWebRequest();
                apiStatus = webRequest.result == UnityWebRequest.Result.Success;
            }
            StartCoroutine(CheckAPI());
        }

        IEnumerator CheckInternetConnection()
        {
            yield return new WaitForSeconds(2.5f);
            using (UnityWebRequest request = new UnityWebRequest("https://www.google.com/"))
            {
                yield return request.SendWebRequest();
                googleTest = request.error == null;
                StartCoroutine(CheckInternetConnection());
            }
        }
    }
}
