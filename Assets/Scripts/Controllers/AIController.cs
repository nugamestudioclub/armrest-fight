using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : IActionProvider
{
    private List<ActionState> oldQueue;

    private Player player;
    private Player enemy;

    private AIConfig config;
    public List<ActionResponse> ActionResponses => config.actionResponses;

    AIAction curAction = AIAction.EMPTY;

    public AIController(Player player, Player enemy, AIConfig config)
    {
        this.player = player;
        this.enemy = enemy;
        this.config = config;
        oldQueue = new();
    }

    public Action GetNextAction()
    {
        Action toReturn = Action.None;

        List<ActionState> curQueue = enemy.ActionList.Select(action => action.state).ToList();

        if (curAction.Equals(AIAction.EMPTY)) {
            if (player.CurrentActionData.state == ActionState.IDLE) { 
                if (isNewActionTaken(curQueue))
                {
                    OpponentAction action = findOpponentAction(curQueue);
                    curAction = GetReponseAction(action);
                }
            }
        }
        else
        {
            if (curAction.checkDelay())
            {
                toReturn = curAction.Action;
                curAction = AIAction.EMPTY;
            }
        }

        oldQueue = curQueue;
        return toReturn;
    }

    private bool isNewActionTaken(List<ActionState> curQueue) {
        return oldQueue.Count == 0 && curQueue.Count > 0;
    }

    private OpponentAction findOpponentAction(List<ActionState> curQueue)
    {
        if (curQueue[0] == ActionState.COOLDOWN)
        {
            return new OpponentAction(ActionState.COOLDOWN, curQueue.Count);
        }
        for (int i = 0; i < curQueue.Count; i++)
        {
            if (curQueue[i] != ActionState.BUSY)
            {
                return new OpponentAction(curQueue[i], i);
            }
        }
        throw new System.Exception("Something messed up when finding actions");
    }

    private AIAction GetReponseAction(OpponentAction opponentAction)
    {
        for (int i = 0; i < ActionResponses.Count; i++)
        {
            if (ActionResponses[i].ActionState == opponentAction.Action)
            {
                return ActionResponses[i].GetResponse();
            }
        }
        throw new Exception("You forgot a response action didn't you");
    }

}
