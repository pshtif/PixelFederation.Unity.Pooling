using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixel.Pool
{
    [System.Serializable]
    public class ObjectPool
    {
        private ObjectPoolManager _manager;
        private int _initialSize;
        private Stack<GameObject> _freeStack;
        private List<GameObject> _usedPool;
        private GameObject objectPrototype;

        public int maxSize;
        public bool expandable = true;

        public int GetUsedCount()
        {
            return _usedPool.Count;
        }

        public ObjectPool(ObjectPoolManager p_manager, int p_initialSize, GameObject p_objectPrototype)
        {
            if (p_objectPrototype == null) throw new System.Exception("Cannot use null as pool prototype.");
            _manager = p_manager;
            _initialSize = p_initialSize;
            objectPrototype = p_objectPrototype;
            _freeStack = new Stack<GameObject>();
            _usedPool = new List<GameObject>();

            for (int i = 0; i < _initialSize; ++i)
            {
                CreateInstance();
            }
        }

        private void CreateInstance()
        {
            GameObject instance = Object.Instantiate(objectPrototype);
            instance.SetActive(false);
            _freeStack.Push(instance);
        }

        public GameObject GetInstance()
        {
            GameObject instance = null;
            if (_freeStack.Count == 0 && expandable && (_usedPool.Count < maxSize || maxSize == 0))
            {
                CreateInstance();
            }

            if (_freeStack.Count > 0)
            {
                instance = _freeStack.Pop();
                _usedPool.Add(instance);
            }

            return instance;
        }

        public GameObject GetTimedInstance(bool p_activate, float p_reuseInTime)
        {
            GameObject instance = GetInstance();
            if (instance != null)
            {
                if (p_activate) instance.SetActive(true);
                _manager.StartCoroutine(Reuse(instance, p_reuseInTime));
            }
            return instance;
        }

        IEnumerator Reuse(GameObject p_instance, float p_time)
        {
            yield return new WaitForSeconds(p_time);
            ReturnInstance(p_instance);
        }

        public T GetInstance<T>() where T : Component
        {
            GameObject go = GetInstance();
            return (go != null) ? go.GetComponent<T>() : null;
        }

        public void ReturnInstance(GameObject p_instance)
        {
            p_instance.SetActive(false);
            _usedPool.Remove(p_instance);
            _freeStack.Push(p_instance);
        }

        public void ReturnInstance<T>(T p_instance) where T : Component
        {
            ReturnInstance(p_instance.gameObject);
        }
    }
}