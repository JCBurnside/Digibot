namespace DigiBotExtension
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Used to mark a class a
    /// </summary>
    public class ConnectionAttribute : Attribute
    {
        public string Alias { get; set; }
        public ConnectionAttribute(string alias)
        {
            Alias = alias;
        }
    }
}
