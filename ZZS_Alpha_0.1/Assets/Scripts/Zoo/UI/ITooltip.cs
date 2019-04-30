using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.UI
{
    /// <summary>
    /// Any class with this interface must provide a tooltip string.
    /// </summary>
    public interface ITooltip
    {

        /// <summary>
        /// Returns this object's tooltip as a string.
        /// </summary>
        /// <returns></returns>
        string GetTooltip();


    }
}
