using System;
namespace Game_Server.Util.Database
{
    public interface IDatabase<T>
    {
        T ToEntity();
        void FromEntity(T item);
    }
}
