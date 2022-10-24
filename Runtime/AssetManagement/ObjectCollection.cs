using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace BB.BardicFramework.AssetManagement
{
    public abstract class ObjectCollection : ScriptableObject
    {
        protected abstract void PopulateLookup();
    }

    public abstract class ObjectCollection<T> : ObjectCollection
    {
        [SF] protected List<T> items = default;

        protected Dictionary<string, T> lookup;

        public T this[int index] => items[index];
        public int Count => items.Count;


        public bool Contains(T item) => this.items.Contains(item);

        private void OnValidate()
        {
            if (items == null) Debug.LogWarning("items = null, "+name,this);
        }

        private void OnEnable()
        {
            lookup = new Dictionary<string, T>();
            PopulateLookup();
        }

        public bool LookupContains(string name)
        {
            return lookup.ContainsKey(name);
        }
        public virtual T[] ToArray()=> items.ToArray();

        public T Lookup(string name)
        {
            if (this.lookup == null) OnEnable();
            Debug.Assert(lookup.ContainsKey(name), this.name+ " does not have "+name);
            return lookup[name];
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }
    }
}