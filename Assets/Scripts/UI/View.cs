using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    [SerializeField]
    private Slider leftStamina;

    [SerializeField]
    private TMPro.TMP_Text leftTimer;

    [SerializeField]
    private Slider rightStamina;

    [SerializeField]
    private TMPro.TMP_Text rightTimer;

    [SerializeField]
    private Transform positionIndicator;
    private Vector3 startingPositionIndicatorPosition;
    private Quaternion startingPositionIndicatorRotation;

    [SerializeField]
    private SpriteRenderer leftSpriteRenderer;

    [SerializeField]
    private SpriteRenderer rightSpriteRenderer;

    [SerializeField]
    private ArmViewPosition armView;

    [SerializeField]
    private GameObject leftOnScreenControls;

    [SerializeField]
    private GameObject rightOnScreenControls;

    private readonly System.Random random = new();

    [SerializeField]
    private Bubble leftBubble;
    [SerializeField]
    private Bubble rightBubble;
    [SerializeField]
    private Bubble centerBubble;

    private Dictionary<Action, KeyValuePair<List<BubbleConfig>, float>> actionVfx;

    private Dictionary<RefereeEventType, KeyValuePair<List<BubbleConfig>, float>> interactionVfx;

    private Dictionary<PlayerState, KeyValuePair<List<BubbleConfig>, float>> stateVfx;

    void Awake()
    {
        startingPositionIndicatorPosition = positionIndicator.position;
        startingPositionIndicatorRotation = positionIndicator.rotation;
    }

    void Start()
    {
        var gameInProgress = GameInProgress.Instance;
        leftSpriteRenderer.sprite = gameInProgress.LeftPlayer.idleSprite;
        rightSpriteRenderer.sprite = gameInProgress.RightPlayer.idleSprite;
        leftOnScreenControls.SetActive(gameInProgress.ShowOnScreenControls);
        rightOnScreenControls.SetActive(gameInProgress.ShowOnScreenControls && gameInProgress.PlayerCount > 1);
    }

    public void Bind(Referee referee, Environment environment, Player leftPlayer, Player rightPlayer, SpecialEffectConfig specialEffects)
    {
        referee.RefereeEvent += Referee_OnInteraction;
        environment.EnvironmentChangeEvent += Environment_OnChange;
        leftPlayer.PlayerEvent += Player_OnChange;
        leftPlayer.PlayerTickEvent += Player_OnTick;
        rightPlayer.PlayerEvent += Player_OnChange;
        rightPlayer.PlayerTickEvent += Player_OnTick;

        actionVfx = new(specialEffects.actions.Select(x =>
            new KeyValuePair<Action, KeyValuePair<List<BubbleConfig>, float>>
            (x.action, new(new(x.specialEffect.vfx), x.specialEffect.vfxDuration))
        ));

        interactionVfx = new(specialEffects.interactions.Select(x =>
            new KeyValuePair<RefereeEventType, KeyValuePair<List<BubbleConfig>, float>>
            (x.interaction, new(new(x.specialEffect.vfx), x.specialEffect.vfxDuration))
        ));

        stateVfx = new(specialEffects.states.Select(x =>
            new KeyValuePair<PlayerState, KeyValuePair<List<BubbleConfig>, float>>
            (x.state, new(new(x.specialEffect.vfx), x.specialEffect.vfxDuration))
        ));
    }



    public void HandleBlockDown0() => HandleButton(Button.Block, 0, true);
    public void HandleBlockDown1() => HandleButton(Button.Block, 1, true);
    public void HandleBlockUp0() => HandleButton(Button.Block, 0, false);
    public void HandleBlockUp1() => HandleButton(Button.Block, 1, false);

    public void HandleDodgeDown0() => HandleButton(Button.Dodge, 0, true);
    public void HandleDodgeDown1() => HandleButton(Button.Dodge, 1, true);

    public void HandlePushDown0() => HandleButton(Button.Push, 0, true);
    public void HandlePushDown1() => HandleButton(Button.Push, 1, true);
    public void HandlePushUp0() => HandleButton(Button.Push, 0, false);
    public void HandlePushUp1() => HandleButton(Button.Push, 1, false);

    public void HandleShoveDown0() => HandleButton(Button.Shove, 0, true);
    public void HandleShoveDown1() => HandleButton(Button.Shove, 1, true);
    private void Referee_OnInteraction(object sender, RefereeEventArgs e)
    {
        Debug.Log($"ref event {e.type}");
        if (interactionVfx.TryGetValue(e.type, out KeyValuePair<List<BubbleConfig>, float> vfx) && vfx.Key.Count > 0)
        {
            Debug.Log("inside!!!");
            ViewSide side = GetViewSide(e.sender);
            List<BubbleConfig> bubbleConfig = vfx.Key;
            float duration = vfx.Value;
            if (vfx.Key.Count == 1) //play center
            {
                centerBubble.Set(bubbleConfig[0]);
                centerBubble.Play(duration);
            }
            else //play left and right
            {
                leftBubble.Set(side == ViewSide.Left ? bubbleConfig[0] : bubbleConfig[1]);
                leftBubble.Play(duration);
                rightBubble.Set(side == ViewSide.Left ? bubbleConfig[1] : bubbleConfig[0]);
                rightBubble.Play(duration);
            }
        }
    }
    private void Environment_OnChange(object sender, EnvironmentEventArgs e)
    {
        if (e.type == EnvironmentEventType.PosistionChange)
        {
            //move players
            SetPosition(e.value / (float)e.maxValue);

        }
        else
        {
            SetPlayerTimer(GetViewSide(e.type), e.value);
        }
    }

    private void Player_OnChange(object sender, PlayerEventArgs e)
    {
        ViewSide side = GetViewSide(e.sender);

        SetPlayerStamina(side, e.stamina / (float)e.maxStamina);

        if (actionVfx.TryGetValue(e.action, out KeyValuePair<List<BubbleConfig>, float> vfx) && vfx.Key.Count > 0)
        {
            List<BubbleConfig> bubbleConfig = vfx.Key;
            float duration = vfx.Value;
            if (vfx.Key.Count == 1) //play center
            {
                centerBubble.Set(bubbleConfig[0]);
                centerBubble.Play(duration);
            }
            else //play left and right
            {
                leftBubble.Set(side == ViewSide.Left ? bubbleConfig[0] : bubbleConfig[1]);
                leftBubble.Play(duration);
                rightBubble.Set(side == ViewSide.Left ? bubbleConfig[1] : bubbleConfig[0]);
                rightBubble.Play(duration);
            }
        }
    }

    private void Player_OnTick(object sender, PlayerTickEventArgs e)
    {
        (e.sender switch
        {
            EventSource.LEFT => leftSpriteRenderer,
            EventSource.RIGHT => rightSpriteRenderer,
            _ => throw new InvalidOperationException()
        }).sprite = e.actionFrameData.sprite;
    }

    private ViewSide GetViewSide(EventSource source)
    {
        return source switch
        {
            EventSource.LEFT => ViewSide.Left,
            EventSource.RIGHT => ViewSide.Right,
            _ => ViewSide.None
        };
    }

    private ViewSide GetViewSide(EnvironmentEventType type)
    {
        return type switch
        {
            EnvironmentEventType.LeftPlayerTimerUpdated => ViewSide.Left,
            EnvironmentEventType.RightPlayerTimerUpdated => ViewSide.Right,
            _ => ViewSide.None
        };
    }

    private void SetPlayerTimer(ViewSide side, int value)
    {
        (side switch
        {
            ViewSide.Left => leftTimer,
            ViewSide.Right => rightTimer,
            _ => throw new ArgumentOutOfRangeException(nameof(side))
        }).text = value.ToString();
    }

    private void SetPlayerStamina(ViewSide side, float value)
    {
        (side switch
        {
            ViewSide.Left => leftStamina,
            ViewSide.Right => rightStamina,
            _ => throw new ArgumentOutOfRangeException(nameof(side))
        }).value = value;
    }

    private void SetPosition(float value)
    {
        armView.UpdateTransform(value);

        positionIndicator.position = startingPositionIndicatorPosition;
        positionIndicator.rotation = startingPositionIndicatorRotation;

        float valueConstrained = Math.Max(0, Math.Min(value, 1));
        positionIndicator.Rotate(0, 0, (valueConstrained * 180) - 90);
    }

    private void HandleButton(Button button, int playerId, bool isDown)
    {
        Debug.Log($"{button} ({playerId})");
        var input = playerId == 0
            ? GameInProgress.Instance.LeftInput
            : GameInProgress.Instance.RightInput;
        input.InputData.SetButtonState(button, isDown);
    }
}