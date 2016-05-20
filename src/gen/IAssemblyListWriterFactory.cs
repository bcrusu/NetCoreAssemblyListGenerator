namespace gen
{
    public interface IAssemblyListWriterFactory
    {
        IAssemblyListWriter Create(string name);
    }
}
