; -----------------------------------------------------------------------------------------------
; Tree functions
; -----------------------------------------------------------------------------------------------

; Display tree
Tree_Display:           push                    rbp                                             ; void Tree_Display(int line_count, char *buffer)
                        mov                     rbp, rsp
                        mov                     rdx, rsi
                        mov                     r8d, 1                                          ; Line number
._line_start:           xor                     r9, r9                                          ; Character count
._line_loop:            mov                     byte [rdx], '*'
                        inc                     rdx
                        inc                     r9d
                        cmp                     r9d, r8d
                        jb                      ._line_loop
                        mov                     byte [rdx], `\n`
                        inc                     rdx
                        inc                     r8d
                        cmp                     r8d, edi
                        jbe                     ._line_start
                        sub                     rdx, rsi
                        Console_WriteLine
                        pop                     rbp
                        ret