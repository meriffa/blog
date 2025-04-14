; -----------------------------------------------------------------------------------------------
; ByteZoo.Blog.Asm Library
; -----------------------------------------------------------------------------------------------

                        %include                "Common.inc"

                        global                  _Function1
                        global                  _Function2
                        global                  _Function3
                        global                  _Function4

                        section                 .text

                        %include                "System.inc"
                        %include                "Macros.inc"

_Function1:             push                    rbp                                             ; p1 = RDI, p2 = RSI, p3 = RDX, p4 = RCX, p5 = R8, p6 = R9, p7 = [RBP + 16], p8 = [RBP + 24], p9 = [RBP + 32], p10 = [RBP + 40], Return = RAX
                        mov                     rbp, rsp
                        xor                     rax, rax
                        add                     al, dil
                        add                     al, sil
                        add                     al, dl
                        add                     ax, cx
                        add                     ax, r8w
                        add                     ax, r9w
                        add                     eax, [rbp + 16]
                        add                     eax, [rbp + 24]
                        add                     rax, [rbp + 32]
                        add                     rax, [rbp + 40]
                        pop                     rbp
                        ret

_Function2:             push                    rbp                                             ; p1 = XMM0, p2 = XMM1, p3 = XMM2, p4 = XMM3, p5 = XMM4, p6 = XMM5, p7 = XMM6, p8 = XMM7, p9 = [RBP + 16], p10 = [RBP + 24], Return = XMM0
                        mov                     rbp, rsp
                        vcvtss2sd               xmm0, xmm0, xmm0
                        vaddsd                  xmm0, xmm0, xmm1
                        vcvtss2sd               xmm2, xmm2, xmm2
                        vaddsd                  xmm0, xmm0, xmm2
                        vaddsd                  xmm0, xmm0, xmm3
                        vcvtss2sd               xmm4, xmm4, xmm4
                        vaddsd                  xmm0, xmm0, xmm4
                        vaddsd                  xmm0, xmm0, xmm5
                        vcvtss2sd               xmm6, xmm6, xmm6
                        vaddsd                  xmm0, xmm0, xmm6
                        vaddsd                  xmm0, xmm0, xmm7
                        vcvtss2sd               xmm8, xmm8, [rbp + 16]
                        vaddsd                  xmm0, xmm0, xmm8
                        vaddsd                  xmm0, xmm0, [rbp + 24]
                        pop                     rbp
                        ret

_Function3:             push                    rbp                                             ; p1 = RDI, p2 = XMM0, p3 = XMM1, p4 = RSI, Return = RDX:RAX
                        mov                     rbp, rsp
                        xor                     rcx, rcx
                        shr                     esi, 2
._loop:                 test                    esi, esi
                        jz                      ._complete
                        mov                     eax, dword [rdi]
                        add                     ecx, eax
                        dec                     esi
                        add                     rdi, 4
                        jmp                     ._loop
._complete:             mov                     rax, rcx
                        vcvtss2si               edx, xmm0
                        vcvtsd2si               ecx, xmm1
                        add                     edx, ecx
                        pop                     rbp
                        ret

_Function4:             push                    rbp                                             ; Hidden = [RDI], p1 = RSI, p2 = [RBP + 16], p3 = [RDX], p4 = [RCX], p5 = [R8], p6 = [R9], p7 = **[RBP + 24], p8 = [RBP + 32], Return Buffer = [RDI]
                        mov                     rbp, rsp
                        xor                     r10, r10
                        mov                     rax, rsi
                        shl                     rax, 32
                        shr                     rax, 32
                        add                     r10, rax
                        mov                     rax, rsi
                        shr                     rax, 32
                        add                     r10, rax
                        movzx                   rax, byte [rbp + 16]
                        add                     r10, rax
                        mov                     eax, dword [rbp + 16 + 1]
                        add                     r10, rax
                        mov                     eax, [rdx]
                        add                     r10, rax
                        mov                     eax, [rdx + 4]
                        add                     r10, rax
                        add                     r10, [r8]
                        add                     r10, [r8 + 8]
                        mov                     [rdi], r10
                        xor                     r10, r10
._loop1:                inc                     r10
                        cmp                     byte [r9], 0
                        je                      ._complete1
                        inc                     r9
                        jmp                     ._loop1
._complete1:            mov                     [rdi + 8], r10
                        push                    rcx
                        xor                     r10, r10
                        mov                     r9, [rbp + 24]
                        mov                     r9, [r9]
                        xor                     rax, rax
                        xor                     rcx, rcx
                        mov                     ecx, [rbp + 32]
._loop2:                test                    ecx, ecx
                        jz                      ._complete2
                        dec                     ecx
                        mov                     eax, dword [r9 + 4 * rcx]
                        add                     r10, rax
                        jmp                     ._loop2
._complete2:            mov                     [rdi + 16], r10
                        pop                     rcx
                        mov                     dword [rdx], 0x99999999
                        mov                     dword [rdx + 4], 0xAAAAAAAA
                        mov                     dword [rcx], 0x77777777
                        mov                     dword [rcx + 4], 0x88888888
                        pop                     rbp
                        ret

                        section                 .data