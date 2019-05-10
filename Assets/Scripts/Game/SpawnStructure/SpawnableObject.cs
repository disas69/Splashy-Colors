using System;
using Framework.Extensions;
using UnityEngine;

namespace Game.SpawnStructure
{
    public class SpawnableObject : MonoBehaviour
    {
        public event Action<SpawnableObject> Deactivated;

        public virtual void Deactivate()
        {
            Deactivated.SafeInvoke(this);
        }
    }
}