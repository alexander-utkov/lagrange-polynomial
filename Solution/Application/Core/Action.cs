using Antlr4.Runtime.Misc;
using System.Windows;

// TODO: Рефакторинг.

namespace NumericalMethods.Core
{
    /// <summary>
    /// Представляет математическое действие - единицу решения задачи.
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Создает представление действия из ресурсов приложения.
        /// </summary>
        /// <param name="key">Основа ключей ресурсов.</param>
        /// <param name="args">Аргументы форматирования для <see cref="GetContent(string, object[])"/>.</param>
        public Action(string key, params object[] args)
        {
            m_key = key;
            m_args = args;
        }

        /// <summary>
        /// Описание действия в текстовом формате. Может быть null.
        /// </summary>
        public string Description => Application.Current.TryFindResource(m_key) as string;

        /// <summary>
        /// Содержание действия в формате LaTeX. Может быть null.
        /// </summary>
        public string Content => GetContent(m_key + m_content_key_suffix, m_args);

        /// <summary>
        /// Представляет собой дополнение к ключу описания для формирования ключа содержания (шаблона).
        /// </summary>
        protected static string m_content_key_suffix = ".content";

        protected string m_key;
        protected object[] m_args;

        /// <summary>
        /// Получает содержание действия.
        /// </summary>
        /// <remarks>
        /// Содержание формируется в зависимости от наличия в ресурсах шаблона <paramref name="key"/>. Если ресурса нет,
        /// то содержание есть объединение <paramref name="args"/> через символ новой линии; иначе, содержанием является
        /// результат форматирования (<see cref="string.Format(string, object[])"/>) данного ресурса, причем аргументами
        /// форматирования являются <paramref name="args"/>, однако, если первый элемент <see cref="args"/> является
        /// вложенным <see cref="Action"/>, то все элементы <see cref="Args>"/> считаются такового типа; в таком случае
        /// содержанием будет объединение свойств <see cref="Content"/> данных аргументов через символ новой линии.
        /// </remarks>
        /// <param name="key">Ключ содержания (шаблона). Может быть null.</param>
        /// <param name="args">Аргументы форматирования или вложенные <see cref="Action"/>.</param>
        /// <returns>
        /// Содержание действия в формате LaTeX. Может быть null, если ни определен ресурс <paramref name="key"/>, ни
        /// определены аргументы (строки) <paramref name="args"/>.
        /// </returns>
        public static string GetContent(string key, params object[] args)
        {
            object content = Application.Current.TryFindResource(key);
            if (content != null)
            {
                if (args.Length == 0)
                {
                    return content as string;
                }
                else
                {
                    var template = "";
                    var template_raw = content as string;

                    // Экранируем скобки LaTeX макросов
                    template_raw = template_raw.Replace("{", "{{");
                    template_raw = template_raw.Replace("}", "}}");

                    // Заменяем специальный символ на метку вставки
                    int num = 0;
                    var parts = template_raw.Split('@');
                    for (int index = 0; index < parts.Length; index++)
                    {
                        if (index != 0)
                        {
                            template += $"{{{num}}}";
                            num++;
                        }
                        template += parts[index];
                    }

                    return string.Format(template, args);
                }
            }
            else if (args.Length != 0)
            {
                if (args[0].GetType() == typeof(Action))
                {
                    string result = "";
                    for (int index = 0; index < args.Length; index++)
                    {
                        if (index != 0)
                        {
                            result += "\\\\";
                        }
                        result += (args[index] as Action).Content;
                    }
                    return result;
                }
                return string.Join("\\\\", args);
            }
            return null;
        }
    }
}
