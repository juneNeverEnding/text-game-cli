namespace TextGameCLI.Core;

using System.Text;

public class ArgParser
{
    public (string Command, string[] Args) Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ("", Array.Empty<string>());
        
        var parts = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in input.Trim())
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }
        if (current.Length > 0)
            parts.Add(current.ToString());
        
        if (parts.Count == 0) return ("", Array.Empty<string>());

        return (parts[0], parts.Skip(1).ToArray());
    }
}