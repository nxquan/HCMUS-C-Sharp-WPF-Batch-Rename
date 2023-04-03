using Contract;
using System.Text;

namespace AddCounter
{
    public class AddCounterRule : IRule
    {
        public int Start { get; set; }
        public int Step { get; set; }
        public int Current { get; set; }
        public int NumberOfDigit { get; set; }

        public string Name => "AddCounter";

        public IRule create(string data)
        {
            AddCounterRule result = new AddCounterRule();
            var tokens = data.Split(' ');
            var properties = tokens[1].Split(new string[] {","}, StringSplitOptions.None);
            var startString = properties[0].Split('=');
            var stepString = properties[1].Split('=');
            var numberString = properties[2].Split('=');

            result.Start = int.Parse(startString[1]);
            result.Current = result.Start;
            result.Step = int.Parse(stepString[1]);
            result.NumberOfDigit = int.Parse(numberString[1]);

            return result;
        }

        public string GetParam()
        {
            return $"Start={Start},Step={Step},NumberOfDigit={NumberOfDigit}";
        }

        public void PassParam(string firstParam)
        {
            throw new NotImplementedException();
        }

        public void PassParam(int firstParam, int secondParam, int thirdParam)
        {
            this.Start = firstParam;
            this.Current = firstParam;
            this.Step = secondParam;
            this.NumberOfDigit = thirdParam;
        }

        public bool RefreshParam()
        {
            this.Start = 0;
            this.Step = 0;
            this.NumberOfDigit = 0;
            return true;
        }

        public string Rename(string origin)
        {
            string newName = origin;
            int lastIndexOfDot = origin.LastIndexOf('.');
            string currentCounter = Current.ToString().PadLeft(NumberOfDigit, '0');
            newName = newName.Insert(lastIndexOfDot, currentCounter);

            Current += Step;
            return newName;
        }

    }
}