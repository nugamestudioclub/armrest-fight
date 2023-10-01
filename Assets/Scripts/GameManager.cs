using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //referee
    private Referee referee;
    //sounds
    [SerializeField]
    private AudioHandler audioManager;
    //ui
    [SerializeField]
    private View view;
    //logic
    private GameLogic logic;


    [SerializeField]
    private GameConfig config;


    private static GameManager _instance;

    [SerializeField]
    private InputController _inputController;

    private HumanController humanController;

    private Player leftPlayer;

    private Player rightPlayer;

    private int frameCount;

    public static GameManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        int currentFrameCount = Time.frameCount;
        if (currentFrameCount > frameCount)
        {
            var action = humanController.GetNextAction();
            if (action != Action.None)
            {
                // Debug.Log(action);
            }
            frameCount = currentFrameCount;
        }
        leftPlayer.Update();
        rightPlayer.Update();
    }

    private void FixedUpdate()
    {
        //we can add conditionals here
        referee.Tick();
    }

    private void Initialize()
    {
        //create player and environment from the configs
        leftPlayer = new Player(
            config.leftPlayerConfig.shove,
            config.leftPlayerConfig.push,
            config.leftPlayerConfig.dodge,
            config.leftPlayerConfig.block,
            config.leftPlayerConfig.maxStamina,
            EventSource.LEFT);

        rightPlayer = new Player(
            config.rightPlayerConfig.shove,
            config.rightPlayerConfig.push,
            config.rightPlayerConfig.dodge,
            config.rightPlayerConfig.block,
            config.rightPlayerConfig.maxStamina,
            EventSource.RIGHT);

        _inputController.Initialize();
        humanController = new(_inputController);
        var aiController = new AIController(rightPlayer, leftPlayer, config.aIConfig);

        leftPlayer.Bind(humanController);
        rightPlayer.Bind(aiController);

        Environment environment = new Environment(
            config.environmentConfig.startingPositon,
            config.environmentConfig.armrestWidth,
            config.leftPlayerConfig.health,
            config.rightPlayerConfig.health);

        referee = new Referee(leftPlayer, rightPlayer, environment);
        logic = new GameLogic(config, leftPlayer, rightPlayer, environment);
        logic.Bind(referee);

        view.Bind(environment, leftPlayer, rightPlayer);
        audioManager.Bind(referee, leftPlayer, rightPlayer, environment);
    }
}