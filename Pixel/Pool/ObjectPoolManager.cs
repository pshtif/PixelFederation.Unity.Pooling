using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixel.Pool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        static private Dictionary<string, ObjectPool> _pools;
        static private bool _initialized = false;
        static private ObjectPoolManager _instance;

        static public void Initialize()
        {
            Debug.Log("Initialize ObjectPoolManager");
            _initialized = true;
            GameObject core = new GameObject();
            core.name = "ObjectPoolManager";
            core.hideFlags = HideFlags.HideAndDontSave;
            _instance = core.AddComponent<ObjectPoolManager>();
            GameObject.DontDestroyOnLoad(core);

            _pools = new Dictionary<string, ObjectPool>();
        }

        static public ObjectPool AllocatePool(string p_name, GameObject p_objectPrototype, int p_initSize, bool p_expandable, int p_maxSize)
        {
            if (!_initialized) Initialize();
            ObjectPool pool = new ObjectPool(_instance, p_initSize, p_objectPrototype);
            pool.expandable = p_expandable;
            pool.maxSize = p_maxSize;
            _pools.Add(p_name, pool);
            return pool;
        }

        static public ObjectPool GetPool(string p_name)
        {
            if (!_initialized) Initialize();
            return _pools[p_name];
        }
    }
}