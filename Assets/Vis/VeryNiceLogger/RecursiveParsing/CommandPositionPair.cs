public struct CommandPositionPair
{
    public int Position;
    public Commands Command;

    public CommandPositionPair(Commands command)
    {
        Command = command;
        Position = default;
    }

    public CommandPositionPair(int position, Commands command)
    {
        Position = position;
        Command = command;
    }
}
