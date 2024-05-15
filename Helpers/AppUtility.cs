using System.Text;

namespace Helpers
{
    public class AppUtility
    {
        public static AppUtility O => instance.Value;
        private static Lazy<AppUtility> instance = new Lazy<AppUtility>(() => new AppUtility());
        private AppUtility() { }
        public string AddSpacesAfterEveryNCharacters(string input, int n)
        {
            StringBuilder result = new StringBuilder();
            int counter = 0;
            foreach (char c in input)
            {
                result.Append(c);
                if (++counter % n == 0)
                {
                    result.Append(" ");
                }
            }
            return result.ToString();
        }
    }
}
