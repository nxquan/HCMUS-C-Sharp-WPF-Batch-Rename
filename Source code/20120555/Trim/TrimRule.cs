using Contract;

namespace Trim
{
    public class TrimRule : IRule
    {
        public string Name => "Trim";

        public IRule create(string data)
        {
            TrimRule result = new TrimRule();

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
            string newName = origin;

            newName = origin.Trim();
            return newName;
        }
    }
}