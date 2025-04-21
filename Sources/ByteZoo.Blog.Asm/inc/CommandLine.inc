; -----------------------------------------------------------------------------------------------
; Command-Line Functions
; -----------------------------------------------------------------------------------------------

; Display command-line arguments
CommandLine_Print:      push                    rbp                                             ; void CommandLine_Print(int argument_count, char **argument_values)
                        mov                     rbp, rsp
                        sub                     rsp, 16
                        mov                     [rbp - 8], edi                                  ; Argument count
                        mov                     [rbp - 16], rsi                                 ; Arguments list
                        xor                     r8, r8
._loop:                 mov                     rsi, [rbp - 16]
                        mov                     rsi, [rsi + 8 * r8]
                        push                    r8
                        sub                     rsp, 8
                        Console_WriteLine_SZ
                        Console_WriteLine       new_line, 1
                        add                     rsp, 8
                        pop                     r8
                        inc                     r8
                        dec	                    dword [rbp - 8]
                        jnz                     ._loop
                        leave
                        ret