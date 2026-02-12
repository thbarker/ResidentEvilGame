using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public RecordsManager recordsManager;
    public EventSystem eventSystem;
    public UIManager UIManager;
    public Image blackScreen;
    public GameObject winScreen;
    public GameObject playAgain;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI completionTime;
    public TextMeshProUGUI records;
    public AudioSource audioSource, musicSource;
    public AudioClip music, door, typewriter;

    private void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        recordsManager = GameObject.Find("Records Manager")?.GetComponent<RecordsManager>();
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();
        UIManager = GameObject.Find("UIManager")?.GetComponent<UIManager>();
        blackScreen = transform.Find("BlackScreen")?.GetComponent<Image>();
        winScreen = transform.Find("WinImage").gameObject;
        rank = transform.Find("WinImage/Text Panel/Rank")?.GetComponent<TextMeshProUGUI>();
        completionTime = transform.Find("WinImage/Text Panel/Completion Time")?.GetComponent<TextMeshProUGUI>();
        records = transform.Find("WinImage/Text Panel/Records")?.GetComponent<TextMeshProUGUI>();
        playAgain = transform.Find("WinImage/Play Again")?.gameObject;
        

        // Ensure black screen starts fully transparent
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
        }

        if (winScreen != null)
            winScreen.SetActive(false);
    }

    public void WinGame()
    {
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        eventSystem.SetSelectedGameObject(playAgain);
        audioSource.PlayOneShot(door);
        musicSource.PlayOneShot(music);
        audioSource.PlayOneShot(typewriter);

        UIManager.StartUI();
        playerMovement.StateMachine.ChangeState(playerMovement.WinState);

        // Fade black screen and music in
        yield return StartCoroutine(FadeBlackScreen(0f, 1f, 2f, true));


        // Activate win screen
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            switch (recordsManager.rank)
            {
                case Ranks.SS:
                    rank.text = "S+";
                    break;
                default:
                    rank.text = recordsManager.rank.ToString();
                    break;
            }
            string timeString = System.TimeSpan.FromSeconds(recordsManager.gameTimer).ToString(@"mm\:ss\:fff");
            completionTime.text = timeString;
            int zombiesKilled = recordsManager.zombiesKilled;
            int timesBitten = recordsManager.timesBitten;
            int healsUsed = recordsManager.timesHealed;
            records.text = "Zombies Killed: " + zombiesKilled + 
                "\r\nTimes Bitten: " + timesBitten + 
                "\r\nHeals Used: " + healsUsed;
        }

        // Fade black screen out
        yield return StartCoroutine(FadeBlackScreen(1f, 0f, 4f, false));
    }

    private IEnumerator FadeBlackScreen(float startAlpha, float endAlpha, float duration, bool music)
    {
        float elapsed = 0f;

        if (blackScreen == null)
            yield break;

        Color color = blackScreen.color;
        color.a = startAlpha;
        blackScreen.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            color.a = alpha;
            if(music)
                musicSource.volume = alpha;
            blackScreen.color = color;
            yield return null;
        }

        // Ensure final alpha is exact
        color.a = endAlpha;
        blackScreen.color = color;
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}