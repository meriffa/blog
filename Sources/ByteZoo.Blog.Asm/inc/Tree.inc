; -----------------------------------------------------------------------------------------------
; Tree Display
; -----------------------------------------------------------------------------------------------

_Tree_Display:          push                    rbp
                        mov                     rbp, rsp
                        mov                     rdx, rsi
                        mov                     r8, 1                                           ; Line number
._line_start:           xor                     r9, r9                                          ; Character count
._line_loop:            mov                     byte [rdx], '*'
                        inc                     rdx
                        inc                     r9
                        cmp                     r9, r8
                        jb                      ._line_loop
                        mov                     byte [rdx], `\n`
                        inc                     rdx
                        inc                     r8
                        cmp                     r8, rdi
                        jbe                     ._line_start
                        sub                     rdx, rsi
                        Console_WriteLine
                        pop                     rbp
                        ret