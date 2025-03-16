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


# Грамматика
1. Исходная грамматика (разработанная мной)
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
