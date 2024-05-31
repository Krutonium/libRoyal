using System;
using System.Net;
using libRoyalRoad;

namespace RoyalTerminal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RoyalTerminal <URL>");
                return;
            }
            RoyalRoad royalRoad = new RoyalRoad();
            var story = royalRoad.GetStoryData(args[1]);
            // Print the story title, author, rating, description and save the cover image to disk
            Console.WriteLine($"Title:  ------------ {story.Title}");
            Console.WriteLine($"Author: ------------ {story.Author}");
            Console.WriteLine($"Rating: ------------ {story.Rating} Stars");
            Console.WriteLine($"Brief Description: - {story.Description}");
            WebClient wc = new WebClient();
            var chapter =
                royalRoad.GetChapter(story.Chapter_One);
            // Print the chapter title, format and save to disk in folder by the name of the story
            int i = 0;
            string index = "";
            
            if(!Directory.Exists(chapter.Title))
            {
                Directory.CreateDirectory(chapter.Title);
            }
            wc.DownloadFile(story.Cover, $"./{chapter.Title}/cover.jpg");

            while (true)
            {
                // Print the chapter title
                Console.WriteLine(chapter.Title + " " + chapter.ChapterTitle);
                // Write the chapter text to disk
                string ChapterText = $"{chapter.Title} {Environment.NewLine} {chapter.Text}";
                string fileName = $"{chapter.Title}/{i} - {chapter.ChapterTitle}.txt";
                System.IO.File.WriteAllText(fileName, ChapterText);
                // Get the next chapter
                index += $"{i} - {chapter.Title}{Environment.NewLine}";
                i++;
                if(chapter.next == null)
                {
                    break;
                }
                chapter = royalRoad.GetChapter(chapter.next);
                System.Threading.Thread.Sleep(5000);
            }
            System.IO.File.WriteAllText("index.txt", index);
        }
    }
        
}