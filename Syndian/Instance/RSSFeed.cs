
/*
 * MIT License
 *
 * Copyright (c) 2023 Aptivi
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Xml;

namespace Syndian.Instance
{
    /// <summary>
    /// RSS feed class instance
    /// </summary>
    public class RSSFeed
    {

        private string _FeedUrl;
        private RSSFeedType _FeedType;
        private string _FeedTitle;
        private string _FeedDescription;
        private List<RSSArticle> _FeedArticles = new();

        /// <summary>
        /// A URL to RSS feed
        /// </summary>
        public string FeedUrl
            => _FeedUrl;

        /// <summary>
        /// RSS feed type
        /// </summary>
        public RSSFeedType FeedType
            => _FeedType;

        /// <summary>
        /// RSS feed type
        /// </summary>
        public string FeedTitle
            => _FeedTitle;

        /// <summary>
        /// RSS feed description (Atom feeds not supported and always return an empty string)
        /// </summary>
        public string FeedDescription
            => _FeedDescription;

        /// <summary>
        /// Feed articles
        /// </summary>
        public List<RSSArticle> FeedArticles
            => _FeedArticles;

        /// <summary>
        /// Makes a new instance of an RSS feed class
        /// </summary>
        /// <param name="FeedUrl">A URL to RSS feed</param>
        /// <param name="FeedType">A feed type to parse. If set to Infer, it will automatically detect the type based on contents.</param>
        public RSSFeed(string FeedUrl, RSSFeedType FeedType)
            => Refresh(FeedUrl, FeedType);

        /// <summary>
        /// Refreshes the RSS class instance
        /// </summary>
        public void Refresh()
            => Refresh(_FeedUrl, _FeedType);

        /// <summary>
        /// Refreshes the RSS class instance
        /// </summary>
        /// <param name="FeedUrl">A URL to RSS feed</param>
        /// <param name="FeedType">A feed type to parse. If set to Infer, it will automatically detect the type based on contents.</param>
        public void Refresh(string FeedUrl, RSSFeedType FeedType)
        {
            // Make a web request indicator
            var FeedWebRequest = RSSTools.Client.GetAsync(FeedUrl).Result;

            // Load the RSS feed and get the feed XML document
            var FeedStream = FeedWebRequest.Content.ReadAsStreamAsync().Result;
            var FeedDocument = new XmlDocument();
            FeedDocument.Load(FeedStream);

            // Infer feed type
            var FeedNodeList = default(XmlNodeList);
            if (FeedType == RSSFeedType.Infer)
            {
                if (FeedDocument.GetElementsByTagName("rss").Count != 0)
                {
                    FeedNodeList = FeedDocument.GetElementsByTagName("rss");
                    _FeedType = RSSFeedType.RSS2;
                }
                else if (FeedDocument.GetElementsByTagName("rdf:RDF").Count != 0)
                {
                    FeedNodeList = FeedDocument.GetElementsByTagName("rdf:RDF");
                    _FeedType = RSSFeedType.RSS1;
                }
                else if (FeedDocument.GetElementsByTagName("feed").Count != 0)
                {
                    FeedNodeList = FeedDocument.GetElementsByTagName("feed");
                    _FeedType = RSSFeedType.Atom;
                }
            }
            else if (FeedType == RSSFeedType.RSS2)
            {
                FeedNodeList = FeedDocument.GetElementsByTagName("rss");
                if (FeedNodeList.Count == 0)
                    throw new RSSException("Invalid RSS2 feed.");
            }
            else if (FeedType == RSSFeedType.RSS1)
            {
                FeedNodeList = FeedDocument.GetElementsByTagName("rdf:RDF");
                if (FeedNodeList.Count == 0)
                    throw new RSSException("Invalid RSS1 feed.");
            }
            else if (FeedType == RSSFeedType.Atom)
            {
                FeedNodeList = FeedDocument.GetElementsByTagName("feed");
                if (FeedNodeList.Count == 0)
                    throw new RSSException("Invalid Atom feed.");
            }

            // Populate basic feed properties
            string FeedTitle = Convert.ToString(RSSTools.GetFeedProperty("title", FeedNodeList, _FeedType));
            string FeedDescription = Convert.ToString(RSSTools.GetFeedProperty("description", FeedNodeList, _FeedType));

            // Populate articles
            var Articles = RSSTools.MakeRssArticlesFromFeed(FeedNodeList, _FeedType);

            // Install the variables to a new instance
            _FeedUrl = FeedUrl;
            _FeedTitle = FeedTitle.Trim();
            _FeedDescription = FeedDescription.Trim();
            if (_FeedArticles.Count != 0 & Articles.Count != 0)
            {
                if (!_FeedArticles[0].Equals(Articles[0]))
                    _FeedArticles = Articles;
            }
            else
                _FeedArticles = Articles;
        }

    }

    /// <summary>
    /// Enumeration for RSS feed type
    /// </summary>
    public enum RSSFeedType
    {
        /// <summary>
        /// The RSS format is RSS 2.0
        /// </summary>
        RSS2,
        /// <summary>
        /// The RSS format is RSS 1.0
        /// </summary>
        RSS1,
        /// <summary>
        /// The RSS format is Atom
        /// </summary>
        Atom,
        /// <summary>
        /// Try to infer RSS type
        /// </summary>
        Infer = 1024
    }
}
