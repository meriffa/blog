; -----------------------------------------------------------------------------------------------
; String Functions
; -----------------------------------------------------------------------------------------------

; Return string length [int _String_GetLength(const char *str)]
_String_GetLength:      push                    rbp
                        mov                     rbp, rsp
                        xor                     rax, rax
                        xor                     ecx, ecx
._loop:                 cmp                     [rdi], al
                        je                      ._found
                        inc                     ecx
                        inc                     rdi
                        jmp                     ._loop
._found:                mov                     eax, ecx
                        pop                     rbp
                        ret

; Convert string to lower case in-place [void _String_ToLowerInPlace(char *str)]
_String_ToLowerInPlace: push                    rbp
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
._skip:                 inc                     rdi
                        jmp                     ._loop
._complete:             pop                     rbp
                        ret

; Convert string to upper case in-place [void _String_ToUpperInPlace(char *str)]
_String_ToUpperInPlace: push                    rbp
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
._skip:                 inc                     rdi
                        jmp                     ._loop
._complete:             pop                     rbp
                        ret

; Convert unsigned integer to string [(char *str, size_t count) _String_FromUInteger(int value, char *str)]
_String_FromUInteger:   push                    rbp
                        mov                     rbp, rsp
                        add                     rsi, 12
                        xor                     r8, r8
                        mov                     eax, edi
                        mov                     ecx, 10
._loop:                 xor                     edx, edx
                        div                     ecx
                        add                     dl, '0'
                        mov                     [rsi], dl
                        dec                     rsi
                        inc                     r8
                        test                    eax, eax
                        jne                     ._loop
                        inc                     rsi
                        mov                     rax, rsi
                        mov                     rdx, r8
                        pop                     rbp
                        ret

; Convert signed integer to string [(char *str, size_t count) _String_FromSInteger(int value, char *str)]
_String_FromSInteger:   push                    rbp
                        mov                     rbp, rsp
                        add                     rsi, 12
                        xor                     r8, r8
                        xor                     r9, r9
                        mov                     eax, edi
                        mov                     ecx, 10
                        test                    eax, eax
                        jns                     ._loop
                        neg                     eax
                        inc                     r9
._loop:                 xor                     edx, edx
                        div                     ecx
                        add                     dl, '0'
                        mov                     [rsi], dl
                        dec                     rsi
                        inc                     r8
                        test                    eax, eax
                        jne                     ._loop
                        test                    r9, r9
                        jz                      ._skip_sign
                        mov                     byte [rsi], '-'
                        dec                     rsi
                        inc                     r8
._skip_sign:            inc                     rsi
                        mov                     rax, rsi
                        mov                     rdx, r8
                        pop                     rbp
                        ret