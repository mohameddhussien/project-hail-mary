using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : StartFullScreen
{
    private static WaitForSeconds _waitForSeconds0_13 = new WaitForSeconds(0.13f);
    private static WaitForSeconds _waitForSeconds0_8 = new WaitForSeconds(0.8f);
    private static WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);
    [SerializeField] private UIDocument _document;

    // Optional: assign your ship sprite in the Inspector
    [SerializeField] private Sprite shipSprite;

    private static readonly WaitForSeconds _waitForSeconds0_25 = new(0.25f), _waitForSeconds0_15 = new(0.15f);

    // Animation state
    private VisualElement _titleAccentLine;

    void OnEnable()
    {
        var root = _document.rootVisualElement;

        // ── Wire navigation buttons ──────────────────────────────
        root.Q<Button>("playBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Gameplay"));
        root.Q<Button>("storyBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Intro"));
        root.Q<Button>("settingsBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Settings"));
        root.Q<Button>("exitBtn").RegisterCallback<ClickEvent>(OnExitButtonPressed);

        // ── Cache ship elements ──────────────────────────────────
        _titleAccentLine = root.Q<VisualElement>("title-accent-line");

        // ── Scanline overlay: ignore pointer events ──────────────
        var scanlines = root.Q<VisualElement>("scanlines");
        if (scanlines != null)
            scanlines.pickingMode = PickingMode.Ignore;

        // ── Startup animations ───────────────────────────────────
        StartCoroutine(PlayIntroSequence(root));
    }

    // ── Exit ─────────────────────────────────────────────────────
    public void OnExitButtonPressed(ClickEvent e)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Intro sequence: stagger-reveal menu elements ─────────────
    private IEnumerator PlayIntroSequence(VisualElement root)
    {
        // Start invisible
        var leftPanel = root.Q<VisualElement>("left-panel");
        if (leftPanel != null)
        {
            leftPanel.style.opacity = 0;
            leftPanel.style.translate = new StyleTranslate(new Translate(-30, 0));
        }

        yield return _waitForSeconds0_15;

        // Fade in left panel
        if (leftPanel != null)
        {
            leftPanel.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> {
                    new("opacity"),
                    new("translate")
                });
            leftPanel.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> {
                    new(0.55f, TimeUnit.Second),
                    new(0.55f, TimeUnit.Second)
                });
            leftPanel.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new System.Collections.Generic.List<EasingFunction> {
                    new(EasingMode.EaseOut),
                    new(EasingMode.EaseOut)
                });
            leftPanel.style.opacity = 1;
            leftPanel.style.translate = new StyleTranslate(new Translate(0, 0));
        }

        yield return _waitForSeconds0_25;

        // Animate title accent line expanding
        if (_titleAccentLine != null)
        {
            _titleAccentLine.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> {
                    new("width")
                });
            _titleAccentLine.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> {
                    new(0.6f, TimeUnit.Second)
                });
            _titleAccentLine.style.width = 80;
        }

        yield return _waitForSeconds0_3;
    }
}