public interface IResourceConsumer
{
    bool TryConsume(ResourceInventory inventory);
}
