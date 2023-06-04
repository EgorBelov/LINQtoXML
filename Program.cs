using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LINQtoXML
{
    internal class Program
    {
        
        static void Num2()
        {
            string inputFile = "../../input.txt"; // имя исходного текстового файла
            string outputFile = "../../output.xml"; // имя создаваемого XML-документа

            // Чтение строк из текстового файла
            string[] lines = File.ReadAllLines(inputFile);

            // Создание XML-документа с использованием LINQ to XML
            XDocument doc = new XDocument(
                new XElement("root",
                    lines.Select((line, index) =>
                        new XElement("line" + (index + 1), line))
                    )
                );


            Console.WriteLine(doc);
            Console.WriteLine();
            // Сохранение XML-документа в файл
            //doc.Save(outputFile);

            Console.WriteLine("XML-документ успешно создан.");
        }
        
        static void Num12()
        {
            string xmlFilePath = "../../input.xml"; // Путь к XML-документу

            // Загрузка XML-документа
            XDocument doc = XDocument.Load(xmlFilePath);
            Console.WriteLine(doc);
            Console.WriteLine();
            // Получение всех различных имен элементов первого уровня и их количество
            var elementCounts = doc.Root.Descendants()
                .Where(e => e.Parent == doc.Root) // Ограничиваем поиск только элементами первого уровня
                .GroupBy(e => e.Name.LocalName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderBy(e => e.Name);

            // Вывод результатов
            foreach (var element in elementCounts)
            {
                Console.WriteLine($"{element.Name}: {element.Count}");
            }
            
        }
        static void Num22()
        {
            string xmlFilePath = "../../input.xml"; // Путь к XML-документу
            string elementNameToRemove = "element2"; // Имя элемента для удаления

            // Загрузка XML-документа
            XDocument doc = XDocument.Load(xmlFilePath);
            Console.WriteLine(doc);
            Console.WriteLine();
            // Удаление элементов с заданным именем
            doc.Descendants(elementNameToRemove).Remove();

            // Сохранение изменений в XML-документе
            doc.Save("../../input.xml");

            Console.WriteLine(doc);
            Console.WriteLine();
            Console.WriteLine("Элементы с именем \"{0}\" успешно удалены.", elementNameToRemove);
        }

        static void Num32()
        {
            string xmlFilePath = "../../input32.xml"; // Путь к XML-документу
            string elementNameToAdd = "S2"; // Имя элемента, который нужно добавить
            string elementNameToInsertBefore = "S1"; // Имя элемента, перед которым нужно добавить новый элемент

            // Загрузка XML-документа
            XDocument doc = XDocument.Load(xmlFilePath);
            Console.WriteLine(doc);
            Console.WriteLine();
            // Поиск элементов второго уровня с заданным именем
            var elementsToInsertBefore = doc.Root.Elements(elementNameToInsertBefore).ToList();

            // Добавление элементов перед найденными элементами
            foreach (var element in elementsToInsertBefore)
            {
                // Создание нового элемента для добавления
                XElement newElement = new XElement(elementNameToAdd);

                // Проверка, есть ли у элемента дочерние элементы
                if (element.Elements().Any())
                {
                    // Получение последнего атрибута и первого дочернего элемента
                    XAttribute lastAttribute = element.Attributes().LastOrDefault();
                    XElement firstChildElement = element.Elements().FirstOrDefault();

                    // Добавление последнего атрибута к новому элементу
                    if (lastAttribute != null)
                        newElement.Add(lastAttribute);

                    // Добавление первого дочернего элемента к новому элементу
                    if (firstChildElement != null)
                        newElement.Add(firstChildElement);
                }
                else
                {
                    // Создание комбинированного тега, если у элемента нет дочерних элементов
                    newElement.Value = element.Value;
                }

                // Вставка нового элемента перед текущим элементом
                element.AddBeforeSelf(newElement);
            }
            Console.WriteLine(doc);
            Console.WriteLine();
            // Сохранение изменений в XML-документе
            //doc.Save("../../input32.xml");

            Console.WriteLine("Элементы успешно добавлены.");
        }


     
        static void Num42()
        {
            string xmlFilePath = "../../input42.xml"; // Путь к XML-документу

            // Загружаем XML-документ из файла или создаем новый
            XDocument doc = XDocument.Load(xmlFilePath);
            Console.WriteLine(doc);
            Console.WriteLine();
            // Получение элементов первого уровня
            var elements = doc.Root.Elements();

            foreach (var element in elements)
            {
                // Подсчет суммы атрибутов дочерних элементов
                double sum = element.Elements()
                .SelectMany(e => e.Attributes())
                .Sum(attr => double.Parse(attr.Value));

                // Округление суммы до двух десятичных знаков
                sum = Math.Round(sum, 2);

                // Добавление элемента "sum" с текстовым представлением суммы
                element.Add(new XElement("sum", sum.ToString("0.##")));
            }
            Console.WriteLine(doc);
            Console.WriteLine();
            // Сохраняем изменения в файл
            //doc.Save(xmlFilePath);

            Console.WriteLine("XML-документ успешно обработан.");
        }
        static int GetAttributeValueOrDefault(XElement element, string attributeName, int defaultValue)
        {
            XAttribute attribute = element.Attribute(attributeName);
            return attribute != null ? int.Parse(attribute.Value) : defaultValue;
        }
        static void Num52()
        {
            // Загрузка XML-документа из файла или другого источника
            XDocument doc = XDocument.Load("../../input52.xml");
            Console.WriteLine(doc);
            Console.WriteLine();
            var elements = doc.Descendants().ToList();

            foreach (var element in elements)
            {
                int year = GetAttributeValueOrDefault(element, "year", 2000);
                int month = GetAttributeValueOrDefault(element, "month", 1);
                int day = GetAttributeValueOrDefault(element, "day", 10);

                XElement dateElement = new XElement("date", new DateTime(year, month, day).ToString("yyyy-MM-dd"));

                element.AddFirst(dateElement);

                element.Attributes()
                       .Where(attr => attr.Name.LocalName == "year" ||
                                      attr.Name.LocalName == "month" ||
                                      attr.Name.LocalName == "day")
                       .ToList()
                       .ForEach(attr => attr.Remove());
            }
            Console.WriteLine(doc);
            Console.WriteLine();
            // Сохранение измененного XML-документа
            //doc.Save("../../input52.xml");
            Console.WriteLine("XML-документ успешно обработан.");
        }

     
        static void Num62()
        {
            string filePath = "../../input62.xml";
            XDocument doc = XDocument.Load(filePath);
            Console.WriteLine(doc);
            Console.WriteLine();
            var transformedElements = doc.Root
                .Elements()
                .OrderBy(e => int.Parse(e.Attribute("id").Value))
                .ThenBy(e => GetDateTimeFromElement(e))
                .Select((e, index) => CreateTransformedElement(e, index + 1))
                .ToList();

            doc.Root.ReplaceAll(transformedElements);
            Console.WriteLine(doc);
            //doc.Save(filePath);

            Console.WriteLine("XML-документ успешно обработан и сохранен.");
        }
        static DateTime GetDateTimeFromElement(XElement element)
        {
            int year = int.Parse(element.Attribute("year").Value);
            int month = int.Parse(element.Attribute("month").Value);
            return new DateTime(year, month, 1);
        }

        static XElement CreateTransformedElement(XElement element, int index)
        {
            DateTime date = GetDateTimeFromElement(element);
            string clientId = "id" + element.Attribute("id").Value;
            int minutes = ParseDuration(element.Value);

            return new XElement(clientId,
                new XAttribute("date", date.ToString("yyyy-MM-01T00:00:00")),
                minutes);
        }




        static int ParseDuration(string duration)
        {
            string[] parts = duration.TrimStart('P', 'T').Split(new[] { 'H', 'M' }, StringSplitOptions.RemoveEmptyEntries);

            int hours = 0;
            int minutes = 0;

            if (parts.Length > 0)
            {
                hours = int.Parse(parts[0]);
            }

            if (parts.Length > 1)
            {
                minutes = int.Parse(parts[1]);
            }

            return hours * 60 + minutes;
        }

        static void Num72()
        {
            string filePath = "../../input72.xml";
            // Загрузка XML-документа
            XDocument doc = XDocument.Load(filePath);
            Console.WriteLine(doc);
            Console.WriteLine();
            // Выполнение группировки данных по названиям улиц и маркам бензина
            var groupedData = doc.Descendants("price")
                .GroupBy(e => new { Street = e.Attribute("street").Value, Brand = e.Attribute("brand").Value })
                .Select(g => new
                {
                    Street = g.Key.Street,
                    Brand = g.Key.Brand,
                    MinPrice = g.Min(e => int.Parse(e.Value)),
                    Company = g.First(f => int.Parse(f.Value) == g.Min(e => int.Parse(e.Value))).Parent.Name.ToString()
                })
                .OrderBy(g => g.Street)
                .ThenBy(g => g.Brand);

            // Создание нового XML-документа
            XDocument newDoc = new XDocument(
                new XElement("Root",
                    groupedData
                    .GroupBy(g => g.Street)
                    .Select(g => new XElement(g.Key,
                        g.Select(d => new XElement("b" + d.Brand,
                            new XElement("min-price",
                                new XAttribute("company", d.Company),
                                d.MinPrice)))))));

            // Сохранение нового XML-документа
            Console.WriteLine(newDoc);


            Console.WriteLine("XML-документ успешно обработан и сохранен.");
        }
       

        static void Num82()
        {

            string filePath = "../../input82.xml";
            XDocument document = XDocument.Load(filePath);
            Console.WriteLine(document);
            Console.WriteLine();
            // Выполнение группировки данных по номеру дома и номеру этажа
            var groupedData = document
                .Descendants()
                .Where(e => e.Name.LocalName.StartsWith("addr"))
                .GroupBy(e => new
                {
                    HouseNumber = e.Name.LocalName.Substring(4, e.Name.LocalName.IndexOf("-") - 4),
                    FloorNumber = e.Name.LocalName.Substring(e.Name.LocalName.IndexOf("-") + 1)
                })
                .OrderBy(g => g.Key.HouseNumber)
                .ThenBy(g => g.Key.FloorNumber);

            // Создание нового XML-документа с преобразованными данными
            XDocument transformedDocument = new XDocument(
                new XElement("root",
                    groupedData.Select(g =>
                        new XElement($"house{g.Key.HouseNumber}",
                            g.Select(e =>
                                new XElement($"floor{g.Key.FloorNumber}",
                                    new XAttribute("count", g.Count()),
                                    new XAttribute("total-debt", Math.Round(g.Sum(x => ParseDecimal(x.Value)), 2))
                                )
                            )
                        )
                    )
                )
            );

            // Сохранение преобразованного XML-документа в файл или другой источник
            Console.WriteLine(transformedDocument);

            Console.WriteLine("XML-документ успешно преобразован и сохранен.");
        }
        static decimal ParseDecimal(string value)
        {
            return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
        
        
        
        static void Num61()
        {
            XDocument xdoc = XDocument.Load("../../input61.xml");
            Console.WriteLine(xdoc);

            List<XElement> listElements = xdoc.Root.Elements().ToList();

            for (int i = 0; i < listElements.Count; i++)
            {
                var elem1 = listElements[i];
                var elements2arr = new XElement[elem1.Elements().Count()];
                elem1.Elements().ToList().CopyTo(elements2arr);

                var year = DateTime.Parse(elements2arr[1].Value).Year;
                var month = DateTime.Parse(elements2arr[1].Value).Month;

                var listAttributes = new List<XAttribute>();
                listAttributes.Add(new XAttribute("id", elements2arr[0].Value));
                listAttributes.Add(new XAttribute("year", year));
                listAttributes.Add(new XAttribute("month", month));

                elem1.AddBeforeSelf(new XElement("time", elements2arr[2].Value, listAttributes));
                elem1.Remove();
            }

            Console.WriteLine();
            Console.WriteLine(xdoc);
        }


        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Введите номер задачи (от 0 до 9)");
                switch (Console.ReadLine())
                {
                    case "1":
                        Num2();
                        Console.ReadLine();
                        break;
                    case "2":
                        Num12();
                        Console.ReadLine();
                        break;
                    case "3":
                        Num22();
                        Console.ReadLine();
                        break;
                    case "4":
                        Num32();
                        Console.ReadLine();
                        break;
                    case "5":
                        Num42();
                        Console.ReadLine();
                        break;
                    case "6":
                        Num52();
                        Console.ReadLine();
                        break;
                    case "7":
                        Num62();
                        Console.ReadLine();
                        break;
                    case "8":
                        Num72();
                        Console.ReadLine();
                        break;
                    case "9":
                        Num82();
                        Console.ReadLine();
                        break;
                    case "0":
                        Num61();
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine("Такой задачи нет!");
                        Console.ReadLine();
                        break;
                }
                Console.Clear();
            }
            Console.ReadKey();

        }
    }
}
