; -----------------------------------------------------------------------------------------------
;  System V Application Binary Interface (https://gitlab.com/x86-psABIs/x86-64-ABI)
; -----------------------------------------------------------------------------------------------
;
; - Parameters:
;                       Integral Parameter 1    RDI
;                       Integral Parameter 2    RSI
;                       Integral Parameter 3    RDX
;                       Integral Parameter 4    RCX
;                       Integral Parameter 5    R8
;                       Integral Parameter 6    R9
;                       Integral Parameter N    [RBP + 8 * (N - 7) + 16]
;                       Float Parameter 1       XMM0
;                       Float Parameter 2       XMM1
;                       Float Parameter 3       XMM2
;                       Float Parameter 4       XMM3
;                       Float Parameter 5       XMM4
;                       Float Parameter 6       XMM5
;                       Float Parameter 7       XMM6
;                       Float Parameter 8       XMM7
;                       Float Parameter N       [RBP + 8 * (N - 9) + 16]
;
; - Return Values:
;                       Integral Return Value   RAX                                             ; 8 bit (byte), 16 bit (short), 32 bit (int), 64 bit (long)
;                       Integral Return Value   RDX:RAX                                         ; 128 bit
;                       Float Return Value      XMM0                                            ; 32 bit (single), 64 bit (double), 128 bit
;                       Float Return Value      XMM1:XMM0                                       ; 256 bit
;
; - Temporary Registers:
;                       Integral                RAX, RCX, RDX, RSI, RDI, R8 ... R11
;                       Float                   XMM0 ... XMM15, MM0 ... MM7, K0 ... K7, ST0 ... ST7
;
; - Preserve Registers:
;                       Integral                RBX, RSP, RBP, R12 ... R15, RFLAGS.DF, MXCSR, CR0
;
; - Stack Frame:
;                       Parameter 9             [RBP + 32]
;                       Parameter 8             [RBP + 24]
;                       Parameter 7             [RBP + 16]
;                       Return Address          [RBP + 8]
; RBP, RSP (Leaf) ->    Original RBP            [RBP]
;                       Local Variable 1        [RBP - Local_Variable_1_Size]
;                       Local Variable 2        [RBP - Local_Variable_1_Offset - Local_Variable_2_Size]
;                       Local Variable 3        [RBP - Local_Variable_2_Offset - Local_Variable_3_Size]
;                       Padding                 [RBP - Local_Variable_3_Offset - Padding_Size]
;                       Parameter 1 Copy        [RBP - Padding_Offset - Parameter_1_Copy_Size]
;                       Parameter 2 Copy        [RBP - Parameter_1_Copy_Offset - Parameter_2_Copy_Size]
;                       Parameter 3 Copy        [RBP - Parameter_2_Copy_Offset - Parameter_3_Copy_Size]
;                       Parameter 4 Copy        [RBP - Parameter_3_Copy_Offset - Parameter_4_Copy_Size]
;                       Parameter 5 Copy        [RBP - Parameter_4_Copy_Offset - Parameter_5_Copy_Size]
; RSP (Non-Leaf) ->     Parameter 6 Copy        [RBP - Parameter_5_Copy_Offset - Parameter_6_Copy_Size]
;                       Red Zone Start          [RSP]
;                       Red Zone End            [RSP - 128]
;
;_Call_Site:            push                    9                                               ; Parameter 9
;                       push                    8                                               ; Parameter 8
;                       push                    7                                               ; Parameter 7
;                       mov                     r9d, 6                                          ; Parameter 6
;                       mov                     r8d, 5                                          ; Parameter 5
;                       mov                     rcx, 4                                          ; Parameter 4
;                       mov                     rdx, 3                                          ; Parameter 3
;                       mov                     rsi, 2                                          ; Parameter 2
;                       mov                     rdi, 1                                          ; Parameter 1
;                       sub                     rsp, 8                                          ; Stack Alignment (16-byte)
;                       call                    _Call_Target
;                       add                     rsp, 32                                         ; 8 * Stack_Parameter_Count + Stack Alignment (3 * 8 + 8 = 32)
;
;_Call_Target_Non_Leaf: push                    rbp
;                       mov                     rbp, rsp
;                       sub                     rsp, 40
;                       mov                     dword [rbp - 20], edi                           ; Parameter 1
;                       mov                     dword [rbp - 24], esi                           ; Parameter 2
;                       mov                     dword [rbp - 28], edx                           ; Parameter 3
;                       mov                     dword [rbp - 32], ecx                           ; Parameter 4
;                       mov                     dword [rbp - 36], r8d                           ; Parameter 5
;                       mov                     dword [rbp - 40], r9d                           ; Parameter 6
;                       mov                     eax, dword [rbp + 16]                           ; Parameter 7
;                       mov                     eax, dword [rbp + 24]                           ; Parameter 8
;                       mov                     eax, dword [rbp + 32]                           ; Parameter 9
;                       mov                     dword [rbp - 4], 0                              ; Local 1
;                       leave                                                                   ; mov rsp, rbp & pop rbp
;                       ret
;
;_Call_Target_Leaf:     push                    rbp
;                       mov                     rbp, rsp
;                       mov                     dword [rbp - 20], edi                           ; Parameter 1
;                       mov                     dword [rbp - 24], esi                           ; Parameter 2
;                       mov                     dword [rbp - 28], edx                           ; Parameter 3
;                       mov                     dword [rbp - 32], ecx                           ; Parameter 4
;                       mov                     dword [rbp - 36], r8d                           ; Parameter 5
;                       mov                     dword [rbp - 40], r9d                           ; Parameter 6
;                       mov                     eax, dword [rbp + 16]                           ; Parameter 7
;                       mov                     eax, dword [rbp + 24]                           ; Parameter 8
;                       mov                     eax, dword [rbp + 32]                           ; Parameter 9
;                       mov                     dword [rbp - 4], 0                              ; Local 1
;                       pop                     rbp
;                       ret
;
; -----------------------------------------------------------------------------------------------