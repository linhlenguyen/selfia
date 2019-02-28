using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Selfia
{
    public enum Orientation
    {
        Horizontal = 0,
        Vertical = 1
    };
    public class Picture
    {
        public int Id { get; set; }
        public Orientation Orientation { get; set; }
        public List<string> Tags { get; set; }

        public Tuple<int, int> VerticalSources { get; set; }

        public Picture()
        {

        }

        public Picture(Orientation orientation, List<string> tags)
        {
            Orientation = orientation;
            Tags = tags;
        }
    }

    internal class Program
    {
        public static List<Picture> BestVerticalPicturesMatch(List<Picture> verticals)
        {
            var toReturn = new List<Picture>();

            var remainingTags = verticals.OrderByDescending(x => x.Tags.Count).ToList();
            for (var i = 0; i < verticals.Count() / 2; i++)
            {
                if (i >= remainingTags.Count)
                {
                    break;
                }
                var currentPicture = remainingTags[i];
                remainingTags.RemoveAt(i);

                var options = remainingTags.Where(x => x.Tags.Count() >= currentPicture.Tags.Count());

                var mininumUnion = int.MaxValue;
                var bestOption = currentPicture;
                foreach (var option in options)
                {
                    var currentUnion = option.Tags.Union(currentPicture.Tags).Distinct();
                    if (currentUnion.Count() < mininumUnion)
                    {
                        mininumUnion = currentUnion.Count();
                        bestOption = option;
                    }
                }
                if (currentPicture.Id != bestOption.Id)
                {
                    toReturn.Add(new Picture()
                    {
                        Orientation = Orientation.Vertical,
                        Tags = currentPicture.Tags.Union(bestOption.Tags).ToList(),
                        VerticalSources = new Tuple<int, int>(currentPicture.Id, bestOption.Id)
                    });

                    remainingTags.Remove(bestOption);
                }
            }

            return toReturn;
        }

        public static int calculateMatchingScore(Picture pic1, Picture pic2)
        {
            var union = pic1.Tags.Intersect(pic2.Tags).Count();
            return Math.Min(union, Math.Min(pic1.Tags.Count() - union, pic2.Tags.Count() - union));
        }

        public static Tuple<Picture,int> findBestMatch(List<Picture> pics, Picture source)
        {
            var score = 0;
            Picture match = null;
            foreach (Picture p in pics)
            {
                var newScore = calculateMatchingScore(p, source);
                if (newScore > score)
                {
                    score = newScore;
                    match = p;
                }
            }
            return new Tuple<Picture, int>(match, score);
        }

        public static int index = 0;
        public static Picture ToPicture(string picString)
        {
            string[] splittedString = picString.Split(' ');
            var orientation = splittedString[0];
            var tagCount = splittedString[1];
            var tags = splittedString.Skip(2);
            return new Picture()
            {
                Orientation = orientation.Equals("H") ? Orientation.Horizontal : Orientation.Vertical,
                Id = index++,
                Tags = tags.ToList()
            };
        }

        public static List<Picture> ReadFile()
        {
            var a = @"../../InputFiles/";
            var b = new List<String>();
            var c = Directory.GetFiles(a);

            var b_lovely_landscapes = c
              .Where(f => f.Contains(@"b_lovely_landscapes"))
              .Select(g => new { FileName = @"b_lovely_landscapes", Contents = File.ReadAllLines(g) })
              .ToList();

            var c_memorable_moments = c
              .Where(f => f.Contains(@"c_memorable_moments"))
              .Select(g => new { FileName = @"c_memorable_moments", Contents = File.ReadAllLines(g) })
              .ToList();

            var d_pet_pictures = c
              .Where(f => f.Contains(@"d_pet_pictures"))
              .Select(g => new { FileName = @"d_pet_pictures", Contents = File.ReadAllLines(g) })
              .ToList();

            var e_shiny_selfies = c
              .Where(f => f.Contains(@"e_shiny_selfies"))
              .Select(g => new { FileName = @"e_shiny_selfies", Contents = File.ReadAllLines(g) })
              .ToList();

            var r1 = c_memorable_moments.SelectMany(x => x.Contents.Skip(1).Select(y => ToPicture(y))).ToList();
            return r1;
        }
        
        public static void Main(string[] args)
        {
            var pictures = ReadFile();
            var horizontals = pictures.Where(picture => picture.Orientation == Orientation.Horizontal);
            var verticals = pictures.Where(picture => picture.Orientation == Orientation.Vertical).ToList();
            var verticalPairs = BestVerticalPicturesMatch(verticals);
            var pictureSet = horizontals.Concat(verticalPairs).ToList();
            LinkedList<Picture> result = new LinkedList<Picture>();
            var first = pictureSet.First();
            pictureSet.Remove(first);
            result.AddFirst(first);
            var bestMatch = findBestMatch(pictureSet, first);
            pictureSet.Remove(bestMatch.Item1);
            result.AddLast(bestMatch.Item1);
            while (true)
            {
                var fqueue = result.First();
                var lqueue = result.Last();
                var bestMatch1 = findBestMatch(pictureSet, fqueue);
                var bestMatch2 = findBestMatch(pictureSet, lqueue);
                if (bestMatch1.Item1 == null && bestMatch2.Item1 == null)
                {
                    break;
                }
                if (bestMatch1.Item2 > bestMatch2.Item2)
                {
                    result.AddFirst(bestMatch1.Item1);
                    pictureSet.Remove(bestMatch1.Item1);
                }
                else
                {
                    result.AddLast(bestMatch2.Item1);
                    pictureSet.Remove(bestMatch2.Item1);
                }
            }
            
            var b = result.Aggregate(@"", (a, i) =>
            {
                if (i.Orientation == Orientation.Horizontal)
                {
                    return a + '\n' + i.Id.ToString();
                }
                else return a + '\n' + i.VerticalSources.Item1.ToString() + " " + i.VerticalSources.Item2.ToString();
            });

            File.WriteAllText(@"../../r.txt", b);
        }
    }
}