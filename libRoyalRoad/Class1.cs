using System.Text;
using HtmlAgilityPack;
namespace libRoyalRoad;

public class RoyalRoad()
{
    HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
    public StoryData GetStoryData(string URL)
    {
        var page = web.Load(URL);
        StoryData storyData = new StoryData();
        storyData.Author = page.DocumentNode.SelectSingleNode("//meta[@property='books:author']").GetAttributeValue("content", string.Empty);
        storyData.Title = page.DocumentNode.SelectSingleNode("//title").InnerText.Replace(" | Royal Road", "");
        storyData.Chapters = page.DocumentNode.SelectSingleNode("//table[@id='chapters']").GetAttributeValue("content", string.Empty);
        storyData.Description = page.DocumentNode.SelectSingleNode("//meta[@name='description']").GetAttributeValue("content", string.Empty);
        storyData.Cover = page.DocumentNode.SelectSingleNode("//meta[@property='og:image']").GetAttributeValue("content", string.Empty);
        storyData.Rating = page.DocumentNode.SelectSingleNode("//meta[@property='books:rating:value']").GetAttributeValue("content", string.Empty);

        var firstChapterRow = page.DocumentNode.SelectSingleNode("//table[@id='chapters']//tbody//tr[@class='chapter-row']");
        var chapterLink = firstChapterRow.SelectSingleNode(".//td/a");
        if (chapterLink != null)
        {
            string chapterName = chapterLink.InnerText.Trim();
            string chapterUrl = chapterLink.GetAttributeValue("href", string.Empty);
            storyData.Chapter_One = "https://royalroad.com" + chapterUrl;
        }

        return storyData;
    }
    public ChapterData GetChapter(string URL)
    {
        var page = web.Load(URL);
        var chapterTitle = page.DocumentNode.SelectSingleNode("//title").InnerText;
        chapterTitle = chapterTitle.Replace(" | Royal Road", "");
        var metaKeywords = page.DocumentNode.SelectSingleNode("//meta[@name='keywords']");
        // Extract the content attribute
        string content = metaKeywords.GetAttributeValue("content", string.Empty);
            
        // Split the content by semicolon and trim each keyword
        var keywords = content.Split(';').Select(keyword => keyword.Trim()).ToList();
        // Get the title
        string title = keywords[0];
        
        var nodes = page.DocumentNode.SelectNodes("//div[@class='chapter-inner chapter-content']//p");
        StringBuilder sb = new StringBuilder();
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                string text = HtmlEntity.DeEntitize(node.InnerText.Trim());
                sb.AppendLine(text);
            }
        }

        // Get the final text
        string finalText = sb.ToString();
        
        
        // Select link elements with rel='canonical', rel='prev', and rel='next'
        var canonicalLink = page.DocumentNode.SelectSingleNode("//link[@rel='canonical']");
        var prevLink = page.DocumentNode.SelectSingleNode("//link[@rel='prev']");
        var nextLink = page.DocumentNode.SelectSingleNode("//link[@rel='next']");

        string canonicalHref = canonicalLink.GetAttributeValue("href", string.Empty);
        string nextHref;
        string prevHref;

        if (prevLink == null)
        {
            // If there is no previous link, let it be null.
            prevHref = null;
        }
        else
        {
            prevHref = "https://www.royalroad.com" + prevLink.GetAttributeValue("href", string.Empty);
        }
        if(nextLink == null)
        {
            // If there is no next link, let it be null.
            nextHref = null;
        }
        else
        {
            nextHref = "https://www.royalroad.com" + nextLink.GetAttributeValue("href", string.Empty);
        }
 
        return new ChapterData
        {
            Title = title, 
            ChapterTitle = chapterTitle,
            Text = finalText,
            previous = prevHref,
            next = nextHref,
            canonical = canonicalHref
        };
    }
    public class ChapterData
    {
        public string Title { get; set; }
        public string ChapterTitle { get; set; }
        public string Text { get; set; }
        public string previous { get; set; }
        public string next { get; set; }
        public string canonical { get; set; }
    }
    public class StoryData
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public string Rating { get; set; }
        public string Chapters { get; set; }
        public string Chapter_One { get; set; }
    }
}

