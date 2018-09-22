namespace DigiBotExtension
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Used to mark a class as an IRC connection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IrcConnectionAttribute : Attribute
    {
        public string Alias { get; private set; }
        public IrcConnectionAttribute(string alias)
        {
            Alias = alias;
        }
    }
}
