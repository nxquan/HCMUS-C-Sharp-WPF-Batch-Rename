using Contract;
using System.Text;

namespace OneSpace
{
    public class OneSpaceRule : IRule
    {
        public string Name { get => "OneSpace"; }

        public IRule create(string data)
        {
            return new OneSpaceRule();
        }

        public string GetParam()
        {
            return "";
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
            return true;
        }

        public string Rename(string origin)
        {
            char Space = ' ';
            var newName = new StringBuilder();
            newName.Append(origin[0]);
            for(int i = 1; i < origin.Length; i++)
            {
                if (origin[i] == Space)
                {
                    if (origin[i-1] != Space)
                    {
                        newName.Append(origin[i]);
                    }
                }
                else
                {
                    newName.Append(origin[i]);
                }
            }

            return newName.ToString();
        }
    }
}