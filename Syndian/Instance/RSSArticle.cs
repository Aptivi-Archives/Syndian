
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

using System.Collections.Generic;
using System.Xml;

namespace Syndian.Instance
{
    /// <summary>
    /// RSS article instance
    /// </summary>
    public class RSSArticle
    {

        private readonly string articleTitle;
        private readonly string articleLink;
        private readonly string articleDescription;
        private readonly Dictionary<string, XmlNode> articleVariables;

        /// <summary>
        /// RSS Article Title
        /// </summary>
        public string ArticleTitle
            => articleTitle;

        /// <summary>
        /// RSS Article Link
        /// </summary>
        public string ArticleLink
            => articleLink;

        /// <summary>
        /// RSS Article Descirption
        /// </summary>
        public string ArticleDescription
            => articleDescription;

        /// <summary>
        /// RSS Article Parameters
        /// </summary>
        public Dictionary<string, XmlNode> ArticleVariables
            => articleVariables;

        /// <summary>
        /// Makes a new instance of RSS article
        /// </summary>
        /// <param name="ArticleTitle">Article title</param>
        /// <param name="ArticleLink">Link to article</param>
        /// <param name="ArticleDescription">Article description</param>
        /// <param name="ArticleVariables">Article variables as <see cref="XmlNode"/>s</param>
        internal RSSArticle(string ArticleTitle, string ArticleLink, string ArticleDescription, Dictionary<string, XmlNode> ArticleVariables)
        {
            articleTitle = !string.IsNullOrWhiteSpace(ArticleTitle) ? ArticleTitle.Trim() : "";
            articleLink = !string.IsNullOrWhiteSpace(ArticleLink) ? ArticleLink.Trim() : "";
            articleDescription = !string.IsNullOrWhiteSpace(ArticleDescription) ? ArticleDescription.Trim() : "";
            articleVariables = ArticleVariables;
        }

    }
}
