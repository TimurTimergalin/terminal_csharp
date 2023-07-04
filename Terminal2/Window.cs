using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terminal
{
    public struct ColorScheme
    {
        /// <summary>
        /// Defines standard background
        /// </summary>
        public ConsoleColor Background { get; }

        /// <summary>
        /// Defines standard foreground
        /// </summary>
        public ConsoleColor Foreground { get; }

        /// <summary>
        /// Define background of aby Focusable element when it is focused 
        /// </summary>
        /// <seealso cref="Focusable"/>
        public ConsoleColor BackgroundFocused { get; }

        /// <summary>
        /// Define foreground of aby Focusable element when it is focused 
        /// </summary>
        /// <seealso cref="Focusable"/>
        public ConsoleColor ForegroundFocused { get; }

        /// <summary>
        /// Define foreground of error messages
        /// </summary>
        public ConsoleColor ForegroundError { get; }

        /// <summary>
        /// Define background of aby Focusable element when it is not focused 
        /// </summary>
        /// <seealso cref="Focusable"/>
        public ConsoleColor BackgroundFocusable { get; }

        /// <summary>
        /// Define foreground of aby Focusable element when it is not focused 
        /// </summary>
        /// <seealso cref="Focusable"/>
        public ConsoleColor ForegroundFocusable { get; }

        /// <summary>
        /// Defines a background of InputElement when it is being used
        /// </summary>
        /// <seealso cref="InputElement"/>
        public ConsoleColor BackgroundActivated { get; }

        /// <summary>
        /// Defines a froeground of InputElement when it is being used
        /// </summary>
        /// <seealso cref="InputElement"/>
        public ConsoleColor ForegroundActivated { get; }

        /// <summary>
        /// Define a foreground color of a chosen option in SwitchElement
        /// </summary>
        /// <seealso cref="SwitchElement"/>
        public ConsoleColor ForegroundChosen { get; }

        public ColorScheme(ConsoleColor background, ConsoleColor foreground, ConsoleColor backgroundFocused,
            ConsoleColor foregroundFocused, ConsoleColor foregroundError, ConsoleColor backgroundFocusable,
            ConsoleColor foregroundFocusable, ConsoleColor backgroundActivated, ConsoleColor foregroundActivated,
            ConsoleColor foregroundChosen)
        {
            Background = background;
            Foreground = foreground;
            BackgroundFocused = backgroundFocused;
            ForegroundFocused = foregroundFocused;
            ForegroundError = foregroundError;
            BackgroundFocusable = backgroundFocusable;
            ForegroundFocusable = foregroundFocusable;
            BackgroundActivated = backgroundActivated;
            ForegroundActivated = foregroundActivated;
            ForegroundChosen = foregroundChosen;
        }

        public static ColorScheme BlackTheme => new
        (
            ConsoleColor.Black,
            ConsoleColor.White,
            ConsoleColor.White,
            ConsoleColor.Black,
            ConsoleColor.Red,
            ConsoleColor.DarkGray,
            ConsoleColor.White,
            ConsoleColor.Gray,
            ConsoleColor.Black,
            ConsoleColor.Black
        );

        public static ColorScheme WhiteTheme => new
        (
            ConsoleColor.White,
            ConsoleColor.Black,
            ConsoleColor.Black,
            ConsoleColor.White,
            ConsoleColor.DarkRed,
            ConsoleColor.Gray,
            ConsoleColor.Black,
            ConsoleColor.DarkGray,
            ConsoleColor.White,
            ConsoleColor.White
        );

        public static ColorScheme LightBlueTheme => new
        (
            ConsoleColor.Cyan,
            ConsoleColor.Black,
            ConsoleColor.DarkBlue,
            ConsoleColor.White,
            ConsoleColor.DarkMagenta,
            ConsoleColor.DarkCyan,
            ConsoleColor.Black,
            ConsoleColor.Blue,
            ConsoleColor.White,
            ConsoleColor.White
        );

        public static ColorScheme DarkBlueTheme => new
        (
            ConsoleColor.DarkBlue,
            ConsoleColor.White,
            ConsoleColor.Cyan,
            ConsoleColor.Black,
            ConsoleColor.Red,
            ConsoleColor.Blue,
            ConsoleColor.White,
            ConsoleColor.DarkCyan,
            ConsoleColor.Black,
            ConsoleColor.Black
        );

        public static ColorScheme GreenTheme => new
        (
            ConsoleColor.Black,
            ConsoleColor.Green,
            ConsoleColor.Green,
            ConsoleColor.White,
            ConsoleColor.Red,
            ConsoleColor.DarkGreen,
            ConsoleColor.Black,
            ConsoleColor.Green,
            ConsoleColor.Black,
            ConsoleColor.White
        );
    }

    /// <summary>
    /// Container for Window's size properties
    /// </summary>
    /// <seealso cref="Window"/>
    public struct WindowProps
    {
        public int Width { get; }
        public int Height { get; }
        public int TopMargin { get; }
        public int LeftMargin { get; }

        public WindowProps(int width, int height, int topMargin, int leftMargin)
        {
            Width = width;
            Height = height;
            TopMargin = topMargin;
            LeftMargin = leftMargin;
        }
    }

    /// <summary>
    /// Container for elements to be displayed on the screen
    /// </summary>
    /// <seealso cref="Element"/>
    public class Window
    {
        public ColorScheme Colors { get; }
        public WindowProps Props { get; }

        private List<Element> _Elements { get; }
        public Element[] Elements => _Elements.ToArray();
        public Action<Window> OnStop { get; }

        /// <summary>
        /// Holds an element that is currently focused
        /// </summary>
        public Focusable Focused
        {
            get => FocusedIndex == -1 ? null : (Focusable)_Elements[FocusedIndex];
            set
            {
                if (value == null)
                {
                    Focused?.Unfocus();
                    FocusedIndex = -1;
                    return;
                }

                for (int i = 0; i < _Elements.Count; i++)
                {
                    if (_Elements[i] == value)
                    {
                        Focused?.Unfocus();
                        FocusedIndex = i;
                        Focused?.Focus();
                    }
                }
            }
        }

        internal Focusable NextFocusable()
        {
            for (int i = FocusedIndex + 1; i < _Elements.Count; i++)
            {
                Element cur = _Elements[i];
                if (cur is Focusable focusable)
                {
                    return focusable;
                }
            }

            return null;
        }

        internal Focusable PreviousFocusable()
        {
            for (int i = FocusedIndex - 1; i >= 0; i--)
            {
                Element cur = _Elements[i];
                if (cur is Focusable focusable)
                {
                    return focusable;
                }
            }

            return null;
        }

        private int FocusedIndex { get; set; } = -1;

        public Window(ColorScheme colors, WindowProps props, Action<Window> onStop,
            params Element[] elements)
        {
            Colors = colors;
            Props = props;
            OnStop = onStop;
            _Elements = new List<Element>(elements);
            _Elements.Insert(0, new TextElement("", ""));

            foreach (Element element in _Elements)
            {
                element.ToggleWindow(this);
            }

            RecalculateCords();
        }

        public Window(ColorScheme colors, WindowProps props,
            params Element[] elements) : this(colors, props, null, elements)
        {
        }

        /// <summary>
        /// Prints error message at the top of the window
        /// </summary>
        /// <param name="text">Text of the message to print</param>
        public void Error(string text)
        {
            ((TextElement)_Elements[0]).Error(text);
        }

        private void FocusFirst()
        {
            foreach (Element element in _Elements)
            {
                if (element is Focusable focusable)
                {
                    Focused = focusable;
                    break;
                }
            }
        }

        private void RecalculateCords()
        {
            int top = Props.TopMargin;
            foreach (Element element in _Elements)
            {
                element.Top = top;
                element.Left = Props.LeftMargin;
                top += element.Height;
            }
        }

        private void PaintBackground()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = Console.ForegroundColor = Colors.Background;
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                for (int j = 0; j < Console.WindowWidth; j++)
                {
                    s.Append('+');
                }

                s.Append('\n');
            }

            s.Remove(s.Length - 1, 1);
            Console.Write(s);
        }

        private void Display()
        {
            PaintBackground();
            foreach (Element element in _Elements)
            {
                element.Display();
            }
        }

        /// <summary>
        /// Insert element into window's structure
        /// </summary>
        /// <param name="el">The element to insert</param>
        /// <param name="before">The element that defines position of a new element - new element will be inserted before this one. If it is null, new element will inserted at the end. </param>
        /// <exception cref="ArgumentException">Element to add is already contained in the window, or before does not belong to this window</exception>
        public void Add(Element el, Element before = null)
        {
            Focusable curFocused = Focused;
            if (_Elements.Contains(el))
            {
                throw new ArgumentException("Such element already exists");
            }

            if (before == null)
            {
                _Elements.Add(el);
                el.ToggleWindow(this);
                FocusedIndex = curFocused == null ? -1 : _Elements.IndexOf(curFocused);
                RecalculateCords();
                Display();
                return;
            }

            if (!_Elements.Contains(before))
            {
                throw new ArgumentException("No such element");
            }

            _Elements.Insert(_Elements.IndexOf(before), el);
            el.ToggleWindow(this);
            FocusedIndex = curFocused == null ? -1 : _Elements.IndexOf(curFocused);
            RecalculateCords();
            Display();
        }


        /// <summary>
        /// Removes element from window's structure
        /// </summary>
        /// <param name="el">Element to remove</param>
        /// <param name="newFocused">Element that will be focused if current focused is removed; If null, nearest to current focused will be chosen instead.</param>
        /// <exception cref="ArgumentException">Such element does not belong to this window</exception>
        public void Remove(Element el, Focusable newFocused = null)
        {
            Focusable curFocused = Focused;
            Focusable replace = newFocused ?? PreviousFocusable() ?? NextFocusable();
            Focused = null;
            if (!_Elements.Remove(el))
            {
                Focused = curFocused;
                throw new ArgumentException("No such element");
            }

            Focused = curFocused == el ? replace : curFocused;
            RecalculateCords();

            Display();
        }

        internal void Start()
        {
            Console.SetWindowSize(Props.Width, Props.Height);
            FocusFirst();
            Display();
        }

        internal bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                try
                {
                    var exits = _Elements.Where(t => t.Tags.Contains("Exit")).GetEnumerator();
                    exits.MoveNext();
                    ((ButtonElement)exits.Current).Invoke();
                    return true;
                }
                catch (NullReferenceException)
                {
                }
                catch (InvalidCastException)
                {
                }
            }

            return Focused != null && Focused.HandleKey(keyInfo);
        }

        internal void Stop()
        {
            OnStop?.Invoke(this);
        }
    }
}