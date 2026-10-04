namespace TwoGether.Domain.Common;

public static class EntityExtensions
{
    public static void SoftDelete(this Entity entity) => entity.IsDeleted = true;
}
