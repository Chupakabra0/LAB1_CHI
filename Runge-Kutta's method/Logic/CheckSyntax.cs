using System.Text.RegularExpressions;

namespace WpfApp1.Logic {
    public static class CheckSyntax {
        public static bool IsDouble(string str) => string.Empty.Equals(str) && !Regex.IsMatch(str, $@"^(\d+)(.(\d+)$|$)");
    }
}