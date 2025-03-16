#include <stdio.h>
#include "parser.tab.h"

// Объявляем yyparse как extern, так как она определена в parser.tab.c
extern int yyparse();

// Функция-обёртка для вызова yyparse
int parse_function() {
    return yyparse();
}