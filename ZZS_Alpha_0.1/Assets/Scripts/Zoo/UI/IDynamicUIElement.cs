using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.UI
{
    /// <summary>
    /// Intended for any custom/dynamic UI element scripts.
    /// </summary>
    public interface IDynamicUIElement
    {


        /// <summary>
        /// Set all required component references for this object.
        /// </summary>
        void SetComponentReferences();

        /// <summary>
        /// Update dynamic UI components controlled by this object.
        /// </summary>
        void UpdateElementData();

    }
}
