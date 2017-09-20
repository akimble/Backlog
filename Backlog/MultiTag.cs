using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backlog
{
    class MultiTag
    {
        public Dictionary<string, object> TagDictionary { get; set; }

        public MultiTag()
        {
            this.TagDictionary = new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            this.TagDictionary.Add(key, value);
        }

        public object Get(string key)
        {
            return this.TagDictionary[key];
        }

        public void Update(string key, object value)
        {
            this.TagDictionary[key] = value;
        }
    }
}
