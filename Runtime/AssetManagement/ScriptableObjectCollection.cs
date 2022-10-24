using BardicBytes.BardicFramework.Core;
using UnityEngine;

namespace BardicBytes.BardicFramework.AssetManagement
{
    namespace Bardic
    {
        public abstract class ScriptableObjectCollection<T> : ObjectCollection<T> where T : ScriptableObject
        {
            protected override void PopulateLookup()
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var useGUID = items[i] is IProvideGUID;
                    var key = useGUID ? ((IProvideGUID)items[i]).GUID : items[i].name;
                    Debug.Assert(key != null, name + ".  index:" + i + ". " + items[i].name);
                    lookup.Add(key, items[i]);
                }
            }

        }
    }
}