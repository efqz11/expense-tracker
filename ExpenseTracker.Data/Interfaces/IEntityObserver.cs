namespace ExpenseTracker.Interfaces;

public interface IEntityObserver
{
    void EntityCreated(object entity);
    void EntityUpdated(object oldEntity, object newEntity);
    void EntityDeleted(object entity);
    bool CheckType(object entity);
}
