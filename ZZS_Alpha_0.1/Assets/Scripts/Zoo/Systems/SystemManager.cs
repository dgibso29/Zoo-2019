using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Systems
{
    /// <summary>
    /// Base for all Manager-type classes. Mandates a static, self-referencing Instance property.
    /// </summary>
    public abstract class SystemManager<T> : MonoBehaviour
    {
        /// <summary>
        /// Static reference to the active instance of the manager.
        /// </summary>
        public static T Instance;

        protected virtual void Awake()
        {
            Instance = GetComponent<T>();
            
        }
    }
}
