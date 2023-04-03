namespace Contract
{
    public interface IRule
    {   
        public string Name { get;}
        string Rename(string origin);
        public void PassParam(string firstParam);
        public void PassParam(int firstParam, int secondParam, int thirdParam);
        public IRule create(string data);
        public bool RefreshParam();

        public string GetParam();
    }
}