using Cosmos.Infrastructure;
using UnityEngine;
using VContainer;

namespace Cosmos.Test
{
    public class Test : MonoBehaviour
    {
        [Inject]
        private void InjectDependency(UpdateRunner updateRunner)
        {
            if (updateRunner == null)
            {
                Debug.Log("updateRunner is null");
            }
            else
            {
                Debug.Log("updateRunner is not null");
            }
        }
    }

}
