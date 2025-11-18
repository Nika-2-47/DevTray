namespace ConsoleEffects
{
    public interface IConsoleEffect
    {
        string Name { get; }
        string Description { get; }
        void Run();
    }
}
