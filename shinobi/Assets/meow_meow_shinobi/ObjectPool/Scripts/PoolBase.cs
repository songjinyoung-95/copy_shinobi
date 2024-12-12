using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Pool
{
    public interface IObjectPool<T> where T : Component
    {
        Transform Parent { get; }

        void Generator(T origin, string type, int count);

        T Spawn(string type, Vector3 pos);
        T Spawn(string type, Vector3 pos, Quaternion rot);
        T Spawn(string type, Vector3 pos, Quaternion rot, Transform parent);

        void Despawn(T pool, string type);
    }

    /// <summary>
    /// 오브젝트 풀 베이스
    /// </summary>
    public class PoolBase<T> : IObjectPool<T> where T : Component
    {
        /// -------------------
        /// private
        /// -------------------
        
        private Dictionary<string, Queue<T>> _dict;
        private Dictionary<string, T> _originDict;


        /// -------------------
        /// get, set
        /// -------------------
        /// 
        public Transform Parent { get; }



        public PoolBase(Transform parent)
        {
            _dict       = new Dictionary<string, Queue<T>>();
            _originDict = new Dictionary<string, T>();
            Parent      = parent;
        }

        public void Generator(T origin, string type, int count)
        {
            if(!origin.TryGetComponent<T>(out var component))
                return;

            if(!_originDict.ContainsKey(type))
                _originDict.Add(type, origin);

            if(!_dict.ContainsKey(type))
                _dict.Add(type, new Queue<T>());

            for (int i = 0; i < count; i++)
            {
                T newObject = Object.Instantiate(component, Parent);
                newObject.gameObject.SetActive(false);

                _dict[type].Enqueue(newObject);
            }
        }

        private T _Spawn(string type)
        {
            if (_dict[type].Count <= 0)
                Generator(_originDict[type], type, 5);

            T component = _dict[type].Dequeue();

            return component;
        }

        public T Spawn(string type, Vector3 pos)
        {
            T component     = _Spawn(type);
            GameObject obj  = component.gameObject;

            obj.transform.position = pos;

            return component;
        }

        public T Spawn(string type, Vector3 pos, Quaternion rot)
        {
            T component     = _Spawn(type);
            GameObject obj  = component.gameObject;

            obj.transform.position = pos;
            obj.transform.rotation = rot;

            return component;
        }

        public T Spawn(string type, Vector3 pos, Quaternion rot, Transform parent)
        {
            T component     = _Spawn(type);
            GameObject obj  = component.gameObject;

            obj.transform.position  = pos;
            obj.transform.rotation  = rot;
            obj.transform.parent    = parent;

            return component;      
        }

        public void Despawn(T pool, string type)
        {
            pool.gameObject.SetActive(false);
            _dict[type].Enqueue(pool);
        }
    }
}