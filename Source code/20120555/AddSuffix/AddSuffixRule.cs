using Contract;

namespace AddSuffix
{
    public class AddSuffixRule : IRule
    {
        public string Name => "AddSuffix";
        public string? Suffix { get; set; } = "";
        public IRule create(string data)
        {
            AddSuffixRule result = new AddSuffixRule();
            var tokens = data.Split(' ');
            result.Suffix = tokens[1];

            return result;
        }

        public string GetParam()
        {
            return Suffix;
        }

        public void PassParam(string firstParam)
        {
            this.Suffix = firstParam;
        }

        public void PassParam(int firstParam, int secondParam, int thirdParam)
        {
            throw new NotImplementedException();
        }

        public bool RefreshParam()
        {
            this.Suffix = "";
            return true;
        }

        public string Rename(string origin)
        {
            string newName = "";
            int lastIndexOfDot = origin.LastIndexOf('.');
            if(Suffix != null)
            {
                newName = origin.Insert(lastIndexOfDot, Suffix);
            }
            return newName;
        }
    }
}