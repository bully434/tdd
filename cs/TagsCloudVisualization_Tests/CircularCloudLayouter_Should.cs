﻿using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization
{
    [TestFixture]
    class CircularCloudLayouter_Should
    {
        private Point center;
        private CircularCloudLayouter layout;
        private CircularCloudVisualizer visualizer;
        private List<Rectangle> rectangles;
        private Size defaultSize;

        [SetUp]
        public void SetUp()
        {
            center = new Point(0,0);
            layout = new CircularCloudLayouter();
            rectangles = new List<Rectangle>();
            visualizer = new CircularCloudVisualizer(rectangles);
            defaultSize = new Size(100, 50);
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Passed)
            {
                var directory = TestContext.CurrentContext.TestDirectory;
                var filename = TestContext.CurrentContext.Test.Name;
                var path = $"{directory}\\{filename}.png";
                var bitmap = visualizer.DrawRectangles();
                bitmap.Save(path);
                TestContext.WriteLine($"Tag cloud visualization saved to file {path}");
            }
        }


        [Test]
        public void CircularCloudLayouter_CreateEmptyLayout_EmptyLayout()
        {
            rectangles.Count.Should().Be(0);
        }

        [Test]
        public void PutNextRectangle_PutSingleRectangle_SingleRectangleInCenter()
        {
            var rectangle = layout.PutNextRectangle(defaultSize);
            rectangle.ShouldBeEquivalentTo(new Rectangle(new Point(-50, -25), defaultSize));
        }


        [Test]
        public void PutNextRectangle_PutTwoRectangles_RectanglesDoNotIntersect()
        {
            var firstRectangle = layout.PutNextRectangle(defaultSize);
            var secondRectangle = layout.PutNextRectangle(new Size(80, 40));
            firstRectangle.IntersectsWith(secondRectangle).Should().BeFalse();
        }

        [Test]
        public void PutNextRectangle_PutMultipleRectangles_RectanglesDoNotIntersect()
        {
            var random = new Random();

            for (var i = 0; i < 1000; i++)
            {
                var randomSize = new Size(random.Next(200), random.Next(100));
                var newRectangle = layout.PutNextRectangle(randomSize);
                rectangles.ForEach(rect => rect.IntersectsWith(newRectangle).Should().BeFalse());
                rectangles.Add(newRectangle);
            }
        }

        [Test]
        public void PutNextRectangle_PutMultipleRectangles_RectanglesArePlacedTightly()
        {
            var random = new Random();
            var rectangles = new List<Rectangle>();
            var circleRadius = 0;
            var cloudSquare = 0;

            for (var i = 0; i < 100; i++)
            {
                var randomSize = new Size(random.Next(150, 200), random.Next(75, 100));
                var newRectangle = layout.PutNextRectangle(randomSize);
                rectangles.ForEach(rect => rect.IntersectsWith(newRectangle).Should().BeFalse());
                circleRadius = newRectangle.GetCircumcircleRadius();
                cloudSquare += newRectangle.Width * newRectangle.Height;
                rectangles.Add(newRectangle);
            }
            var circleSquare = Math.PI * Math.Pow(circleRadius, 2);
            (cloudSquare / circleSquare).Should().BeApproximately(0.8, 0.2);
        }

    }
}