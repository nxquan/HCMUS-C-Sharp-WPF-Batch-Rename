using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using OneSpace;

namespace BatchRename
{
    public class RuleFactory
    {
        public static Dictionary<string, IRule> _prototypes = new Dictionary<string, IRule>();

        static private RuleFactory? _instance = null;
        private RuleFactory()
        {
        }
        static public RuleFactory Instance()
        {
            if (_instance == null)
            {
                _instance = new RuleFactory();
            }
            return _instance;
        }
        static public void register(IRule rule)
        {
            _prototypes.Add(rule.Name, rule);
        }
        public IRule create(string data)
        {
            IRule? item = null;
            var tokens = data.Split(new string[] { " " }, StringSplitOptions.None);
            string key = tokens[0];
            if(_prototypes.ContainsKey(key))
            {
                item = _prototypes[key].create(data);
            }
            return item;
        }
    }
}
