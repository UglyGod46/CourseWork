%{
#include <stdio.h>
#include <stdlib.h>

// Объявляем функции для обработки ошибок и вывода результата
void yyerror(const char *s);
int yylex();

// Объявляем переменные для хранения значений токенов
extern char *yytext;

// Объявляем переменную yyin, которая используется лексером для чтения файла
extern FILE *yyin;
%}

// Объявляем типы для токенов
%union {
    int num;
    char *str;
}

// Объявляем токены
%token FUNCTION RETURN LPAREN RPAREN LBRACE RBRACE COMMA SEMICOLON PLUS MULTIPLY
%token <num> NUMBER
%token <str> IDENTIFIER

// Объявляем правила грамматики
%%

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

%%

// Функция для обработки ошибок
void yyerror(const char *s) {
    fprintf(stderr, "Ошибка: %s\n", s);
}

int main(int argc, char **argv) {
    if (argc > 1) {
        FILE *file = fopen(argv[1], "r");
        if (!file) {
            perror("Ошибка открытия файла");
            return 1;
        }
        yyin = file;  // Указываем лексеру читать из файла
    }

    printf("Запуск парсера...\n");
    int result = yyparse();  // Запуск парсера
    printf("Парсер завершил работу с кодом: %d\n", result);

    return 0;
}