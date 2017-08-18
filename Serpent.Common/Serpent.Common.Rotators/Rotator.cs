namespace Serpent.Common.Rotators
{
    using System.Collections;
    using System.Collections.Generic;

    public abstract class Rotator<T>
    {
        public abstract T GetNext();
    }   
}