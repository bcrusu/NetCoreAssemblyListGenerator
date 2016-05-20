namespace gen
{
    public interface IApplicationEngine
    {
        void Run(string inDirectory, string outDirectory, string targetFramework, string listFormat);
    }
}
