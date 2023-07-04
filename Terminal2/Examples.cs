using System;
using System.Collections.Generic;
using System.Linq;

namespace Terminal
{
    public abstract class Examples
    {
        public static void ExampleClicker()
        {
            WindowManager app = new WindowManager();
            ColorScheme scheme = ColorScheme.GreenTheme;

            WindowProps props = new WindowProps
            (
                60,
                30,
                1,
                2
            );

            TextElement title;
            ButtonElement choice1;
            TextElement output1 = null;
            ButtonElement choice2 = null;
            TextElement output2 = null;

            title = new TextElement("My terminal program", 2);
            choice1 = new ButtonElement("Button 1", element => output1.Write(
                string.Join(" ", output1.Text.Trim().Split()[..^1]) + " " +
                (int.Parse(output1.Text.Trim().Split()[^1]) + 1)
            ));
            output1 = new TextElement("Button 1 pressed: 0");
            choice2 = new ButtonElement("Button 2", element => output2.Write(
                string.Join(" ", output2.Text.Trim().Split()[..^1]) + " " +
                (int.Parse(output2.Text.Trim().Split()[^1]) + 1)
            ));

            output2 = new TextElement("Button 2 pressed: 0");

            Window mainWindow = new Window
            (
                scheme, props,
                title, choice1, output1, choice2, output2
            );
            app.Running = mainWindow;
            app.Serve();
        }

        public static void ExampleEcho()
        {
            WindowManager app = new WindowManager();

            ColorScheme scheme = ColorScheme.GreenTheme;

            WindowProps props = new WindowProps
            (
                60,
                30,
                1,
                2
            );

            TextElement title = null;
            ButtonElement print = null;
            InputElement input = null;
            TextElement output = null;

            title = new TextElement("Echo terminal", 2);
            print = new ButtonElement("Print", element =>
            {
                output.Write($"You've written: {input.Text}");
                input.Clear();
            });
            input = new InputElement();
            output = new TextElement("You have not printed anything yet");

            Window mainWindow = new Window
            (
                scheme, props,
                new ButtonElement("Exit", element => app.Running = null).WithTag("Exit"),
                title,
                input,
                print, output
            );

            app.Running = mainWindow;
            app.Serve();
        }

        public static void ExampleCalculator()
        {
            WindowManager app = new WindowManager();

            ColorScheme scheme = ColorScheme.GreenTheme;

            WindowProps props = new WindowProps
            (
                60,
                30,
                1,
                2
            );

            Window mainWindow = null;
            Window addWindow = null;
            Window subWindow = null;
            Window mulWindow = null;
            Window divWindow = null;
            Window powWindow = null;

            Action<Window> reset = w => Array.ForEach(w.Elements, e => e.Reset());

            {
                Element back = new ButtonElement("Back", e => app.Running = mainWindow).WithTag("Exit");
                TextElement title = new TextElement("Addition", 2);
                TextElement operand1Label = new TextElement("Number 1:");
                InputElement operand1 = new InputElement();
                TextElement operand2Label = new TextElement("Number 2:");
                InputElement operand2 = new InputElement();
                TextElement res = new TextElement("", 1, "");
                ButtonElement calculate = new ButtonElement("Calculate",
                    e => res.Write((double.Parse(operand1.Text) + double.Parse(operand2.Text)).ToString()));
                addWindow = new Window
                (
                    scheme, props, reset,
                    back,
                    TextElement.Skip(1),
                    title,
                    operand1Label,
                    operand1,
                    operand2Label,
                    operand2,
                    TextElement.Skip(1),
                    calculate,
                    res
                );
            }

            {
                Element back = new ButtonElement("Back", e => app.Running = mainWindow).WithTag("Exit");
                TextElement title = new TextElement("Subtraction", 2);
                TextElement operand1Label = new TextElement("Number 1:");
                InputElement operand1 = new InputElement();
                TextElement operand2Label = new TextElement("Number 2:");
                InputElement operand2 = new InputElement();
                TextElement res = new TextElement("", 1, "");
                ButtonElement calculate = new ButtonElement("Calculate",
                    e => res.Write((double.Parse(operand1.Text) - double.Parse(operand2.Text)).ToString()));
                subWindow = new Window
                (
                    scheme, props, reset,
                    back,
                    TextElement.Skip(1),
                    title,
                    operand1Label,
                    operand1,
                    operand2Label,
                    operand2,
                    TextElement.Skip(1),
                    calculate,
                    res
                );
            }
            {
                Element back = new ButtonElement("Back", e => app.Running = mainWindow).WithTag("Exit");
                TextElement title = new TextElement("Multiplication", 2);
                TextElement operand1Label = new TextElement("Number 1:");
                InputElement operand1 = new InputElement();
                TextElement operand2Label = new TextElement("Number 2:");
                InputElement operand2 = new InputElement();
                TextElement res = new TextElement("", 1, "");
                ButtonElement calculate = new ButtonElement("Calculate",
                    e => res.Write((double.Parse(operand1.Text) * double.Parse(operand2.Text)).ToString()));
                mulWindow = new Window
                (
                    scheme, props, reset,
                    back,
                    TextElement.Skip(1),
                    title,
                    operand1Label,
                    operand1,
                    operand2Label,
                    operand2,
                    TextElement.Skip(1),
                    calculate,
                    res
                );
            }

            {
                Element back = new ButtonElement("Back", e => app.Running = mainWindow).WithTag("Exit");
                TextElement title = new TextElement("Division", 2);
                TextElement operand1Label = new TextElement("Number 1:");
                InputElement operand1 = new InputElement();
                TextElement operand2Label = new TextElement("Number 2:");
                InputElement operand2 = new InputElement();
                TextElement res = new TextElement("", 1, "");
                ButtonElement calculate = new ButtonElement("Calculate",
                    e => res.Write(double.Parse(operand2.Text) == 0
                        ? double.NaN.ToString()
                        : (double.Parse(operand1.Text) / double.Parse(operand2.Text)).ToString()));
                divWindow = new Window
                (
                    scheme, props, reset,
                    back,
                    TextElement.Skip(1),
                    title,
                    operand1Label,
                    operand1,
                    operand2Label,
                    operand2,
                    TextElement.Skip(1),
                    calculate,
                    res
                );
            }

            {
                Element back = new ButtonElement("Back", e => app.Running = mainWindow).WithTag("Exit");
                TextElement title = new TextElement("Power", 2);
                TextElement operand1Label = new TextElement("Number 1:");
                InputElement operand1 = new InputElement();
                TextElement operand2Label = new TextElement("Number 2:");
                InputElement operand2 = new InputElement();
                TextElement res = new TextElement("", 1, "");
                ButtonElement calculate = new ButtonElement("Calculate",
                    e => res.Write(Math.Pow(double.Parse(operand1.Text), double.Parse(operand2.Text)).ToString()));
                powWindow = new Window
                (
                    scheme, props, reset,
                    back,
                    TextElement.Skip(1),
                    title,
                    operand1Label,
                    operand1,
                    operand2Label,
                    operand2,
                    TextElement.Skip(1),
                    calculate,
                    res
                );
            }

            {
                Element exit = new ButtonElement("Exit", e => app.Running = null).WithTag("Exit");
                TextElement title = new TextElement("Calculator", 2);
                ButtonElement add = new ButtonElement("Addition", e => app.Running = addWindow);
                ButtonElement sub = new ButtonElement("Subtraction", e => app.Running = subWindow);
                ButtonElement mul = new ButtonElement("Multiplication", e => app.Running = mulWindow);
                ButtonElement div = new ButtonElement("Division", e => app.Running = divWindow);
                ButtonElement pow = new ButtonElement("Power", e => app.Running = powWindow);
                mainWindow = new Window
                (
                    scheme, props,
                    exit,
                    TextElement.Skip(1),
                    title,
                    add,
                    sub,
                    mul,
                    div,
                    pow
                );
            }

            app.Running = mainWindow;
            app.Serve();
        }

        public static void ExampleSwitch()
        {
            WindowManager app = new WindowManager();
            ColorScheme scheme = ColorScheme.GreenTheme;
            WindowProps props = new WindowProps
            (
                60,
                30,
                1,
                2
            );

            Window mainWindow = null;
            TextElement title = new TextElement("Switch!", 2);
            SwitchElement switchElement = new SwitchElement(new[] { "Choice 1", "Choice 2", "Choice 3" });
            TextElement output = new TextElement("", "");
            ButtonElement print =
                new ButtonElement("Print", e => output.Write($"You've chosen: {switchElement.Chosen}"));

            mainWindow = new Window
            (
                scheme, props,
                title,
                switchElement,
                print,
                output
            );
            app.Running = mainWindow;
            app.Serve();
        }

        public static void ExampleDynamic()
        {
            WindowManager app = new WindowManager();
            ColorScheme scheme = ColorScheme.DarkBlueTheme;
            WindowProps props = new WindowProps
            (
                60,
                30,
                1,
                2
            );

            Window mainWindow = null;

            TextElement title = new TextElement("Add multiple numbers", 2);
            int i = 1;

            ButtonElement add = new ButtonElement("Add", e =>
            {
                Element label = new TextElement($"Number {i++}:").WithTag("Label");
                Element input = new InputElement().WithTag("Input");
                mainWindow.Add(label, e);
                mainWindow.Add(input, e);
            });
            ButtonElement remove = new ButtonElement("Remove", e =>
            {
                mainWindow.Remove(new List<Element>(mainWindow.Elements.Where(t => t.Tags.Contains("Label")))[^1]);
                mainWindow.Remove(new List<Element>(mainWindow.Elements.Where(t => t.Tags.Contains("Input")))[^1]);
                i--;
            });

            TextElement output = new TextElement("", "");

            ButtonElement calculate = new ButtonElement("Calculate", e =>
            {
                double res = 0;
                foreach (Element element in mainWindow.Elements.Where(t => t.Tags.Contains("Input")))
                {
                    InputElement input = element as InputElement;
                    res += double.Parse(input.Text);
                }

                output.Write(res.ToString());
            });

            mainWindow = new Window
            (
                scheme, props,
                title,
                add,
                remove,
                TextElement.Skip(),
                calculate,
                output
            );

            app.Running = mainWindow;
            add.Invoke();
            add.Invoke();
            app.Serve();
        }

        public static void ExampleCoffeeMachine()
        {
            WindowManager app = new WindowManager();
            ColorScheme scheme = ColorScheme.BlackTheme;
            WindowProps props = new WindowProps
            (
                60,
                30,
                1,
                2
            );

            List<Element> elements = new List<Element>();

            int insertedAmount = 0;
            TextElement inserted = new TextElement("Внесено: 0");

            Dictionary<String, int> choices = new Dictionary<string, int>();
            choices["Эспрессо"] = 25;
            choices["Двойной эспрессо"] = 45;
            choices["Американо"] = 35;
            choices["Капучино"] = 40;
            choices["Латте"] = 40;

            String chosen = null;
            TextElement choice = new TextElement("Выберите напиток");

            {
                TextElement label = new TextElement("Купюроприемник");
                TextElement nominalLabel = new TextElement("Внести сумму:");
                SwitchElement nominal = new SwitchElement(
                    new[] { "1", "2", "5", "10", "50", "100", "200", "500" }
                );

                ButtonElement insert = new ButtonElement("Внести", element =>
                {
                    insertedAmount += int.Parse(nominal.Chosen);
                    inserted.Write($"Внесено: {insertedAmount}");
                });

                ButtonElement change = new ButtonElement("Выдать сдачу", element =>
                {
                    insertedAmount = 0;
                    inserted.Reset();
                });

                elements.Add(label);
                elements.Add(TextElement.Skip());
                elements.Add(nominalLabel);
                elements.Add(nominal);
                elements.Add(insert);
                elements.Add(change);
                elements.Add(inserted);
                elements.Add(TextElement.Skip());
            }
            {
                TextElement label = new TextElement("Выбор напитка");


                elements.Add(label);
                elements.Add(TextElement.Skip());
                elements.Add(choice);

                foreach (var key in choices.Keys)
                {
                    elements.Add(new ButtonElement($"{key}, {choices[key]} руб.", element =>
                    {
                        chosen = key;
                        choice.Write($"Выбрано: {key}");
                    }));
                }

                elements.Add(new ButtonElement("Отмена", element =>
                {
                    chosen = null;
                    choice.Reset();
                }));
                elements.Add(TextElement.Skip());
            }
            {
                TextElement label = new TextElement("Диспенсер");
                TextElement status = new TextElement("Статус: свободно");

                ButtonElement start = null;
                ButtonElement wait = null;
                ButtonElement take = null;

                start = new ButtonElement("Приготовить", element =>
                {
                    if (chosen == null)
                    {
                        element.ParentWindow.Error("ОШИБКА: напиток не выбран");
                        return;
                    }

                    if (insertedAmount < choices[chosen])
                    {
                        element.ParentWindow.Error("ОШИБКА: недостаточно средств");
                        return;
                    }
                    element.ParentWindow.Error("");  // Очистить строку ошибки 

                    insertedAmount -= choices[chosen];
                    inserted.Write($"Внесено: {insertedAmount}");

                    status.Write($"Статус: готовится {chosen}");
                    
                    element.ParentWindow.Add(wait);
                    element.ParentWindow.Remove(element, wait);
                });

                wait = new ButtonElement("Подождать", element =>
                {
                    status.Write($"Статус: {chosen} готов");
                    element.ParentWindow.Add(take);
                    element.ParentWindow.Remove(element, take);
                });

                take = new ButtonElement("Взять напиток", element =>
                {
                    status.Reset();
                    chosen = null;
                    choice.Reset();

                    element.ParentWindow.Add(start);
                    element.ParentWindow.Remove(element, start);
                });
                
                elements.Add(label);
                elements.Add(TextElement.Skip());
                elements.Add(status);
                elements.Add(start);
            }

            Window mainWindow = new Window(scheme, props, elements.ToArray());
            app.Running = mainWindow;
            app.Serve();
        }

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid args");
                return;
            }

            switch (args[0].ToLower())
            {
                case "clicker":
                    ExampleClicker();
                    break;
                case "echo":
                    ExampleEcho();
                    break;
                case "calculator":
                    ExampleCalculator();
                    break;
                case "switch":
                    ExampleSwitch();
                    break;
                case "dynamic":
                    ExampleDynamic();
                    break;
                case "coffee":
                    ExampleCoffeeMachine();
                    break;
                default:
                    Console.WriteLine("No such example");
                    break;
            }
        }
    }
}