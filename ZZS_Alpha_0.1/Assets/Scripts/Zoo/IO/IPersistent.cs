using System;

namespace Zoo.IO
{
    public interface IPersistent
    {

        /// <summary>
        /// Initialize object after deserialization or instantiation.
        /// </summary>
        void InitializeFromSave();


    }
}
