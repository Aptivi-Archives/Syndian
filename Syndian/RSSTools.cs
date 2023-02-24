
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

using Extensification.DictionaryExts;
using HtmlAgilityPack;
using Syndian.Instance;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml;

namespace Syndian
{
    /// <summary>
    /// The RSS tools
    /// </summary>
    public static class RSSTools
    {
        internal static HttpClient Client = new();

        /// <summary>
        /// Make instances of RSS Article from feed node and type
        /// </summary>
        /// <param name="FeedNode">Feed XML node</param>
        /// <param name="FeedType">Feed type</param>
        public static List<RSSArticle> MakeRssArticlesFromFeed(XmlNodeList FeedNode, RSSFeedType FeedType)
        {
            var Articles = new List<RSSArticle>();
            switch (FeedType)
            {
                case RSSFeedType.RSS2:
                    {
                        foreach (XmlNode Node in FeedNode[0]) // <channel>
                        {
                            foreach (XmlNode Child in Node.ChildNodes) // <item>
                            {
                                if (Child.Name == "item")
                                {
                                    var Article = MakeArticleFromFeed(Child);
                                    Articles.Add(Article);
                                }
                            }
                        }

                        break;
                    }
                case RSSFeedType.RSS1:
                    {
                        foreach (XmlNode Node in FeedNode[0]) // <channel> or <item>
                        {
                            if (Node.Name == "item")
                            {
                                var Article = MakeArticleFromFeed(Node);
                                Articles.Add(Article);
                            }
                        }

                        break;
                    }
                case RSSFeedType.Atom:
                    {
                        foreach (XmlNode Node in FeedNode[0]) // <feed>
                        {
                            if (Node.Name == "entry")
                            {
                                var Article = MakeArticleFromFeed(Node);
                                Articles.Add(Article);
                            }
                        }

                        break;
                    }

                default:
                    {
                        throw new RSSException("Invalid RSS feed type.");
                    }
            }
            return Articles;
        }

        /// <summary>
        /// Generates an instance of article from feed
        /// </summary>
        /// <param name="Article">The child node which holds the entire article</param>
        /// <returns>An article</returns>
        public static RSSArticle MakeArticleFromFeed(XmlNode Article)
        {
            // Variables
            var Parameters = new Dictionary<string, XmlNode>();
            string Title = default, Link = default, Description = default;

            // Parse article
            foreach (XmlNode ArticleNode in Article.ChildNodes)
            {
                // Check the title
                if (ArticleNode.Name == "title")
                {
                    // Trimming newlines and spaces is necessary, since some RSS feeds (GitHub commits) might return string with trailing and leading spaces and newlines.
                    Title = ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' ');
                }

                // Check the link
                if (ArticleNode.Name == "link")
                {
                    // Links can be in href attribute, so check that.
                    if (ArticleNode.Attributes.Count != 0 & ArticleNode.Attributes.GetNamedItem("href") is not null)
                    {
                        Link = ArticleNode.Attributes.GetNamedItem("href").InnerText;
                    }
                    else
                    {
                        Link = ArticleNode.InnerText;
                    }
                }

                // Check the summary
                if (ArticleNode.Name == "summary" | ArticleNode.Name == "content" | ArticleNode.Name == "description")
                {
                    // It can be of HTML type, or plain text type.
                    if (ArticleNode.Attributes.Count != 0 & ArticleNode.Attributes.GetNamedItem("type") is not null)
                    {
                        if (ArticleNode.Attributes.GetNamedItem("type").Value == "html")
                        {
                            // Extract plain text from HTML
                            var HtmlContent = new HtmlDocument();
                            HtmlContent.LoadHtml(ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' '));

                            // Some feeds have no node called "pre," so work around this...
                            var PreNode = HtmlContent.DocumentNode.SelectSingleNode("pre");
                            if (PreNode is null)
                            {
                                Description = HtmlContent.DocumentNode.InnerText;
                            }
                            else
                            {
                                Description = PreNode.InnerText;
                            }
                        }
                        else
                        {
                            Description = ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' ');
                        }
                    }
                    else
                    {
                        Description = ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' ');
                    }
                }
                Parameters.AddIfNotFound(ArticleNode.Name, ArticleNode);
            }
            return new RSSArticle(Title, Link, Description, Parameters);
        }

        /// <summary>
        /// Gets a feed property
        /// </summary>
        /// <param name="FeedProperty">Feed property name</param>
        /// <param name="FeedNode">Feed XML node</param>
        /// <param name="FeedType">Feed type</param>
        public static object GetFeedProperty(string FeedProperty, XmlNodeList FeedNode, RSSFeedType FeedType)
        {
            switch (FeedType)
            {
                case RSSFeedType.RSS2:
                    {
                        foreach (XmlNode Node in FeedNode[0]) // <channel>
                        {
                            foreach (XmlNode Child in Node.ChildNodes)
                            {
                                if (Child.Name == FeedProperty)
                                {
                                    return Child.InnerXml;
                                }
                            }
                        }

                        break;
                    }
                case RSSFeedType.RSS1:
                    {
                        foreach (XmlNode Node in FeedNode[0]) // <channel> or <item>
                        {
                            foreach (XmlNode Child in Node.ChildNodes)
                            {
                                if (Child.Name == FeedProperty)
                                {
                                    return Child.InnerXml;
                                }
                            }
                        }

                        break;
                    }
                case RSSFeedType.Atom:
                    {
                        foreach (XmlNode Node in FeedNode[0]) // Children of <feed>
                        {
                            if (Node.Name == FeedProperty)
                            {
                                return Node.InnerXml;
                            }
                        }

                        break;
                    }

                default:
                    {
                        throw new RSSException("Invalid RSS feed type.");
                    }
            }
            return "";
        }
    }
}
