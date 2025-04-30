; -----------------------------------------------------------------------------------------------
; Fibonacci functions
; -----------------------------------------------------------------------------------------------

; Display Fibonacci sequence
Fibonacci_Display:      push                    rbp                                             ; void Fibonacci_Display(int element_count)
                        mov                     rbp, rsp
                        sub                     rsp, 16
                        mov                     dword [rbp - 4], 0                              ; Current number
                        mov                     dword [rbp - 8], 1                              ; Next number
                        mov                     [rbp - 12], edi                                 ; Sequence element count
._loop:                 mov                     edi, [rbp - 4]
                        lea                     rsi, [string_buffer]
                        call                    String_FromUInteger
                        mov                     rsi, rax
                        Console_WriteLine
                        Console_WriteLine       new_line, 1
                        mov                     eax, [rbp - 4]                                  ; Load current number
                        mov                     edx, [rbp - 8]                                  ; Load next number
                        mov                     [rbp - 4], edx                                  ; Save new current number
                        add                     [rbp - 8], eax                                  ; Save new next number
                        dec                     dword [rbp - 12]
                        jnz                     ._loop
                        leave
                        ret