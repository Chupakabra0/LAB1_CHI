using System.Text.RegularExpressions;

namespace RKMApp.Logic {
    public static class CheckSyntax {
        public static bool IsDouble(string str) => string.Empty.Equals(str) || !Regex.IsMatch(str, $@"(^|^-)(\d+)(\.(\d+)$|$)");
    }
}