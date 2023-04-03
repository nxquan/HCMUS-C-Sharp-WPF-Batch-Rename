
using Contract;

namespace AddPrefix
{
    public class AddPrefixRule : IRule
    {
        public string Name => "AddPrefix";
        public string? Prefix { get; set; } = "";
        public IRule create(string data)
        {
            AddPrefixRule result = new AddPrefixRule();
            var tokens = data.Split(' ');
            result.Prefix = tokens[1];

            return result;
        }

        public string GetParam()
        {
            return Prefix;
        }

        public void PassParam(string firstParam)
        {
            this.Prefix = firstParam;
        }

        public void PassParam(int firstParam, int secondParam, int thirdParam)
        {
            throw new NotImplementedException();
        }

        public bool RefreshParam()
        {
            this.Prefix = "";
            return true;
        }

        public string Rename(string origin)
        {
            string newName = "";
            if (Prefix != null)
            {
                newName = origin.Insert(0, Prefix);
            }
            return newName;
        }
    }
}