using Contract;
using System.Text;

namespace ToLowerCaseAndRemoveSpace
{
    public class ToLowerCaseAndRemoveSpaceRule : IRule
    {
        public string Name => "ToLowerCaseAndRemoveSpace";

        public IRule create(string data)
        {
            ToLowerCaseAndRemoveSpaceRule result = new ToLowerCaseAndRemoveSpaceRule();

            return result;
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
            StringBuilder newName = new StringBuilder();
            string temp = origin.ToLower();
            foreach(var c in temp)
            {
                if(c != ' ')
                {
                    newName.Append(c);
                }
            }
            return newName.ToString();
        }
    }
}