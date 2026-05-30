using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DirectorControllerScript : MonoBehaviour
{
    [Header("Directors")]
    [SerializeField] private PlayableDirector[] directors;
    [SerializeField] private string[] effectNames;

    [Header("UI")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text effectLabel;
    [SerializeField] private TMP_Text playButtonLabel;

    private int currentIndex;
    private bool isFinished;
    private bool isPaused;

    private void OnEnable()
    {
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(PreviousDirector);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextDirector);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(TogglePlay);
        }

        if (directors != null)
        {
            for (int i = 0; i < directors.Length; i++)
            {
                if (directors[i] != null)
                {
                    directors[i].stopped += OnDirectorStopped;
                }
            }
        }

        SelectDirector(currentIndex, true);
    }

    private void OnDisable()
    {
        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(PreviousDirector);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextDirector);
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(TogglePlay);
        }

        if (directors != null)
        {
            for (int i = 0; i < directors.Length; i++)
            {
                if (directors[i] != null)
                {
                    directors[i].stopped -= OnDirectorStopped;
                }
            }
        }
    }

    private void PreviousDirector()
    {
        if (directors == null || directors.Length == 0)
        {
            return;
        }

        int newIndex = currentIndex - 1;
        if (newIndex < 0)
        {
            newIndex = directors.Length - 1;
        }

        SelectDirector(newIndex, true);
    }

    private void NextDirector()
    {
        if (directors == null || directors.Length == 0)
        {
            return;
        }

        int newIndex = currentIndex + 1;
        if (newIndex >= directors.Length)
        {
            newIndex = 0;
        }

        SelectDirector(newIndex, true);
    }

    private void TogglePlay()
    {
        PlayableDirector director = GetCurrentDirector();
        if (director == null)
        {
            return;
        }

        if (isFinished)
        {
            RestartDirector(director);
            return;
        }

        if (isPaused)
        {
            ResumeDirector(director);
            UpdatePlayButtonLabel(director);
            return;
        }

        if (director.state == PlayState.Playing)
        {
            PauseDirector(director);
            UpdatePlayButtonLabel(director);
            return;
        }

        if (director.time <= 0d)
        {
            director.time = 0d;
        }

        ResumeDirector(director);
        UpdatePlayButtonLabel(director);
    }

    private void SelectDirector(int index, bool reset)
    {
        currentIndex = Mathf.Clamp(index, 0, directors != null ? directors.Length - 1 : 0);
        isFinished = false;
        isPaused = false;

        if (directors != null)
        {
            for (int i = 0; i < directors.Length; i++)
            {
                if (directors[i] != null)
                {
                    directors[i].Stop();
                    if (reset)
                    {
                        directors[i].time = 0d;
                        directors[i].Evaluate();
                    }
                }
            }
        }

        UpdateEffectLabel();
        UpdatePlayButtonLabel(GetCurrentDirector());
    }

    private void RestartDirector(PlayableDirector director)
    {
        if (director == null)
        {
            return;
        }

        isFinished = false;
        isPaused = false;
        director.Stop();
        director.time = 0d;
        director.Evaluate();
        ResumeDirector(director);
        UpdatePlayButtonLabel(director);
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        if (director != GetCurrentDirector())
        {
            return;
        }

        if (director.duration > 0d && director.time >= director.duration - 0.01d)
        {
            isFinished = true;
            isPaused = false;
            UpdatePlayButtonLabel(director);
        }
    }

    private void UpdateEffectLabel()
    {
        if (effectLabel == null)
        {
            return;
        }

        string effectName = GetEffectName(currentIndex);
        effectLabel.text = "Efecto " + (currentIndex + 1) + ": " + effectName;
    }

    private void UpdatePlayButtonLabel(PlayableDirector director)
    {
        if (playButtonLabel == null)
        {
            return;
        }

        if (director == null)
        {
            playButtonLabel.text = "Play";
            return;
        }

        if (isFinished)
        {
            playButtonLabel.text = "Reiniciar";
            return;
        }

        if (isPaused)
        {
            playButtonLabel.text = "Continuar";
            return;
        }

        if (director.state == PlayState.Playing)
        {
            playButtonLabel.text = "Pausa";
            return;
        }

        playButtonLabel.text = director.time > 0d ? "Continuar" : "Play";
    }

    private void PauseDirector(PlayableDirector director)
    {
        if (director == null)
        {
            return;
        }

        isPaused = true;
        PlayableGraph graph = director.playableGraph;
        if (graph.IsValid())
        {
            graph.GetRootPlayable(0).SetSpeed(0d);
        }
    }

    private void ResumeDirector(PlayableDirector director)
    {
        if (director == null)
        {
            return;
        }

        isPaused = false;
        PlayableGraph graph = director.playableGraph;
        if (graph.IsValid())
        {
            graph.GetRootPlayable(0).SetSpeed(1d);
        }

        director.Play();
    }

    private PlayableDirector GetCurrentDirector()
    {
        if (directors == null || directors.Length == 0)
        {
            return null;
        }

        if (currentIndex < 0 || currentIndex >= directors.Length)
        {
            return null;
        }

        return directors[currentIndex];
    }

    private string GetEffectName(int index)
    {
        if (effectNames == null || index < 0 || index >= effectNames.Length)
        {
            return "Nombre";
        }

        string nameValue = effectNames[index];
        return string.IsNullOrWhiteSpace(nameValue) ? "Nombre" : nameValue;
    }
}
