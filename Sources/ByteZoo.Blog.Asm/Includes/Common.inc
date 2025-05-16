; -----------------------------------------------------------------------------------------------
; Common Configuration
; -----------------------------------------------------------------------------------------------

                        default                 rel                                             ; Use RIP-relative memory addressing
                        section                 .note.GNU-stack noalloc noexec nowrite progbits ; Mark stack as non-executable for Binutils 2.39+