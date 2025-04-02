using UnityEngine;

namespace Utilities.GeneralUtilities
{
    public class RandomShowObject : MonoBehaviour
    {
        [SerializeField] private float probability = 1;

        private void Awake()
        {
            var sortedNumber = Random.Range(0f, 100f);

            if (probability < sortedNumber)
            {
                gameObject.SetActive(false);
            }
        }
    }

}


