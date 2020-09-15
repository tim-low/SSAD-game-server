using System;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class Status : GameObject
    {
        private GameObject Object;

        public void Update(GameObject Object)
        {
            this.Object = Object;
        }

        /// <summary>
        /// GameObject Type in which this Status object contain
        /// </summary>
        /// <returns>the type of GameObject we stored under Status</returns>
        public Type GetState()
        {
            if (Object == null)
                return null;
            return this.Object.GetType();
        }

        public virtual string GetIdentifier()
        {
            if (Object == null)
                return "";
            return this.Object.GetIdentifier();
        }

        public T GetObject<T>()
        {
            return (T)Object;
        }
    }
}
