; -----------------------------------------------------------------------------------------------
; ByteZoo.Blog.Asm Library
; -----------------------------------------------------------------------------------------------

                        %include                "Common.inc"

                        global                  Assembly_Function1
                        global                  Assembly_Function2
                        global                  Assembly_Function3
                        global                  Assembly_Function4

                        section                 .text

                        %include                "System.inc"
                        %include                "Macros.inc"

; Return sum of all parameters (integral)
Assembly_Function1:     push                    rbp                                             ; long Assembly_Function1(bool p1, byte p2, sbyte p3, char p4, short p5, ushort p6, int p7, uint p8, long p9, ulong p10)
                        mov                     rbp, rsp
                        xor                     rax, rax
                        add                     al, dil                                         ; p1
                        add                     al, sil                                         ; p2
                        add                     al, dl                                          ; p3
                        add                     ax, cx                                          ; p4
                        add                     ax, r8w                                         ; p5
                        add                     ax, r9w                                         ; p6
                        add                     eax, [rbp + 16]                                 ; p7
                        add                     eax, [rbp + 24]                                 ; p8
                        add                     rax, [rbp + 32]                                 ; p9
                        add                     rax, [rbp + 40]                                 ; p10
                        pop                     rbp
                        ret

; Return sum of all parameters (floating point)
Assembly_Function2:     push                    rbp                                             ; double Assembly_Function2(float p1, double p2, float p3, double p4, float p5, double p6, float p7, double p8, float p9, double p10)
                        mov                     rbp, rsp
                        vcvtss2sd               xmm0, xmm0, xmm0                                ; p1
                        vaddsd                  xmm0, xmm0, xmm1                                ; p2
                        vcvtss2sd               xmm2, xmm2, xmm2                                ; p3
                        vaddsd                  xmm0, xmm0, xmm2
                        vaddsd                  xmm0, xmm0, xmm3                                ; p4
                        vcvtss2sd               xmm4, xmm4, xmm4                                ; p5
                        vaddsd                  xmm0, xmm0, xmm4
                        vaddsd                  xmm0, xmm0, xmm5                                ; p6
                        vcvtss2sd               xmm6, xmm6, xmm6                                ; p7
                        vaddsd                  xmm0, xmm0, xmm6
                        vaddsd                  xmm0, xmm0, xmm7                                ; p8
                        vcvtss2sd               xmm8, xmm8, [rbp + 16]                          ; p9
                        vaddsd                  xmm0, xmm0, xmm8
                        vaddsd                  xmm0, xmm0, [rbp + 24]                          ; p10
                        pop                     rbp
                        ret

; Return structure result (Field1 = Sum(int *p1, Count(p4)), Field2 = p2 + p3)
Assembly_Function3:     push                    rbp                                             ; Structure3 AssemblyFunction3(IntPtr p1, float p2, double p3, UIntPtr p4)
                        mov                     rbp, rsp
                        xor                     rcx, rcx
                        shr                     esi, 2                                          ; p4
._loop:                 test                    esi, esi
                        jz                      ._complete
                        mov                     eax, dword [rdi]                                ; *p1
                        add                     ecx, eax
                        dec                     esi
                        add                     rdi, 4
                        jmp                     ._loop
._complete:             mov                     rax, rcx                                        ; Structure3.Field1
                        vcvtss2si               edx, xmm0                                       ; p2
                        vcvtsd2si               ecx, xmm1                                       ; p3
                        add                     edx, ecx                                        ; Structure3.Field2
                        pop                     rbp
                        ret

; Return structure result (Field1 = p1.Field1 + p1.Field2 + p2.Field1 + p2.Field2 + p3.Field1 + p3.Field2 + p5._lo64 + p5._hi32, Field2 = Length(p6), Field3 = Sum(int **p7, Count(p8)))
Assembly_Function4:     push                    rbp                                             ; Structure4 Assembly_Function4(Structure1 p1, Structure2 p2, ref Structure1 p3, out Structure1 p4, ref decimal p5, string p6, in int[] p7, int p8)
                        mov                     rbp, rsp
                        xor                     r10, r10
                        mov                     rax, rsi                                        ; p1
                        shl                     rax, 32
                        shr                     rax, 32                                         ; p1.Field1
                        add                     r10, rax
                        mov                     rax, rsi
                        shr                     rax, 32                                         ; p1.Field2
                        add                     r10, rax
                        movzx                   rax, byte [rbp + 16]                            ; p2.Field1
                        add                     r10, rax
                        mov                     eax, dword [rbp + 16 + 1]                       ; p2.Field2
                        add                     r10, rax
                        mov                     eax, [rdx]                                      ; *p3.Field1
                        add                     r10, rax
                        mov                     eax, [rdx + 4]                                  ; *p3.Field2
                        add                     r10, rax
                        add                     r10, [r8]                                       ; *p5._lo64
                        add                     r10, [r8 + 8]                                   ; *p5._hi32
                        mov                     [rdi], r10                                      ; Structure4.Field1
                        xor                     r10, r10
._loop1:                inc                     r10
                        cmp                     byte [r9], 0                                    ; *p6
                        je                      ._complete1
                        inc                     r9
                        jmp                     ._loop1
._complete1:            mov                     [rdi + 8], r10                                  ; Structure4.Field2
                        push                    rcx                                             ; p4
                        xor                     r10, r10
                        mov                     r9, [rbp + 24]                                  ; *p7
                        mov                     r9, [r9]
                        xor                     rax, rax
                        xor                     rcx, rcx
                        mov                     ecx, [rbp + 32]                                 ; p8
._loop2:                test                    ecx, ecx
                        jz                      ._complete2
                        dec                     ecx
                        mov                     eax, dword [r9 + 4 * rcx]
                        add                     r10, rax
                        jmp                     ._loop2
._complete2:            mov                     [rdi + 16], r10                                 ; Structure4.Field3
                        pop                     rcx
                        mov                     dword [rdx], 0x99999999                         ; p3.Field1
                        mov                     dword [rdx + 4], 0xAAAAAAAA                     ; p3.Field2
                        mov                     dword [rcx], 0x77777777                         ; p4.Field1
                        mov                     dword [rcx + 4], 0x88888888                     ; p4.Field1
                        pop                     rbp
                        ret

                        section                 .data