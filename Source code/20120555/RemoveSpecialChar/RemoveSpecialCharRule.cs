using Contract;
using System.Text;

namespace RemoveSpecialChar
{
    public class RemoveSpecialCharRule : IRule
    {
        public string Name => "RemoveSpecialChar";
        public List<char> SpecialChars = new List<char>();

        public IRule create(string data)
        {
            var tokens = data.Split(' ');
            RemoveSpecialCharRule result = new RemoveSpecialCharRule();
            var chars = tokens[1].Split(',');
            foreach(var c in chars)
            {
                result.SpecialChars.Add(char.Parse(c));
            }

            return result;
        }

        public string Rename(string origin)
        {
            StringBuilder newName = new StringBuilder();

            foreach(var c in origin)
            {
                if(SpecialChars.Contains(c))
                {
                    newName.Append(' ');
                }
                else
                {
                    newName.Append(c);
                }
            }
            return newName.ToString();
        }

        public void PassParam(string firstParam)
        {
            throw new NotImplementedException();
        }

        public void PassParam(int firstParam, int secondParam, int thirdParam)
        {
            throw new NotImplementedException();
        }

        public bool RefreshParam()
        {
            this.SpecialChars.Clear();
            return true;
        }

        public string GetParam()
        {
            return String.Join(',', SpecialChars.ToArray());
        }
    }
}