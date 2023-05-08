using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.AssetManagement
{
    public class WeightedPrefabRandomizer<ComponentType> where ComponentType : Component
    {
        public delegate bool PrefabValidation(ComponentType prefab);

        private List<WeightedPrefab<ComponentType>> options;
        private float totalWeight;

        public WeightedPrefabRandomizer() : this(optionsArray: null) { }

        public WeightedPrefabRandomizer(WeightedPrefab<ComponentType>[] optionsArray)
        {
            if (optionsArray == null)
                options = new List<WeightedPrefab<ComponentType>>();
            else
                options = new List<WeightedPrefab<ComponentType>>(optionsArray);

            RecalcWeight();
        }

        private void RecalcWeight()
        {
            totalWeight = 0;
            for (int i = 0; i < options.Count; i++)
                totalWeight += options[i].Weight;
        }

        public void Add(WeightedPrefab<ComponentType> newOption)
        {
            options.Add(newOption);
            totalWeight += newOption.Weight;
        }

        public ComponentType GetRandomPrefab()
        {
            return GetRandomPrefab(null);
        }
        public ComponentType GetRandomPrefab(PrefabValidation selectionValidator)
        {
            float r = Random.Range(0f, totalWeight);
            float sum = 0;
            for (int i = 0; i < options.Count; i++)
            {
                sum += options[i].Weight;
                if (options[i].Prefab == null) continue;
                if (r > sum) continue;
                if (selectionValidator == null || selectionValidator(options[i].Prefab))
                {
                    return options[i].Prefab;
                }
            }
            return null;
        }
    }
}
