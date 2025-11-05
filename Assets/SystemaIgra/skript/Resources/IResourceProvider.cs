public interface IResourceProvider
{
    ResourceType ResourceType { get; }
    int Amount { get; }
    void Collect();
}