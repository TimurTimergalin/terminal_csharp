using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terminal
{
    /// <summary>
    /// Base class for all elements to be displayed in the terminal
    /// </summary>
    public abstract class Element
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public int Left { get; set; }
        public int Top { get; set; }

        public HashSet<string> Tags { get; } = new();

        public Element WithTag(params string[] tags)
        {
            foreach (string s in tags)
            {
                Tags.Add(s);
            }

            return this;
        }

        /// <summary>
        /// Sets element to its default state
        /// </summary>
        public virtual void Reset()
        {
            Display();
        }

        public Window ParentWindow { get; set; }

        protected internal virtual void ToggleWindow(Window window)
        {
            ParentWindow = window;
        }

        protected internal abstract void Display();
    }

    public class TextElement : Element
    {
        public string Text { get; private set; }
        private ConsoleColor Foreground { get; set; }

        private string Default { get; }

        /// <param name="text">Text to be displayed at the start of the program</param>
        /// <param name="maxHeight">The number of lines reserved for the element</param>
        /// <param name="default">Text that will be set when the window is reset. Set to null to not change the text of the element after reset</param>
        public TextElement(string text, int maxHeight, string @default = null)
        {
            Text = text;
            Height = maxHeight;
            Default = @default ?? text;
        }

        public TextElement(string text, string @default = null) : this(text, text.Split(Environment.NewLine).Length,
            @default)
        {
        }

        /// <summary>
        /// Creates an TextElement occupying curtain amount of lines
        /// </summary>
        /// <param name="height">Number of lines to occupy</param>
        /// <returns></returns>
        public static TextElement Skip(int height = 1)
        {
            return new TextElement("", height);
        }

        public override void Reset()
        {
            Write(Default);
            base.Reset();
        }

        protected internal override void ToggleWindow(Window window)
        {
            base.ToggleWindow(window);
            Foreground = window.Colors.Foreground;
        }

        public override int Width => Text.Split(Environment.NewLine).Select(x => x.Length).Max();
        public override int Height { get; }

        protected internal override void Display()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ParentWindow.Colors.Background;
            Console.ForegroundColor = Foreground;
            Console.SetCursorPosition(Left, Top);
            foreach (string s in Text.Split(Environment.NewLine))
            {
                Console.Write(s);
                Console.Write('\n');
                Console.SetCursorPosition(Left, Console.CursorTop);
            }
        }

        private string Trim(string text)
        {
            try
            {
                return string.Join(Environment.NewLine, text.Split(Environment.NewLine)[..Height]);
            }
            catch (ArgumentOutOfRangeException)
            {
                return text;
            }
        }

        public void Write(string text)
        {
            Console.BackgroundColor = Console.ForegroundColor = ParentWindow.Colors.Background;
            Console.SetCursorPosition(Left, Top);
            Console.Write(Text);
            Foreground = ParentWindow.Colors.Foreground;
            Text = Trim(text);
            Display();
        }

        public void Append(string text)
        {
            Text = Trim(Text + text);
            Display();
        }

        public void Error(string text)
        {
            Console.BackgroundColor = Console.ForegroundColor = ParentWindow.Colors.Background;
            Console.SetCursorPosition(Left, Top);
            Console.Write(Text);
            Foreground = ParentWindow.Colors.ForegroundError;
            Text = Trim(text);
            Display();
        }
    }

    /// <summary>
    /// Base class for all interactable (an thus, focusable) elements
    /// </summary>
    public abstract class Focusable : Element
    {
        protected internal abstract void Focus();
        protected internal abstract void Unfocus();

        protected internal virtual bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            Focusable newEl;

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    newEl = ParentWindow.PreviousFocusable();
                    break;
                case ConsoleKey.DownArrow:
                    newEl = ParentWindow.NextFocusable();
                    break;
                default:
                    return false;
            }

            if (newEl != null)
            {
                ParentWindow.Focused = newEl;
            }

            return true;
        }
    }

    /// <summary>
    /// Class for simple buttons
    /// </summary>
    public class ButtonElement : Focusable
    {
        /// <summary>
        /// Button label
        /// </summary>
        public string Text { get; }

        private Action<ButtonElement> OnChoice { get; }
        private ConsoleColor Foreground { get; set; }
        private ConsoleColor Background { get; set; }
        private bool Focused { get; set; }

        public ButtonElement(string text, Action<ButtonElement> onChoice)
        {
            Text = text.Split(Environment.NewLine)[0];
            OnChoice = onChoice;
        }

        public override void Reset()
        {
            Unfocus();
            base.Reset();
        }

        protected internal override void ToggleWindow(Window window)
        {
            base.ToggleWindow(window);

            Foreground = ParentWindow.Colors.ForegroundFocusable;
            Background = ParentWindow.Colors.BackgroundFocusable;
        }

        protected internal override void Display()
        {
            Console.BackgroundColor = Background;
            Console.ForegroundColor = Foreground;
            Console.CursorVisible = false;
            Console.SetCursorPosition(Left, Top);
            Console.Write(Text);
        }

        public override int Width => Text.Length;
        public override int Height => 1;

        protected internal override void Focus()
        {
            if (Focused)
            {
                return;
            }

            Focused = true;
            Background = ParentWindow.Colors.BackgroundFocused;
            Foreground = ParentWindow.Colors.ForegroundFocused;
            Display();
        }

        protected internal override void Unfocus()
        {
            if (!Focused)
            {
                return;
            }

            Focused = false;
            Background = ParentWindow.Colors.BackgroundFocusable;
            Foreground = ParentWindow.Colors.ForegroundFocusable;
            Display();
        }

        /// <summary>
        /// Simulate a press on the button
        /// </summary>
        public void Invoke()
        {
            OnChoice(this);
        }

        protected internal override bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (base.HandleKey(keyInfo))
            {
                return true;
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                try
                {
                    Invoke();
                }
                catch (Exception e)
                {
                    ParentWindow.Error(e.Message);
                }

                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Class for input line
    /// </summary>
    public class InputElement : Focusable
    {
        private enum Status
        {
            Unfocused,
            Focused,
            Activated
        }

        private StringBuilder _text = new StringBuilder();
        private Status _status = Status.Unfocused;
        public string Text => _text.ToString();
        private ConsoleColor Foreground { get; set; }
        private ConsoleColor Background { get; set; }

        public override void Reset()
        {
            _text.Clear();
            base.Reset();
        }

        protected internal override void Display()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ParentWindow.Colors.Background;
            Console.ForegroundColor = ParentWindow.Colors.Background;
            Console.SetCursorPosition(Left, Top);
            Console.Write($"{Text}aaaaa");

            Console.ForegroundColor = ParentWindow.Colors.Foreground;
            Console.SetCursorPosition(Left, Top);
            Console.Write($">: ");

            Console.BackgroundColor = Background;
            Console.ForegroundColor = Foreground;
            Console.Write(Text);
            Console.Write(" ");
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            if (_status == Status.Activated)
            {
                Console.CursorVisible = true;
            }
        }

        public override int Width => Text.Length + 3;
        public override int Height => 1;

        protected internal override void ToggleWindow(Window window)
        {
            base.ToggleWindow(window);
            Foreground = ParentWindow.Colors.ForegroundFocusable;
            Background = ParentWindow.Colors.BackgroundFocusable;
        }

        protected internal override void Focus()
        {
            if (_status == Status.Focused)
            {
                return;
            }

            _status = Status.Focused;
            Foreground = ParentWindow.Colors.ForegroundFocused;
            Background = ParentWindow.Colors.BackgroundFocused;
            Display();
        }

        protected internal override void Unfocus()
        {
            if (_status == Status.Unfocused)
            {
                return;
            }

            _status = Status.Unfocused;
            Foreground = ParentWindow.Colors.ForegroundFocusable;
            Background = ParentWindow.Colors.BackgroundFocusable;
            Display();
        }

        private void Activate()
        {
            if (_status == Status.Activated)
            {
                return;
            }

            _status = Status.Activated;
            Foreground = ParentWindow.Colors.ForegroundActivated;
            Background = ParentWindow.Colors.BackgroundActivated;
            Display();
        }

        private bool HandleKeyActivated(ConsoleKeyInfo keyInfo, bool check = true)
        {
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Focus();
                return true;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (_text.Length == 0)
                    return false;
                _text.Remove(_text.Length - 1, 1);
                Display();
                return true;
            }

            if (keyInfo.KeyChar != 0)
            {
                _text.Append(keyInfo.KeyChar);
                Display();
                return true;
            }

            if (!check)
            {
                return false;
            }

            if (HandleKeyFocused(keyInfo, false))
            {
                Unfocus();
                return true;
            }

            return false;
        }

        private bool HandleKeyFocused(ConsoleKeyInfo keyInfo, bool check = true)
        {
            if (base.HandleKey(keyInfo))
            {
                return true;
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Activate();
                return true;
            }

            if (!check)
            {
                return false;
            }

            if (HandleKeyActivated(keyInfo, false))
            {
                Activate();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears input
        /// </summary>
        public void Clear()
        {
            _text.Clear();
            ClearLine();
            Display();
        }

        private void ClearLine()
        {
            Console.ForegroundColor = Console.BackgroundColor = ParentWindow.Colors.Background;
            Console.CursorVisible = false;
            Console.SetCursorPosition(Left, Top);
            for (int i = Left; i < Console.BufferWidth; i++)
            {
                Console.Write(" ");
            }
        }

        protected internal override bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            try
            {
                if (_status == Status.Activated)
                {
                    return HandleKeyActivated(keyInfo);
                }

                return HandleKeyFocused(keyInfo);
            }
            catch (ArgumentOutOfRangeException)
            {
                ParentWindow.Error("Input is too long!");
                _text.Remove(_text.Length - 1, 1);
                Display();
            }

            return true;
        }
    }

    /// <summary>
    /// Class for multiple choice element
    /// </summary>
    public class SwitchElement : Focusable
    {
        private string[] _choices;
        public int ChosenIndex { get; private set; }
        public string Chosen => _choices[ChosenIndex];
        private bool Focused { get; set; }

        public SwitchElement(string[] choices, int chosenIndex = 0)
        {
            if (choices.Length == 0)
            {
                throw new ArgumentException("Empty choices array");
            }

            _choices = new string[choices.Length];
            for (int i = 0; i < choices.Length; i++)
            {
                _choices[i] = choices[i];
            }

            ChosenIndex = chosenIndex;
        }

        public override int Height => 1;
        public override int Width => string.Join(' ', _choices).Length;

        private void ClearLine()
        {
            Console.BackgroundColor = Console.ForegroundColor = ParentWindow.Colors.Background;
            Console.SetCursorPosition(Left, Top);

            StringBuilder s = new StringBuilder();

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                s.Append('+');
            }

            Console.Write(s);
        }

        protected internal override void Display()
        {
            ClearLine();

            Console.SetCursorPosition(Left, Top);

            ConsoleColor background = ParentWindow.Colors.BackgroundFocusable;
            ConsoleColor backgroundChosen;
            ConsoleColor foreground = ParentWindow.Colors.ForegroundFocusable;
            ConsoleColor foregroundChosen;

            if (!Focused)
            {
                backgroundChosen = ParentWindow.Colors.BackgroundFocusable;
                foregroundChosen = ParentWindow.Colors.ForegroundChosen;
            }
            else
            {
                backgroundChosen = ParentWindow.Colors.BackgroundFocused;
                foregroundChosen = ParentWindow.Colors.ForegroundFocused;
            }

            Console.CursorVisible = false;
            Console.SetCursorPosition(Left, Top);

            for (int i = 0; i < _choices.Length; i++)
            {
                if (i == ChosenIndex)
                {
                    Console.BackgroundColor = backgroundChosen;
                    Console.ForegroundColor = foregroundChosen;
                }
                else
                {
                    Console.BackgroundColor = background;
                    Console.ForegroundColor = foreground;
                }

                Console.Write(_choices[i]);
                Console.Write(' ');
            }
        }

        protected internal override void Focus()
        {
            Focused = true;
            Display();
        }

        protected internal override void Unfocus()
        {
            Focused = false;
            Display();
        }

        public override void Reset()
        {
            ChosenIndex = 0;
            base.Reset();
        }

        protected internal override bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (base.HandleKey(keyInfo))
            {
                return true;
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (ChosenIndex != 0)
                    {
                        ChosenIndex--;
                        Display();
                    }

                    return true;
                case ConsoleKey.RightArrow:
                    if (ChosenIndex != _choices.Length - 1)
                    {
                        ChosenIndex++;
                        Display();
                    }

                    return true;
                default:
                    return false;
            }
        }
    }
}