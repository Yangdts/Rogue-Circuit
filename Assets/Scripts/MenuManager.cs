using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [Header ("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button storyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private Button backButton;

    [Header ("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    [Header ("Text Effects")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private float hoverScale = 1.2f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        startButton.onClick.AddListener(StartGame);
        storyButton.onClick.AddListener(ShowStory);
        exitButton.onClick.AddListener(ExitGame);

        AddHoverEffect(startButton);
        AddHoverEffect(storyButton);
        AddHoverEffect(exitButton);

        if (backButton != null) AddHoverEffect(backButton);

        if (storyPanel != null) storyPanel.SetActive(false);
    }

    private void AddHoverEffect(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            buttonText.color = hoverColor;
            buttonText.transform.localScale = Vector3.one * hoverScale;
            PlaySound(hoverSound);
        });

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => {
            buttonText.color = normalColor;
            buttonText.transform.localScale = Vector3.one;
        });

        button.gameObject.AddComponent<EventTrigger>().triggers.Add(pointerEnter);
        button.gameObject.AddComponent<EventTrigger>().triggers.Add(pointerExit);
    }

    private void StartGame()
    {
        PlaySound(clickSound);
        SceneManager.LoadScene(1);
    }

    private void ShowStory()
    {
        PlaySound(clickSound);
        if (storyPanel != null)
        { 
            storyPanel.SetActive(true); 

            if (backButton != null) backButton.gameObject.SetActive(true);
        }
    }

    public void CloseStoryPanel()
    {
        PlaySound(clickSound);
        if (storyPanel != null) storyPanel.SetActive(false);
        if (backButton !=null) backButton.gameObject.SetActive(false);

    }

    private void ExitGame()
    {
        PlaySound(clickSound);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
