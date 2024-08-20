using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class EnableDisableHelper : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectsToEnable;
        [SerializeField] private List<GameObject> objectsToDisable;

        private void Start()
        {
            if (objectsToEnable.Count == 0 && objectsToDisable.Count == 0)
            {
                Debug.LogWarning("No objects to enable or disable");
            }
        }
        
        public void EnableObjects()
        {
            foreach (var obj in objectsToEnable)
            {
                obj.SetActive(true);
            }
            
            foreach (var obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
        }
    }
}
