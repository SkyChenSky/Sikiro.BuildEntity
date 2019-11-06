using System.Text;

namespace 陈珙.AutoBuildEntity.Common.Helper
{
    public static class StringExtension
    {
        public static string ToCaseCamelName(this string name)
        {
            var result = new StringBuilder();
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }

            var nameList = name.Split('_');

            foreach (var field in nameList)
            {
                for (var i = 0; i < field.Length; i++)
                {
                    result.Append(i == 0 ? field[i].ToString().ToUpper() : field[i].ToString().ToLower());
                }
            }

            return result.ToString();
        }
    }
}
