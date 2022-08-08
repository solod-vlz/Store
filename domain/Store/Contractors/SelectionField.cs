using System.Collections.Generic;

namespace Store.Contractors
{
   public class SelectionField: Field
    {
        public IReadOnlyDictionary<string, string> Items;
        
        public SelectionField(string label, string name, string value, IReadOnlyDictionary<string, string> items)
            : base(label, name, value)
        {
            Items = items;
        }
    }
}
