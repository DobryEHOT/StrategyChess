using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T MainSingleton { get; private set; }

        protected void Inicialize(T obj)
        {
            MainSingleton = obj;
        }

        protected void Inicialize()
        {
            if (this is T t)
                MainSingleton = t;
        }
    }
}
