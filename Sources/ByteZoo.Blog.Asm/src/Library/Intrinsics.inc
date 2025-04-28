; -----------------------------------------------------------------------------------------------
; Intrinsics functions
; -----------------------------------------------------------------------------------------------

; Globals
                        global                  MinAvx2_Interop
                        global                  MinAvx512_Interop
                        global                  MaxAvx2_Interop
                        global                  MaxAvx512_Interop
                        global                  SumAvx2_Interop
                        global                  SumAvx512_Interop
                        global                  CompareAvx2_Interop
                        global                  CompareAvx512_Interop
                        global                  CountAvx2_Interop
                        global                  CountAvx512_Interop

; Constants
ELEMENT_SIZE            equ                     4                                               ; sizeof(int)
COMPARE_ELEMENT_SIZE    equ                     1                                               ; sizeof(byte)
YMM_SIZE                equ                     32                                              ; YMM register size (256-bit)
ZMM_SIZE                equ                     64                                              ; ZMM register size (512-bit)

; Return memory region minimum (AVX2)
MinAvx2_Interop:        push                    rbp                                             ; int MinAvx2Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     rax, rax
                        test                    esi, esi
                        jz                      ._complete
                        mov                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        cmp                     esi, YMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_simd
                        vmovdqu                 ymm0, [rdi]
                        add                     rdi, YMM_SIZE
                        sub                     esi, YMM_SIZE / ELEMENT_SIZE
._simd_loop:            cmp                     esi, YMM_SIZE / ELEMENT_SIZE
                        jl                      ._simd_extract
                        vpminsd                 ymm0, ymm0, [rdi]
                        add                     rdi, YMM_SIZE
                        sub                     esi, YMM_SIZE / ELEMENT_SIZE
                        jmp                     ._simd_loop
._simd_extract:         vmovdqu                 [rbp - YMM_SIZE], ymm0
                        mov                     rcx, YMM_SIZE / ELEMENT_SIZE - 1
._simd_extract_loop:    cmp                     eax, [rbp - YMM_SIZE + rcx * ELEMENT_SIZE]
                        jle                     ._simd_extract_skip
                        mov                     eax, [rbp - YMM_SIZE + rcx * ELEMENT_SIZE]
._simd_extract_skip:    dec                     rcx
                        jns                     ._simd_extract_loop
._post_simd:            test                    esi, esi
                        jz                      ._complete
._post_simd_loop:       cmp                     eax, [rdi]
                        jle                     ._post_simd_skip
                        mov                     eax, [rdi]
._post_simd_skip:       add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jnz                     ._post_simd_loop
._complete:             pop                     rbp
                        ret

; Return memory region minimum (AVX-512)
MinAvx512_Interop:      push                    rbp                                             ; int MinAvx512Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     rax, rax
                        test                    esi, esi
                        jz                      ._complete
                        mov                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        cmp                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_simd
                        vmovdqu64               zmm0, [rdi]
                        add                     rdi, ZMM_SIZE
                        sub                     esi, ZMM_SIZE / ELEMENT_SIZE
._simd_loop:            cmp                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jl                      ._simd_extract
                        vpminsd                 zmm0, zmm0, [rdi]
                        add                     rdi, ZMM_SIZE
                        sub                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jmp                     ._simd_loop
._simd_extract:         vmovdqu64               [rbp - ZMM_SIZE], zmm0
                        mov                     rcx, ZMM_SIZE / ELEMENT_SIZE - 1
._simd_extract_loop:    cmp                     eax, [rbp - ZMM_SIZE + rcx * ELEMENT_SIZE]
                        jle                     ._simd_extract_skip
                        mov                     eax, [rbp - ZMM_SIZE + rcx * ELEMENT_SIZE]
._simd_extract_skip:    dec                     rcx
                        jns                     ._simd_extract_loop
._post_simd:            test                    esi, esi
                        jz                      ._complete
._post_simd_loop:       cmp                     eax, [rdi]
                        jle                     ._post_simd_skip
                        mov                     eax, [rdi]
._post_simd_skip:       add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jnz                     ._post_simd_loop
._complete:             pop                     rbp
                        ret

; Return memory region maximum (AVX2)
MaxAvx2_Interop:        push                    rbp                                             ; int MaxAvx2Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     rax, rax
                        test                    esi, esi
                        jz                      ._complete
                        mov                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        cmp                     esi, YMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_simd
                        vmovdqu                 ymm0, [rdi]
                        add                     rdi, YMM_SIZE
                        sub                     esi, YMM_SIZE / ELEMENT_SIZE
._simd_loop:            cmp                     esi, YMM_SIZE / ELEMENT_SIZE
                        jl                      ._simd_extract
                        vpmaxsd                 ymm0, ymm0, [rdi]
                        add                     rdi, YMM_SIZE
                        sub                     esi, YMM_SIZE / ELEMENT_SIZE
                        jmp                     ._simd_loop
._simd_extract:         vmovdqu                 [rbp - YMM_SIZE], ymm0
                        mov                     rcx, YMM_SIZE / ELEMENT_SIZE - 1
._simd_extract_loop:    cmp                     eax, [rbp - YMM_SIZE + rcx * ELEMENT_SIZE]
                        jge                     ._simd_extract_skip
                        mov                     eax, [rbp - YMM_SIZE + rcx * ELEMENT_SIZE]
._simd_extract_skip:    dec                     rcx
                        jns                     ._simd_extract_loop
._post_simd:            test                    esi, esi
                        jz                      ._complete
._post_simd_loop:       cmp                     eax, [rdi]
                        jge                     ._post_simd_skip
                        mov                     eax, [rdi]
._post_simd_skip:       add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jnz                     ._post_simd_loop
._complete:             pop                     rbp
                        ret

; Return memory region maximum (AVX-512)
MaxAvx512_Interop:      push                    rbp                                             ; int MaxAvx512Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     rax, rax
                        test                    esi, esi
                        jz                      ._complete
                        mov                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        cmp                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_simd
                        vmovdqu64               zmm0, [rdi]
                        add                     rdi, ZMM_SIZE
                        sub                     esi, ZMM_SIZE / ELEMENT_SIZE
._simd_loop:            cmp                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jl                      ._simd_extract
                        vpmaxsd                 zmm0, zmm0, [rdi]
                        add                     rdi, ZMM_SIZE
                        sub                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jmp                     ._simd_loop
._simd_extract:         vmovdqu64               [rbp - ZMM_SIZE], zmm0
                        mov                     rcx, ZMM_SIZE / ELEMENT_SIZE - 1
._simd_extract_loop:    cmp                     eax, [rbp - ZMM_SIZE + rcx * ELEMENT_SIZE]
                        jge                     ._simd_extract_skip
                        mov                     eax, [rbp - ZMM_SIZE + rcx * ELEMENT_SIZE]
._simd_extract_skip:    dec                     rcx
                        jns                     ._simd_extract_loop
._post_simd:            test                    esi, esi
                        jz                      ._complete
._post_simd_loop:       cmp                     eax, [rdi]
                        jge                     ._post_simd_skip
                        mov                     eax, [rdi]
._post_simd_skip:       add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jnz                     ._post_simd_loop
._complete:             pop                     rbp
                        ret

; Return memory region sum (AVX2)
SumAvx2_Interop:        push                    rbp                                             ; int SumAvx2Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     rax, rax
._pre_aligned:          test                    esi, esi
                        jz                      ._complete
                        test                    edi, 0x3F                                       ; 64-byte alignment
                        jz                      ._aligned
                        add                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jmp                     ._pre_aligned
._aligned:              vxorps                  xmm0, xmm0, xmm0
._aligned_loop:         cmp                     esi, YMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_aligned
                        vmovdqa                 ymm1, [rdi]
                        vpaddd                  ymm0, ymm0, ymm1
                        add                     rdi, YMM_SIZE
                        sub                     esi, YMM_SIZE / ELEMENT_SIZE
                        jmp                     ._aligned_loop
._post_aligned:         vphaddd                 ymm0, ymm0, ymm0
                        vmovdqu                 [rbp - YMM_SIZE], ymm0
                        add                     eax, [rbp - YMM_SIZE + 0 * ELEMENT_SIZE]
                        add                     eax, [rbp - YMM_SIZE + 1 * ELEMENT_SIZE]
                        add                     eax, [rbp - YMM_SIZE + 4 * ELEMENT_SIZE]
                        add                     eax, [rbp - YMM_SIZE + 5 * ELEMENT_SIZE]
._post_aligned_loop:    test                    esi, esi
                        jz                      ._complete
                        add                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jmp                     ._post_aligned_loop
._complete:             pop                     rbp
                        ret

; Return memory region sum (AVX-512)
SumAvx512_Interop:      push                    rbp                                             ; int SumAvx512Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     rax, rax
._pre_aligned:          test                    esi, esi
                        jz                      ._complete
                        test                    edi, 0x3F                                       ; 64-byte alignment
                        jz                      ._aligned
                        add                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jmp                     ._pre_aligned
._aligned:              vxorps                  ymm0, ymm0, ymm0
._aligned_loop:         cmp                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_aligned
                        vmovdqa64               zmm1, [rdi]
                        vpaddd                  zmm0, zmm0, zmm1
                        add                     rdi, ZMM_SIZE
                        sub                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jmp                     ._aligned_loop
._post_aligned:         vmovdqu64               [rbp - ZMM_SIZE], zmm0
                        mov                     rcx, ZMM_SIZE / ELEMENT_SIZE - 1
._post_aligned_loop1:   add                     eax, [rbp - ZMM_SIZE + rcx * ELEMENT_SIZE]
                        dec                     rcx
                        jns                     ._post_aligned_loop1
._post_aligned_loop2:   test                    esi, esi
                        jz                      ._complete
                        add                     eax, [rdi]
                        add                     rdi, ELEMENT_SIZE
                        dec                     esi
                        jmp                     ._post_aligned_loop2
._complete:             pop                     rbp
                        ret

; Compare memory regions (AVX2)
CompareAvx2_Interop:    push                    rbp                                             ; int CompareAvx2Interop(byte* pRegion1, byte* pRegion2, int itemCount);
                        mov                     rbp, rsp
                        mov                     eax, 1
                        test                    edx, edx
                        jz                      ._complete
._simd_loop:            cmp                     edx, YMM_SIZE / COMPARE_ELEMENT_SIZE
                        jl                      ._post_simd
                        vmovdqu                 ymm0, [rdi]
                        vpcmpeqb                ymm0, ymm0, [rsi]
                        vpmovmskb               ecx, ymm0
                        cmp                     ecx, 0xFFFFFFFF
                        jne                     ._no_match
                        add                     rdi, YMM_SIZE
                        add                     rsi, YMM_SIZE
                        sub                     edx, YMM_SIZE / COMPARE_ELEMENT_SIZE
                        jmp                     ._simd_loop
._post_simd:            test                    edx, edx
                        jz                      ._complete
                        mov                     cl, [rdi]
                        cmp                     cl, [rsi]
                        jne                     ._no_match
                        add                     rdi, COMPARE_ELEMENT_SIZE
                        add                     rsi, COMPARE_ELEMENT_SIZE
                        sub                     edx, 1
                        jmp                     ._post_simd
._no_match:             xor                     rax, rax
._complete:             pop                     rbp
                        ret

; Compare memory regions (AVX-512)
CompareAvx512_Interop:  push                    rbp                                             ; bool CompareAvx512Interop(byte* pRegion1, byte* pRegion2, int itemCount);
                        mov                     rbp, rsp
                        mov                     eax, 1
                        test                    edx, edx
                        jz                      ._complete
._simd_loop:            cmp                     edx, ZMM_SIZE / COMPARE_ELEMENT_SIZE
                        jl                      ._post_simd
                        vmovdqu64               zmm0, [rdi]
                        vpcmpeqb                k1, zmm0, [rsi]
                        kmovq                   rcx, k1
                        cmp                     rcx, 0xFFFFFFFFFFFFFFFF
                        jne                     ._no_match
                        add                     rdi, ZMM_SIZE
                        add                     rsi, ZMM_SIZE
                        sub                     edx, ZMM_SIZE / COMPARE_ELEMENT_SIZE
                        jmp                     ._simd_loop
._post_simd:            test                    edx, edx
                        jz                      ._complete
                        mov                     cl, [rdi]
                        cmp                     cl, [rsi]
                        jne                     ._no_match
                        add                     rdi, COMPARE_ELEMENT_SIZE
                        add                     rsi, COMPARE_ELEMENT_SIZE
                        sub                     edx, 1
                        jmp                     ._post_simd
._no_match:             xor                     rax, rax
._complete:             pop                     rbp
                        ret

; Return item count in memory region (AVX2)
CountAvx2_Interop:      push                    rbp                                             ; int CountAvx2Interop(int* pRegion, int itemCount);
                        mov                     rbp, rsp
                        xor                     eax, eax
                        test                    esi, esi
                        jz                      ._complete
                        vxorps                  xmm0, xmm0, xmm0
                        mov                     [rbp - ELEMENT_SIZE], edx
                        vpbroadcastd            ymm2, [rbp - ELEMENT_SIZE]
._simd_loop:            cmp                     esi, YMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_simd
                        vpcmpeqd                ymm1, ymm2, [rdi]
                        vpsubd                  ymm0, ymm0, ymm1
                        add                     rdi, YMM_SIZE
                        sub                     esi, YMM_SIZE / ELEMENT_SIZE
                        jmp                     ._simd_loop
._post_simd:            vphaddd                 ymm0, ymm0, ymm0
                        vmovdqu                 [rbp - YMM_SIZE], ymm0
                        add                     eax, [rbp - YMM_SIZE + 0 * ELEMENT_SIZE]
                        add                     eax, [rbp - YMM_SIZE + 1 * ELEMENT_SIZE]
                        add                     eax, [rbp - YMM_SIZE + 4 * ELEMENT_SIZE]
                        add                     eax, [rbp - YMM_SIZE + 5 * ELEMENT_SIZE]
._post_simd_loop:       test                    esi, esi
                        jz                      ._complete
                        cmp                     edx, [rdi]
                        jne                     ._no_match
                        inc                     eax
._no_match:             add                     rdi, ELEMENT_SIZE
                        sub                     esi, 1
                        jmp                     ._post_simd_loop
._complete:             pop                     rbp
                        ret

; Return item count in memory region (AVX-512)
CountAvx512_Interop:    push                    rbp                                             ; int CountAvx512Interop(int* pRegion, int itemCount, int item);
                        mov                     rbp, rsp
                        xor                     eax, eax
                        test                    esi, esi
                        jz                      ._complete
                        vxorps                  ymm0, ymm0, ymm0
                        mov                     [rbp - ELEMENT_SIZE], edx
                        vpbroadcastd            zmm2, [rbp - ELEMENT_SIZE]
._simd_loop:            cmp                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jl                      ._post_simd
                        vpcmpeqd                k1, zmm2, [rdi]
                        vpmovm2d                zmm1, k1
                        vpsubd                  zmm0, zmm0, zmm1
                        add                     rdi, ZMM_SIZE
                        sub                     esi, ZMM_SIZE / ELEMENT_SIZE
                        jmp                     ._simd_loop
._post_simd:            vmovdqu64               [rbp - ZMM_SIZE], zmm0
                        mov                     rcx, ZMM_SIZE / ELEMENT_SIZE - 1
._post_simd_loop1:      add                     eax, [rbp - ZMM_SIZE + rcx * ELEMENT_SIZE]
                        dec                     rcx
                        jns                     ._post_simd_loop1
._post_simd_loop:       test                    esi, esi
                        jz                      ._complete
                        cmp                     edx, [rdi]
                        jne                     ._no_match
                        inc                     eax
._no_match:             add                     rdi, ELEMENT_SIZE
                        sub                     esi, 1
                        jmp                     ._post_simd_loop
._complete:             pop                     rbp
                        ret