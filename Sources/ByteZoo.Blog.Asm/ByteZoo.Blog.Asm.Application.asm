; -----------------------------------------------------------------------------------------------
; ByteZoo.Blog.Asm Application
; -----------------------------------------------------------------------------------------------

                        %include                "Common.inc"

                        global                  _start

                        section                 .text

                        %include                "System.inc"
                        %include                "Macros.inc"
                        %include                "String.inc"
                        %include                "CommandLine.inc"
                        %include                "./ByteZoo.Blog.Asm.Application/Fibonacci.asm"
                        %include                "./ByteZoo.Blog.Asm.Application/Tree.asm"

_start:                 Console_WriteLine       title_separator, title_separator_length         ; Display application title
                        Console_WriteLine_SZ    application_title
                        Console_WriteLine       title_separator, title_separator_length
                        Console_WriteLine       new_line, 1
                        mov                     edi, [rsp]                                      ; Display command-line arguments
                        lea                     rsi, [rsp + 8]
                        call                    CommandLine_Print
                        Console_WriteLine       new_line, 1
                        mov                     edi, 11                                         ; Display Fibonacci sequence
                        call                    Fibonacci_Display
                        Console_WriteLine       new_line, 1
                        mov                     edi, 5                                          ; Display tree
                        lea                     rsi, [tree_display_buffer]
                        call                    Tree_Display
                        System_Exit                                                             ; Exit application

                        section                 .data

title_separator:        db                      "********************************", `\n`
title_separator_length: equ                     $ - title_separator
application_title:      db                      "* ByteZoo.Blog.Asm Application *", `\n`, 0
new_line:               db                      `\n`

                        section                 .bss

string_buffer:          resb                    256
tree_display_buffer:    resb                    1024