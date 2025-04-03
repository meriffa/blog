; -----------------------------------------------------------------------------------------------
; Macros
; -----------------------------------------------------------------------------------------------

; Write string to console [Console_WriteLine(const char *buf = RSI, size_t count = RDX)]
%macro                  Console_WriteLine       0
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        syscall
%endmacro

; Write string to console [Console_WriteLine(const char *buf, size_t count)]
%macro                  Console_WriteLine       2
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        lea                     rsi, [%1]
                        mov                     rdx, %2
                        syscall
%endmacro

; Write null-terminated string to console [Console_WriteLine_SZ(const char *str = RSI)]
%macro                  Console_WriteLine_SZ    0
                        mov                     rdi, rsi
                        push                    rsi
                        sub                     rsp, 8
                        call                    _String_GetLength
                        add                     rsp, 8
                        pop                     rsi
                        mov                     rdx, rax
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        syscall
%endmacro

; Write null-terminated string to console [Console_WriteLine_SZ(const char *str)]
%macro                  Console_WriteLine_SZ    1
                        lea                     rdi, [%1]
                        call                    _String_GetLength
                        mov                     rdx, rax
                        mov                     rax, SYS_WRITE
                        mov                     rdi, STDOUT
                        lea                     rsi, [%1]
                        syscall
%endmacro

; Exit application [System_Exit()]
%macro                  System_Exit             0
                        mov                     rax, SYS_EXIT
                        xor                     rdi, rdi
                        syscall
%endmacro

; Exit application [System_Exit(int error_code)]
%macro                  System_Exit             1
                        mov                     rax, SYS_EXIT
                        mov                     rdi, %1
                        syscall
%endmacro