using Contract;
using System.Text;

namespace ChangeExtension
{
    public class ChangeExtensionRule : IRule
    {
        public string Extension { get; set; } = "";
        public string Name => "ChangeExtension";

        public IRule create(string data)
        {
            var tokens = data.Split(new string[] {" "}, StringSplitOptions.None);
            ChangeExtensionRule result = new ChangeExtensionRule();
            result.Extension = tokens[1];

            return result;
        }
        public string GetParam()
        {
            return Extension;
        }
        public void PassParam(string firstParam)
        {
            this.Extension = firstParam;    
        }

        public void PassParam(int firstParam, int secondParam, int thirdParam)
        {
            throw new NotImplementedException();
        }

        public bool RefreshParam()
        {
            this.Extension = "";
            return true;
        }

        public string Rename(string origin)
        {
            StringBuilder newName = new StringBuilder();
            if(Extension.Length > 0)
            {
                char dot = '.';
                int indexOfLastDot = origin.LastIndexOf(dot);
                if (indexOfLastDot != -1)
                {
                    for (int i = 0; i < indexOfLastDot; i++)
                    {
                        newName.Append(origin[i]);
                    }
                }
                newName.Append(dot);
                newName.Append(Extension);
            }
            else
            {
                newName.Append(origin);
            }
            return newName.ToString();

        }
    }
}