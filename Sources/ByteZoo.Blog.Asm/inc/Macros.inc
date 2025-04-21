; -----------------------------------------------------------------------------------------------
; Macros
; -----------------------------------------------------------------------------------------------

; Write string to console
%macro                  Console_WriteLine       0                                               ; void Console_WriteLine(const char *str = (RSI), size_t count = (RDX))
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        syscall
%endmacro

; Write string to console
%macro                  Console_WriteLine       2                                               ; void Console_WriteLine(const char *str = (%1), size_t count = (%2))
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        lea                     rsi, [%1]
                        mov                     rdx, %2
                        syscall
%endmacro

; Write null-terminated string to console
%macro                  Console_WriteLine_SZ    0                                               ; void Console_WriteLine_SZ(const char *str = (RSI))
                        mov                     rdi, rsi
                        push                    rsi
                        sub                     rsp, 8
                        call                    String_GetLength_SZ
                        add                     rsp, 8
                        pop                     rsi
                        mov                     rdx, rax
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        syscall
%endmacro

; Write null-terminated string to console
%macro                  Console_WriteLine_SZ    1                                               ; void Console_WriteLine_SZ(const char *str = (%1))
                        lea                     rdi, [%1]
                        call                    String_GetLength_SZ
                        mov                     rdx, rax
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        lea                     rsi, [%1]
                        syscall
%endmacro

; Exit application
%macro                  System_Exit             0                                               ; void System_Exit()
                        mov                     rax, SYS_EXIT
                        xor                     rdi, rdi
                        syscall
%endmacro

; Exit application
%macro                  System_Exit             1                                               ; void System_Exit(long error_code = (%1))
                        mov                     rax, SYS_EXIT
                        mov                     rdi, %1
                        syscall
%endmacro