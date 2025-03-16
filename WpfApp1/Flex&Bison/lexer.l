%{
#include "parser.tab.h"
#include <stdio.h>  // Добавляем для printf
%}

%%

"function"      { printf("Найдено ключевое слово: function\n"); return FUNCTION; }
"return"        { printf("Найдено ключевое слово: return\n"); return RETURN; }
"("             { printf("Найдено: (\n"); return LPAREN; }
")"             { printf("Найдено: )\n"); return RPAREN; }
"{"             { printf("Найдено: {\n"); return LBRACE; }
"}"             { printf("Найдено: }\n"); return RBRACE; }
","             { printf("Найдено: ,\n"); return COMMA; }
";"             { printf("Найдено: ;\n"); return SEMICOLON; }
"+"             { printf("Найдено: +\n"); return PLUS; }
"*"             { printf("Найдено: *\n"); return MULTIPLY; }
[a-zA-Z][a-zA-Z0-9]* { printf("Найден идентификатор: %s\n", yytext); yylval.str = strdup(yytext); return IDENTIFIER; }
[0-9]+          { printf("Найдено число: %s\n", yytext); yylval.num = atoi(yytext); return NUMBER; }
[ \t\n]         ;  // Игнорируем пробелы, табуляции и переводы строк
.               { printf("Неизвестный символ: %s\n", yytext); }

%%