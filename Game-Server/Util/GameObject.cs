using System;
namespace Game_Server.Util
{
    /// <summary>
    /// Interface Segregation Principle
    /// Any object that require a identifer can implement this Interface to expose
    /// a unique identifier to the caller
    /// </summary>
    public interface GameObject
    {
        string GetIdentifier();
    }
}
