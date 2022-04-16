﻿namespace Privasight.Model.Shared.Display
{
    public class LineChartItem
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }

        public LineChartItem(int number, DateTime date)
        {
            Number = number;
            Date = date;
        }
    }
}