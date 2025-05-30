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

# Диаграмма состояния сканера:

![image](https://github.com/user-attachments/assets/8a8dac49-a441-450b-bc4b-6916ec2b058a)



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
Согласно классификации Хомского, грамматика G[<Function>] является контекстно-свободной (КС-грамматикой), так как все её продукции имеют форму A → α, где A ∈ VN (один нетерминал) и α ∈ V* (произвольная последовательность терминалов и/или нетерминалов). 

Эти правила демонстрируют типичные для КС-грамматик рекурсивные структуры с обработкой приоритета операций: 

<Expression> → <Term> | <Term>"+"<Expression> | <Term>"-"<Expression> 

<Term> → <Factor> | <Factor>"*"<Term> | <Factor>"/"<Term> 

Данное правило позволяет обрабатывать выражения произвольной глубины вложенности: 

<Factor> → "(" <Expression> ")" 

Данная грамматика относится к классу контекстно-свободных, так как все её продукции заменяют ровно один нетерминал без контекстных ограничений. Она поддерживает рекурсивные структуры и вложенные выражения, что характерно для КС-грамматик, но недопустимо в регулярных грамматиках. 

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

# Метод анализа 

Так как грамматика принадлежит классу контекстно-свободных анализ реализован методом рекурсивного спуска. 

Идея метода заключается в том, что каждому нетерминалу ставится в соответствие программная функция, которая распознает цепочку, порожденную этим нетерминалом. 

Эти функции вызываются в соответствии с правилами грамматики и иногда вызывают сами себя, поэтому для реализации необходимо выбрать язык, обладающий рекурсивными возможностями, в нашем случае это язык С#. 
![image](https://github.com/user-attachments/assets/40341747-2755-4601-ae3e-913668aab313)


# Тестовые примеры для парсера:
![image](https://github.com/user-attachments/assets/459546a7-eef3-4054-a217-60ab2224aedc)
![image](https://github.com/user-attachments/assets/d877dd54-fe62-4131-ac86-435a87a42be2)
![image](https://github.com/user-attachments/assets/e87b2c31-6928-41e0-9384-cf6bfa08f18c)


# 6-ая ЛР

Тестовый пример для каждого выражения:
Числа не заканчивающиеся на 0:
![image](https://github.com/user-attachments/assets/c63fd104-e9f4-42d7-8e04-ff870c373fe4)

Идентификаторы:

![image](https://github.com/user-attachments/assets/5b7bd916-1705-4107-b46e-07d6d7f001d5)

Номера для авто:

![image](https://github.com/user-attachments/assets/61d9946d-9aab-4007-96cc-044299ecb14d)



Граф автомата:
![image](https://github.com/user-attachments/assets/a16b2bc4-7a6f-4438-bee3-5ec1ab49dace)


# 8-ая ЛР

## Грамматика:
```
G[S]:
1. S -> <Noun phrase> <Verb phrase>
2. <Noun phrase> -> <Noun>| <Adjective phrase> <Noun> | λ
3. <Verb phrase> -> <Verb><Noun phrase>
4. <Adjective phrase> -> <Adjective phrase ><Adjective> | λ
<Noun> -> flight | passenger | trip | morning | ...
<Verb> -> is | prefers | like | need | depend | fly | ...
<Adjective> -> non-stop | first | direct | ...
```

## Язык:
L(G)={ (abj* noun verb)+ , abj ∈ Abjective, noun ∈ Noun, verb ∈ Verb}

## Классификация грамматики
Тип: Контекстно-свободная (КС, тип 2 по Хомскому).
Признаки:
- Левые части правил — одиночные нетерминалы.
- Рекурсия в AdjectivePhrase.

Ограничения:
- Не является автоматной (требует стека для разбора).
- Допускает неоднозначность (например, множественные прилагательные).

## Схема вызова функции:

![image](https://github.com/user-attachments/assets/f047ef1d-f545-463b-9ff3-c0e894bd8575)

## Тестовые примеры:

![image](https://github.com/user-attachments/assets/535c8995-b201-4943-8f30-b3ea7ed33abf)

![image](https://github.com/user-attachments/assets/a5feb141-cc59-4f63-a088-e15206d78750)

![image](https://github.com/user-attachments/assets/ac28f65f-d5e7-4123-87a4-8dbfb1ff327a)
