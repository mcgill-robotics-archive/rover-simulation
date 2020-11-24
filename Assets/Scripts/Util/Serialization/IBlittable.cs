using System;

namespace roverstd
{
    /// <summary>
    /// This is a Curiously Recurring Template Pattern (CRTP) interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBlittable<T> where T : unmanaged
    {
        
    }
}