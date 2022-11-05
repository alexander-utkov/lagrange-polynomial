using System.Windows;

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
            object description = Application.Current.TryFindResource(key);
            Description = description == null ? null : description as string;

            Content = GetContent(key + ContentKeySuffix, args);
        }

        /// <summary>
        /// Описание действия в текстовом формате. Может быть null.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Содержание действия в формате LaTeX. Может быть null.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Представляет собой дополнение к ключу описания для формирования ключа содержания (шаблона).
        /// </summary>
        protected static string ContentKeySuffix = ".content";

        /// <summary>
        /// Получает содержание действия.
        /// </summary>
        /// <remarks>
        /// Содержание формируется в зависимости от наличия в ресурсах шаблона <paramref name="key"/>. Если ресурса нет,
        /// то содержание есть объединение <paramref name="args"/> через символ новой линии; иначе, содержанием является
        /// результат форматирования (<see cref="string.Format(string, object[])"/>) данного ресурса, причем аргументами
        /// форматирования являются <paramref name="args"/>.
        /// </remarks>
        /// <param name="key">Ключ содержания (шаблона). Может быть null.</param>
        /// <param name="args">Аргументы форматирования.</param>
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
                return string.Join("\\\\", args);
            }
            return null;
        }
    }
}
