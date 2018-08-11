using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_mon
{
    public class Table : IDisposable
    {
        private readonly int _tableWidth;
        private CancellationTokenSource _cts;
        public Tuple<string, string> Header { get; set; }
        public ConcurrentDictionary<string, int> Data { get; set; }
        public Table(int tableWidth)
        {
            _tableWidth = tableWidth;
            Data = new ConcurrentDictionary<string, int>();
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            Console.Clear();
            var origRow = Console.CursorTop;
            var origCol = Console.CursorLeft;
            PrintLine();
            PrintRow(Header.Item1, Header.Item2);
            PrintLine();
            // Create listener thread
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.SetCursorPosition(origCol, origRow + 3);
                    foreach (var item in Data)
                    {
                        PrintRow(item.Key, item.Value);
                    }
                    PrintLine();
                    Thread.Sleep(100);
                }

            }, token);
        }

        public void Stop()
        {
            _cts.Cancel();
        }
        public void PrintLine()
        {
            Console.WriteLine(new string('-', _tableWidth));
        }

        public void PrintRow(string measure, int value)
        {
            PrintRow(measure, value.ToString());
        }
        public void PrintRow(params string[] columns)
        {
            int width = (_tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
