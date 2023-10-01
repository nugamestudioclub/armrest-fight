using System;
public class Environment
{
    private EnvironmentConfig config;
    private int position;
    public int Position
    {
        get => position;
        set
        {
            position = Math.Max(-config.edgeDistance, 
                Math.Min(value, config.edgeDistance + config.armrestWidth));
            OnEnvironmentChange(
                new EnvironmentEventArgs(
                    EventSource.ENVIRONEMNT,
                    EnvironmentEventType.PosistionChange,
                    value,
                    config.armrestWidth));
        }
    }
    private readonly int maxLeftPlayerTime;

    private int leftPlayerTime;
    public int LeftPlayerTime
    {
        get => leftPlayerTime;
        set
        {
            leftPlayerTime = value;
            OnEnvironmentChange(
                new EnvironmentEventArgs(
                    EventSource.ENVIRONEMNT,
                    EnvironmentEventType.LeftPlayerTimerUpdated,
                    value,
                    maxLeftPlayerTime));
        }
    }
    private readonly int maxRightPlayerTime;
    private int rightPlayerTime;
    public int RightPlayerTime
    {
        get => rightPlayerTime;
        set
        {
            rightPlayerTime = value;
            OnEnvironmentChange(
                new EnvironmentEventArgs(
                    EventSource.ENVIRONEMNT,
                    EnvironmentEventType.RightPlayerTimerUpdated,
                    value,
                    maxRightPlayerTime));
        }
    }

    public event EventHandler<EnvironmentEventArgs> EnvironmentChangeEvent;

    public Environment(EnvironmentConfig config, int maxLeftPlayerTime, int maxRightPlayerTime)
    {
        this.config = config;
        Position = config.startingPositon;
        this.maxLeftPlayerTime = maxLeftPlayerTime;
        this.maxRightPlayerTime = maxRightPlayerTime;
        LeftPlayerTime = maxLeftPlayerTime;
        RightPlayerTime = maxRightPlayerTime;
    }

    protected virtual void OnEnvironmentChange(EnvironmentEventArgs e)
    {
        EnvironmentChangeEvent?.Invoke(this, e);
    }

}
