using Contract;
using System.Text.RegularExpressions;

namespace ToPascalCase
{
    public class ToPascalCaseRule : IRule
    {
        public string Name => "ToPascalCase";

        public IRule create(string data)
        {
            return new ToPascalCaseRule();
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
            throw new NotImplementedException();
        }


        public string Rename(string origin)
        {

            int lastIndexOfDot = origin.IndexOf('.');
            string headPath = origin.Substring(0, lastIndexOfDot);
            string extension = origin.Substring(++lastIndexOfDot);

            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(headPath, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase) + '.' + extension;
        }
    }
}