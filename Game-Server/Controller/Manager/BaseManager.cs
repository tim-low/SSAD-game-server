using System;
namespace Game_Server.Controller.Manager
{
    public interface BaseManager<T>
    {
        bool Get(string identifier, out T obj);
    }
}
