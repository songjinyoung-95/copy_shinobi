using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Util
{
    public static class CacheCollider<T>
    {
        private static Dictionary<Collider2D, T> _dict = new Dictionary<Collider2D, T>();

        public static T TryGetComponenet(Collider2D collider2D)
        {
            if(_dict.ContainsKey(collider2D))
            {
                if (_dict.TryGetValue(collider2D, out var component))
                {
                    return component;
                }
            }

            if(!_dict.ContainsKey(collider2D))
            {
                if(collider2D.TryGetComponent<T>(out var component))
                {
                    _dict.Add(collider2D, component);
                    return component;
                }
            }

            return default(T);
        }

        public static void Release() => _dict.Clear();
    }
}