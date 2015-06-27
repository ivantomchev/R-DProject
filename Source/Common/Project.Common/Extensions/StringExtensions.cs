namespace Project.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
            {
                return string.Empty;
            }

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }
}
