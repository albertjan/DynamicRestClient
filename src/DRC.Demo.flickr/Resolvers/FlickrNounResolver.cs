﻿using System;
using System.Collections.Generic;

namespace DRC.Demo.flickr.Resolvers
{
    using Interfaces;

    public class FlickrNounResolver : INounResolver
    {
        public IStringTokenizer Tokenizer { get; set; }

        public FlickrNounResolver(IStringTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public string ResolveNoun (string naaca)
        {
            // so all the urls have this schema: flickr.generaldirection.[get,find,lookup,set,add]Noun
            // example: flickr.people.getInfo
            //      one would need to call:
            //      http://api.flickr.com/services/rest/?method=flick.people.getInfo&api_key=[key]&user_id=61304303%40N08        
            // and we would want then: GetInfoOnPeople(new { user_id = 61304303@N08 });
            // So in the case of flickr this function would always resolve "/services/rest"
            
            return "/services/rest";
        }

        public Dictionary<string, string> PredefinedUrls { get; set; }
    }
}
