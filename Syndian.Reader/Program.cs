//
// MIT License
//
// Copyright (c) 2023 Aptivi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using Syndian.Instance;
using System;

namespace Syndian.Reader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Check to see if the URL to the RSS feed is specified
            if (args.Length == 0)
            {
                Console.WriteLine("You must specify a URL to the RSS feed.");
                Environment.Exit(1);
            }

            // Populate the RSS info
            var feed = new RSSFeed(args[0], RSSFeedType.Infer);
            feed.Refresh();
            Console.WriteLine("Title: {0}", feed.FeedTitle);
            Console.WriteLine("Description: {0}", feed.FeedDescription);
            Console.WriteLine("URL: {0}\n", feed.FeedUrl);

            // Populate the article info
            foreach (RSSArticle article in feed.FeedArticles)
            {
                Console.WriteLine("Title: {0}", article.ArticleTitle);
                Console.WriteLine("Description: {0}", article.ArticleDescription);
                Console.WriteLine("URL: {0}\n", article.ArticleLink);
            }
        }
    }
}
