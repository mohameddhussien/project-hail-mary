using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : StartFullScreen
{
    [SerializeField] private UIDocument _document;

    // Optional: assign your ship sprite in the Inspector
    [SerializeField] private Sprite shipSprite;

    private static readonly WaitForSeconds _waitForSeconds0_25 = new(0.25f), _waitForSeconds0_15 = new(0.15f);

    // Animation state
    private VisualElement _shipContainer;
    private VisualElement _shipBody;
    private VisualElement _shipHud;
    private VisualElement _titleAccentLine;
    private VisualElement[] _trailDots;

    private Coroutine _floatCoroutine;
    private Coroutine _enginePulseCoroutine;
    private bool _shipHovered = false;

    void OnEnable()
    {
        var root = _document.rootVisualElement;

        // ── Wire navigation buttons ──────────────────────────────
        root.Q<Button>("playBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Gameplay"));
        root.Q<Button>("storyBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Intro"));
        root.Q<Button>("settingsBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Settings"));
        root.Q<Button>("exitBtn").RegisterCallback<ClickEvent>(OnExitButtonPressed);

        // ── Cache ship elements ──────────────────────────────────
        _shipContainer = root.Q<VisualElement>("ship-container");
        _shipBody = root.Q<VisualElement>("ship-body");
        _shipHud = root.Q<VisualElement>("ship-hud");
        _titleAccentLine = root.Q<VisualElement>("title-accent-line");

        _trailDots = new VisualElement[]
        {
            root.Q<VisualElement>("trail-dot--1"),
            root.Q<VisualElement>("trail-dot--2"),
            root.Q<VisualElement>("trail-dot--3"),
        };

        _shipContainer.RegisterCallback<MouseEnterEvent>(_ =>
        {
            _shipHud.AddToClassList("visible");
        });

        _shipContainer.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            _shipHud.RemoveFromClassList("visible");
        });

        // ── Assign ship sprite if provided ───────────────────────
        if (shipSprite != null && _shipBody != null)
            _shipBody.style.backgroundImage = new StyleBackground(shipSprite);

        // ── Scanline overlay: ignore pointer events ──────────────
        var scanlines = root.Q<VisualElement>("scanlines");
        if (scanlines != null)
            scanlines.pickingMode = PickingMode.Ignore;

        // ── Ship hover interactions ──────────────────────────────
        if (_shipContainer != null)
        {
            _shipContainer.RegisterCallback<PointerEnterEvent>(OnShipPointerEnter);
            _shipContainer.RegisterCallback<PointerLeaveEvent>(OnShipPointerLeave);
            _shipContainer.RegisterCallback<ClickEvent>(OnShipClicked);
        }

        // ── Startup animations ───────────────────────────────────
        StartCoroutine(PlayIntroSequence(root));
    }

    void OnDisable()
    {
        if (_floatCoroutine != null) StopCoroutine(_floatCoroutine);
        if (_enginePulseCoroutine != null) StopCoroutine(_enginePulseCoroutine);
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
        if (_shipContainer != null)
        {
            _shipContainer.style.opacity = 0;
            _shipContainer.style.scale = new StyleScale(new Scale(new Vector3(0.88f, 0.88f, 1f)));
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

        yield return new WaitForSeconds(0.25f);

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

        yield return new WaitForSeconds(0.3f);

        // Fade in ship
        if (_shipContainer != null)
        {
            _shipContainer.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> {
                    new("opacity"),
                    new("scale")
                });
            _shipContainer.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> {
                    new(0.7f, TimeUnit.Second),
                    new(0.7f, TimeUnit.Second)
                });
            _shipContainer.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new System.Collections.Generic.List<EasingFunction> {
                    new(EasingMode.EaseOut),
                    new(EasingMode.EaseOut)
                });
            _shipContainer.style.opacity = 1;
            _shipContainer.style.scale = new StyleScale(new Scale(Vector3.one));
        }

        // Start idle float loop
        yield return new WaitForSeconds(0.8f);
        _floatCoroutine = StartCoroutine(ShipFloatLoop());
        _enginePulseCoroutine = StartCoroutine(EnginePulseLoop());
    }

    // ── Gentle float animation (bob up/down) ─────────────────────
    private IEnumerator ShipFloatLoop()
    {
        float elapsed = 0f;
        const float period = 3.4f;   // seconds per full cycle
        const float amplitude = 10f;    // pixels

        while (true)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Sin(elapsed / period * Mathf.PI * 2f) * amplitude;

            if (_shipContainer != null && !_shipHovered)
                _shipContainer.style.translate = new StyleTranslate(new Translate(0, offset));

            yield return null;
        }
    }

    // ── Engine trail pulse (opacity flicker on trail dots) ───────
    private IEnumerator EnginePulseLoop()
    {
        if (_trailDots == null) yield break;

        float[] phases = { 0f, 1.1f, 2.2f };   // stagger offsets
        const float speed = 2.5f;
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < _trailDots.Length; i++)
            {
                if (_trailDots[i] == null) continue;
                float alpha = (Mathf.Sin((elapsed + phases[i]) * speed) + 1f) * 0.5f;
                // Remap: dot 0 is brightest, dot 2 is dimmest
                float maxAlpha = 0.9f - i * 0.3f;
                _trailDots[i].style.opacity = alpha * maxAlpha;
            }

            yield return null;
        }
    }

    // ── Ship pointer interactions ─────────────────────────────────
    private void OnShipPointerEnter(PointerEnterEvent e)
    {
        _shipHovered = true;

        // Snap float position to 0 and let USS hover rule handle lift
        if (_shipContainer != null)
            _shipContainer.style.translate = new StyleTranslate(new Translate(0, 0));

        // Reveal HUD: USS sibling selector workaround — toggle class
        if (_shipHud != null)
        {
            _shipHud.style.opacity = 1;
            _shipHud.style.translate = new StyleTranslate(new Translate(0, -6));
        }

        // Brighten border
        if (_shipBody != null)
            _shipBody.AddToClassList("ship-body--active");
    }

    private void OnShipPointerLeave(PointerLeaveEvent e)
    {
        _shipHovered = false;

        if (_shipHud != null)
        {
            _shipHud.style.opacity = 0;
            _shipHud.style.translate = new StyleTranslate(new Translate(0, 0));
        }

        if (_shipBody != null)
            _shipBody.RemoveFromClassList("ship-body--active");
    }

    private void OnShipClicked(ClickEvent e)
    {
        // Optional: play a thruster burst animation then load a ship
        // customisation or lore scene, or simply a satisfying visual pop
        StartCoroutine(ShipThrusterBurst());
    }

    private IEnumerator ShipThrusterBurst()
    {
        if (_shipContainer == null) yield break;

        // Scale pop
        _shipContainer.style.transitionDuration = new StyleList<TimeValue>(
            new System.Collections.Generic.List<TimeValue> {
                new(0.12f, TimeUnit.Second),
                new(0.12f, TimeUnit.Second)
            });
        _shipContainer.style.scale = new StyleScale(new Scale(new Vector3(1.08f, 1.08f, 1f)));

        yield return new WaitForSeconds(0.13f);

        _shipContainer.style.scale = new StyleScale(new Scale(Vector3.one));

        // Briefly brighten all trail dots
        foreach (var dot in _trailDots)
            if (dot != null) dot.style.opacity = 1f;

        yield return _waitForSeconds0_25;
    }
}