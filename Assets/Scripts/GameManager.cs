using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        private static GameManager _instance;

        private string[] _elements;
        private bool[] _currentShowing;
        private bool[] _matches;

        [SerializeField]
        private Text _feedbackText;

        private int _founds;
        private bool _win;

        public static GameManager Instance {
            get
            {
                if (_instance != null) return _instance;
                var singleton = new GameObject();
                _instance = singleton.AddComponent<GameManager>();
                DontDestroyOnLoad(singleton);
                return _instance;
            }
        }

        void Awake()
        {
            if(_instance != null) Destroy(transform.gameObject);
            _instance = this;
            DontDestroyOnLoad(transform.gameObject);
            _elements = FindObjectsOfType<ImageTargetBehaviour>().Select(element => element.TrackableName).ToArray();
            _currentShowing = new bool[_elements.Length];
            _matches = new bool[_elements.Length];

        }

        public void ElementFound(string foundObjectName)
        {
            var index = Array.IndexOf(_elements, foundObjectName);
            _currentShowing[index] = true;
            if (_matches[index]) return;
            _founds++;
            _feedbackText.text = "";
            var pairName = foundObjectName.Contains("2") ? foundObjectName.Substring(0, foundObjectName.Length - 1) : foundObjectName + "2";
            var secondIndex = Array.IndexOf(_elements, pairName);
            if (!_currentShowing[secondIndex])
            {
                if (!_win && _founds % 2 == 0)
                    _feedbackText.text = "Mismatch";
                return;
            }
            _feedbackText.text = "Match";
            _matches[index] = true;
            _matches[secondIndex] = true;
            CheckWin();

        }

        public void ElementLost(string lostObjectName)
        {
            if (_founds == 0) return;
            var index = Array.IndexOf(_elements, lostObjectName);
            _currentShowing[index] = false;
            if (_matches[index] || _win) return;
            _founds--;
            _feedbackText.text = "";
        }

        private void CheckWin()
        {
            _win = true;
            if (!_matches.Contains(false)) _feedbackText.text = "Win";
        }

    }
}
