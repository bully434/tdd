﻿using System.Drawing;

namespace TagsCloudVisualization
{
    public class Tag
    {
        public Tag(string word, Font font, Rectangle rectangle)
        {
            Word = word;
            Font = font;
            Rectangle = rectangle;
        }

        public string Word { get; }
        public Font Font { get; }
        public Rectangle Rectangle { get; }
    }
}