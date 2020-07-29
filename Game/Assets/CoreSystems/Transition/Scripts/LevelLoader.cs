﻿using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreSystems.TransitionSystem
{
    public enum TransitionType
    {
        Fade,
        HorizontalSwipe
    }

    public enum Level
    {
        Splash,
        Menu,
        Game
    }

    public class LevelLoader : Singleton<LevelLoader>
    {
        public TransitionType transitionType;

        private Transition transition;

        public bool LoadingLevel { get; private set; }

        void Awake()
        {
            var transitions = GetComponentsInChildren<Transition>();
            transition = transitions.FirstOrDefault(p => p.TransitionType == transitionType);
            var disabledTransitions = transitions.Where(p => p != transition);

            if (transition == null)
            {
                Debug.LogError($"Transition '{transitionType.ToString()}' does not exist.");
            }
            else
            {
                transition.gameObject.SetActive(true);
            }

            disabledTransitions.ToList().ForEach(p => p.gameObject.SetActive(false));
        }

        public void LoadNextLevel()
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // Start is called before the first frame update
        public void LoadLevel(int sceneBuildIndex)
        {
            StartCoroutine(LoadLevelRoutine(sceneBuildIndex));
        }

        public void LoadLevel(Level level)
        {
            LoadLevel((int)level);
        }

        IEnumerator LoadLevelRoutine(int sceneBuildIndex)
        {
            LoadingLevel = true;
            transition.TransitionOut();

            yield return new WaitForSeconds(transition.TransitionTime);

            LoadingLevel = false;
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}