; -----------------------------------------------------------------------------------------------
; String Functions
; -----------------------------------------------------------------------------------------------

; Return null-terminated string length
String_GetLength_SZ:    push                    rbp                                             ; int String_GetLength_SZ(const char *str)
                        mov                     rbp, rsp
                        xor                     rax, rax
                        xor                     ecx, ecx
._loop:                 cmp                     al, [rdi]
                        je                      ._found
                        add                     ecx, 1
                        add                     rdi, 1
                        jmp                     ._loop
._found:                mov                     eax, ecx
                        pop                     rbp
                        ret

; Convert null-terminated string to lower case (in-place)
String_ToLower_SZ:      push                    rbp                                             ; void String_ToLower_SZ(char *str)
                        mov                     rbp, rsp
._loop:                 mov                     al, [rdi]
                        test                    al, al
                        je                      ._complete
                        cmp                     al, 'A'
                        jb                      ._skip
                        cmp                     al, 'Z'
                        ja                      ._skip
                        add                     al, 32
                        mov                     [rdi], al
._skip:                 add                     rdi, 1
                        jmp                     ._loop
._complete:             pop                     rbp
                        ret

; Convert null-terminated string to upper case (in-place)
String_ToUpper_SZ:      push                    rbp                                             ; void String_ToUpper_SZ(char *str)
                        mov                     rbp, rsp
._loop:                 mov                     al, [rdi]
                        test                    al, al
                        je                      ._complete
                        cmp                     al, 'a'
                        jb                      ._skip
                        cmp                     al, 'z'
                        ja                      ._skip
                        sub                     al, 32
                        mov                     [rdi], al
._skip:                 add                     rdi, 1
                        jmp                     ._loop
._complete:             pop                     rbp
                        ret

; Convert unsigned integer to string
String_FromUInteger:    push                    rbp                                             ; (char *buffer, size_t count) String_FromUInteger(unsigned int value, char *buffer)
                        mov                     rbp, rsp
                        add                     rsi, 12
                        xor                     r8, r8
                        mov                     eax, edi
                        mov                     ecx, 10
._loop:                 xor                     edx, edx
                        div                     ecx
                        add                     dl, '0'
                        mov                     [rsi], dl
                        sub                     rsi, 1
                        add                     r8, 1
                        test                    eax, eax
                        jne                     ._loop
                        add                     rsi, 1
                        mov                     rax, rsi
                        mov                     rdx, r8
                        pop                     rbp
                        ret

; Convert integer to string
String_FromInteger:     push                    rbp                                             ; (char *buffer, size_t count) String_FromInteger(int value, char *buffer)
                        mov                     rbp, rsp
                        add                     rsi, 12
                        xor                     r8, r8
                        xor                     r9, r9
                        mov                     eax, edi
                        mov                     ecx, 10
                        test                    eax, eax
                        jns                     ._loop
                        neg                     eax
                        add                     r9, 1
._loop:                 xor                     edx, edx
                        div                     ecx
                        add                     dl, '0'
                        mov                     [rsi], dl
                        sub                     rsi, 1
                        add                     r8, 1
                        test                    eax, eax
                        jne                     ._loop
                        test                    r9, r9
                        jz                      ._skip_sign
                        mov                     byte [rsi], '-'
                        sub                     rsi, 1
                        add                     r8, 1
._skip_sign:            add                     rsi, 1
                        mov                     rax, rsi
                        mov                     rdx, r8
                        pop                     rbp
                        ret