public class StashBase
{
    public readonly Commands Command;

    public StashBase(Commands command)
    {
        Command = command;
    }

    public static implicit operator StashBase(Commands v)
    {
        return new StashBase(v);
    }

    public static implicit operator Commands(StashBase v)
    {
        return v.Command;
    }
}
