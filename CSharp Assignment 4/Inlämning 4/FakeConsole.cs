using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Inlämning_4
{
    // This class is needed for testing console applications. Do not change or remove it.
    internal class FakeConsole : IDisposable
    {
        private TextReader originalIn;
        private FakeConsoleIn fakeIn;

        private TextWriter originalOut;
        private FakeConsoleOut fakeOut;

        private bool isReading = false;

        public string[] Lines
        {
            get => fakeOut.Outputs.Last().Trim(Environment.NewLine.ToCharArray()).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public string Output
        {
            get => Lines.Last();
        }

        public FakeConsole(params string[] lines)
        {
            string text = string.Join("", lines.Select(x => x + Environment.NewLine));

            originalIn = Console.In;
            fakeIn = new FakeConsoleIn(text, this);
            Console.SetIn(fakeIn);

            originalOut = Console.Out;
            fakeOut = new FakeConsoleOut(this);
            Console.SetOut(fakeOut);
        }

        public void Dispose()
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        private class FakeConsoleIn : TextReader
        {
            private string text;
            private int index = 0;
            private FakeConsole console;

            public FakeConsoleIn(string text, FakeConsole console)
            {
                this.text = text;
                this.console = console;
            }

            public override int Read()
            {
                console.isReading = true;
                if (index < text.Length)
                {
                    int c = text[index];
                    index += 1;
                    return c;
                }
                else
                {
                    return -1;
                }
            }

            public override int Peek()
            {
                return text[index];
            }
        }

        private class FakeConsoleOut : TextWriter
        {
            private FakeConsole console;

            private List<string> outputs = new List<string> { "" };
            public string[] Outputs
            {
                get => outputs.ToArray();
            }

            public override Encoding Encoding => Encoding.UTF8;

            public FakeConsoleOut(FakeConsole console)
            {
                this.console = console;
            }

            public override void Write(char value)
            {
                // If input was just read from the user, add a new group of output, so we can separate them later.
                if (console.isReading)
                {
                    console.isReading = false;
                    outputs.Add("");
                }

                outputs[outputs.Count - 1] += value;
            }
        }
    }
}