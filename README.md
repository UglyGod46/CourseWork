# Персональный вариант:
Создание функции языка JavaScript
```plaintext
function calc(a, b, c) {
  return a + (b * c);
};
```


# Инструкция по работе с программой
1. Для создания нового документа нажмите 'Создать' в меню 'Файл'.
2. Для открытия существующего документа выберите 'Открыть'.
3. Используйте 'Сохранить' для сохранения изменений.
4. Для редактирования текста используйте функции 'Вырезать', 'Копировать', 'Вставить' и 'Удалить'.
5. Вы можете отменить или повторить действия с помощью 'Отменить' и 'Повторить'.
6. Для получения справки нажмите 'Справка' в меню.

Допольнительные функции:
1. Изменение размера шрифта
2. Базовая подсветка синтаксиса в окне редактирования (function, return, var, for, while и др.)
3. Наличие строки состояния для отображения текущей информации о состоянии работы приложения.
4. Горячие клавиши для быстрых команд (Undo, Redo, Copy, Cut, Paste, Select all)
5. Нумерация строк

# Постановка задачи на лабораторную работу
Тема: Разработка лексического анализатора (сканера).

Цель работы:
Изучить назначение лексического анализатора, спроектировать алгоритм и реализовать программный сканер для выделения лексем из текста программного кода.

# Задание
Проектирование диаграммы состояний сканера:
Разработать диаграмму состояний, которая описывает процесс распознавания лексем (токенов) в тексте. Диаграмма должна учитывать все типы лексем, указанные в варианте задания (например, ключевые слова, идентификаторы, числа, операторы, разделители и т.д.).
# Разработка лексического анализатора:
Реализовать лексический анализатор, который:
Читает входной текст (строку или многострочный текст программного кода).
Выделяет лексемы и классифицирует их по типам (например, "ключевое слово", "идентификатор", "число", "оператор", "разделитель", "недопустимый символ" и т.д.).
Игнорирует пробелы и символы табуляции, если они не являются частью лексемы.
Выводит ошибку при обнаружении недопустимых символов.
Интеграция сканера в интерфейс текстового редактора:
Встроить разработанный лексический анализатор в ранее созданный интерфейс текстового редактора.
Обеспечить возможность анализа многострочного текста.

# Формат выходных данных:
Результаты работы сканера должны быть представлены в виде таблицы (например, с использованием элемента управления DataGridView).
Каждая строка таблицы должна содержать:
Условный код лексемы (например, 14 для ключевого слова, 2 для идентификатора и т.д.).
Тип лексемы (например, "ключевое слово", "идентификатор", "число" и т.д.).
Лексему (например, int, x, 123 и т.д.).
Местоположение лексемы в тексте (например, "с 1 по 3 символ").

# Грамматика
1. Исходная грамматика
Исходная грамматика описывает синтаксис функции на языке JavaScript:
```plaintext
<Function> ::= "function" <FunctionName> "(" <Parameters> ")" "{" <FunctionBody> "}"
<FunctionName> ::= <Identifier>
<Parameters> ::= <Parameter> | <Parameter> "," <Parameters>
<Parameter> ::= <Identifier>
<FunctionBody> ::= <Statement> | <Statement> <FunctionBody>
<Statement> ::= "return" <Expression> ";"
<Expression> ::= <Term> | <Term> "+" <Expression> | <Term> "-" <Expression>
<Term> ::= <Factor> | <Factor> "*" <Term> | <Factor> "/" <Term>
<Factor> ::= <Identifier> | <Number> | "(" <Expression> ")"
<Identifier> ::= <Letter> | <Identifier> <Letter> | <Identifier> <Digit>
<Letter> ::= "a" | "b" | ... | "z" | "A" | "B" | ... | "Z"
<Digit> ::= "0" | "1" | ... | "9"
<Number> ::= <Digit> | <Number> <Digit>
```
2. Грамматика для Flex & Bison
Грамматика, переписанная для использования в Flex и Bison, выглядит следующим образом:
```plaintext
function: FUNCTION IDENTIFIER LPAREN parameters RPAREN LBRACE body RBRACE SEMICOLON
    {
        printf("Функция '%s' успешно распознана.\n", $2);
    }
    ;

parameters: IDENTIFIER
    | parameters COMMA IDENTIFIER
    ;

body: RETURN expression SEMICOLON
    ;

expression: term
    | term PLUS expression
    ;

term: factor
    | factor MULTIPLY term
    ;

factor: IDENTIFIER
    | NUMBER
    | LPAREN expression RPAREN
    ;
```
# Классификация грамматики
Грамматика, описанная выше, относится к контекстно-свободным грамматикам (КС-грамматикам). Это подтверждается следующими характеристиками:

Левая часть правил: Каждое правило имеет один нетерминальный символ в левой части.

Правая часть правил: Правая часть может содержать любую последовательность терминальных и нетерминальных символов.

Иерархия: Грамматика может быть классифицирована как грамматика типа 2 по классификации Хомского.

# Примеры допустимых строк:
```plaintext
1)
function calc(a, b, c) {
  return a + (b * c);
};

2)
function summ(a, b) {
  return a + b;
};

3)
function calc(a, b) {
  return a * b;
};
```

# Тестовые примеры:
Вход:
```plaintext
function calc(a, b, c) {
  return a + (b * c);
};
```
Выход:
```plaintext
| Условный код | Тип лексемы       | Лексема          | Местоположение          |
|--------------|-------------------|------------------|-------------------------|
| 0            | Keyword           | function         | с 1   по 9   символ |
| 1            | Identifier        | calc             | с 10  по 13  символ |
| 4            | Punctuation       | (                | с 14  по 14  символ |
| 1            | Identifier        | a                | с 15  по 15  символ |
| 4            | Punctuation       | ,                | с 16  по 16  символ |
| 1            | Identifier        | b                | с 18  по 18  символ |
| 4            | Punctuation       | ,                | с 19  по 19  символ |
| 1            | Identifier        | c                | с 21  по 21  символ |
| 4            | Punctuation       | )                | с 22  по 22  символ |
| 4            | Punctuation       | {                | с 24  по 24  символ |
| 0            | Keyword           | return           | с 28  по 34  символ |
| 1            | Identifier        | a                | с 35  по 35  символ |
| 3            | Operator          | +                | с 37  по 37  символ |
| 4            | Punctuation       | (                | с 39  по 39  символ |
| 1            | Identifier        | b                | с 40  по 40  символ |
| 3            | Operator          | *                | с 42  по 42  символ |
| 1            | Identifier        | c                | с 44  по 44  символ |
| 4            | Punctuation       | )                | с 45  по 45  символ |
| 5            | EndOfStatement    | ;                | с 46  по 46  символ |
| 4            | Punctuation       | }                | с 48  по 48  символ |
| 5            | EndOfStatement    | ;                | с 49  по 49  символ |

```
