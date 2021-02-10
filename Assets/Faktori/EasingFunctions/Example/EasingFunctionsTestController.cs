using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Faktori.EasingFunctions.Examples {
    public class EasingFunctionsTestController : MonoBehaviour
    {
        public EasingFunctionDisplayer displayerPrefab;
        private List<EasingFunctionDisplayer> _displayers = new List<EasingFunctionDisplayer>();

        public IEnumerator Start()
        {
            foreach(Easing.Functions easing in Enum.GetValues(typeof(Easing.Functions)))
            {
                _displayers.Add(Instantiate(displayerPrefab, Vector3.zero, Quaternion.identity, transform));
            }

            yield return null;

            for(int i = 0; i < _displayers.Count; i++)
            {
                Easing.Functions[] values = Enum.GetValues(typeof(Easing.Functions)) as Easing.Functions[];
                _displayers[i].Init(values[i]);
            }
        }
    }
}