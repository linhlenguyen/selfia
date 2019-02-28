using System.Collections.Generic;
using System.Linq;

namespace Selfia
{
    internal class Program
    {
        public enum Orientation
        {
            Horizontal = 0,
            Vertial = 1
        };
        public class Picture
        {
            public Orientation Orientation { get; set; }
            public List<string> Tags { get; set; }

            public Picture(Orientation orientation, List<string> tags)
            {
                Orientation = orientation;
                Tags = Tags;
            }
        }

        public static void Main(string[] args)
        {
            var pictures = new List<Picture>()
            {
                new Picture(Orientation.Horizontal, new List<string>() {"cat", "beach", "sun"}),
                new Picture(Orientation.Vertial, new List<string>() {"selfie", "smile"}),
                new Picture(Orientation.Vertial, new List<string>() {"garden", "selfie"}),
                new Picture(Orientation.Horizontal, new List<string>() {"garden", "cat"})
            };
            LinkedList<Picture> results = new LinkedList<Picture>();
            var horizontals = pictures.Where(picture => picture.Orientation == Orientation.Horizontal);
            var verticals = pictures.Where(picture => picture.Orientation == Orientation.Vertial);
        }
    }
}