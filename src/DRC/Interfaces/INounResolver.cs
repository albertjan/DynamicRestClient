namespace DRC.Interfaces
{
    using System.Collections.Generic;

    public interface INounResolver
    {
        /// <summary>
        /// Since this is never standardized one might have to implement their own!
        /// </summary>
        /// <param name="naaca">Nouns, Adjectives, Adverbs, Conjunctions and Articles</param>
        /// <returns>/a/string/pointing/to/a/location/on/a/resource/</returns>
        string ResolveNoun(string naaca);

        Dictionary<string, string> PredefinedUrls { get; set; }
    }
}