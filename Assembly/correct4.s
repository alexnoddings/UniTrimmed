@ Global definitions for entry points
.global correct4                            @ Make correct4 visible to other assembly code


@ Subroutine to correct a 4 bit word in R0 based on the given 3 parity bits
@ Register R0 has the first 4 bits corrected, and the rest zeroed
@ Registers R1 onwards have their contents preserved
@ Bits and blocks are labeled based on the provided specification diagram
correct4:     
            push    {R1-R3}                 @ Push R1-R3 onto the stack to save their values

            mov     R3,#0                   @ Clear out R3

            @ The following 3 blocks have the same functionality as each other, with minor alterations to which
            @ Bits they are working on. The general purpose is to XOR different parts of R0 into R1 (shifting to
            @ focus on bit 0 in R1), then to save the generated bit into R3 for use later.

            @ Block A: Bit 0 of R3 (RX0 ^ RX2 ^ RX3 ^ RX4)
            mov     R1,#0                   @ Clear out R1
            eor     R1,R1,R0, lsr #4        @ XOR with the 4th bit of R0
            eor     R1,R1,R0, lsr #3        @ XOR with the 3rd bit of R0
            eor     R1,R1,R0, lsr #2        @ XOR with the 2nd bit of R0
            eor     R1,R1,R0, lsr #0        @ XOR with the 0th bit of R0
            bfi     R3,R1,#0,#1             @ Bitfield insert into R3 the 1st bit of R1 at pos 0

            @ Block B: Bit 1 of R3 (RX5 ^ RX3 ^ RX2 ^ RX1)
            mov     R1,#0                   @ Clear out R1 again
            eor     R1,R1,R0, lsr #5        @ XOR with the 5th bit of R0
            eor     R1,R1,R0, lsr #3        @ XOR with the 3rd bit of R0
            eor     R1,R1,R0, lsr #2        @ XOR with the 2nd bit of R0
            eor     R1,R1,R0, lsr #1        @ XOR with the 1st bit of R0
            bfi     R3,R1,#1,#1             @ Bitfield insert into R3 the 1st bit of R1 at pos 1

            @ Block C: Bit 2 of R3 (RX6 ^ RX3 ^ RX1 ^ RX0)
            mov     R1,#0                   @ Clear out R1 again
            eor     R1,R1,R0, lsr #6        @ XOR with the 6th bit of R0
            eor     R1,R1,R0, lsr #3        @ XOR with the 3rd bit of R0
            eor     R1,R1,R0, lsr #1        @ XOR with the 1st bit of R0
            eor     R1,R1,R0, lsr #0        @ XOR with the 0th bit of R0
            bfi     R3,R1,#2,#1             @ Bitfield insert into R3 the 1st bit of R1 at pos 2

            @ Setup ready for part 2
            mov     R2,R0                   @ Copy R0 into R2
            mov     R0,#0                   @ Then clear out R0 for our corrected word

            @ The following 4 blocks have the same functionality as each other, with minor alterations to which
            @ Bits they are working on. The general purpose is to AND and NAND the outputs from the above blocks (in R3)
            @ and then XOR the output of that with the original data bit to generate the output bit into R0.

            @ D0 bit = (A && !B && C) ^ RX0
            mov     R1,R3, lsr #0           @ Take the 0th bit of R3
            bic     R1,R1,R3, lsr #1        @ Bic (nand) it with the 1st bit
            and     R1,R1,R3, lsr #2        @ And it with the 2nd bit
            eor     R1,R1,R2, lsr #0        @ Eor (xor) with bit 0 of R2 to get the output bit
            bfi     R0,R1,#0,#1             @ Insert into R0 at position 0, the 1st bit from R1

            @ D1 bit = (!A && B && C) ^ RX1
            mov     R1,R3, lsr #1           @ Take the 1st bit of R3
            bic     R1,R1,R3, lsr #0        @ Bic (nand) it with the 0th bit
            and     R1,R1,R3, lsr #2        @ And it with the 2nd bit
            eor     R1,R1,R2, lsr #1        @ Eor (xor) with bit 1 of R2 to get the output bit
            bfi     R0,R1,#1,#1             @ Insert into R0 at position 1, the 1st bit from R1

            @ D2 bit = (A && B && !C) ^ RX2
            mov     R1,R3, lsr #0           @ Take the 0th bit of R3
            and     R1,R1,R3, lsr #1        @ And it with the 1st bit
            bic     R1,R1,R3, lsr #2        @ Bic (nand) it with the 2nd bit
            eor     R1,R1,R2, lsr #2        @ Eor (xor) with bit 2 of R2 to get the output bit
            bfi     R0,R1,#2,#1             @ Insert into R0 at position 2, the 1st bit from R1

            @ D3 bit = (A && B && C) ^ RX3
            mov     R1,R3, lsr #0           @ Take the 0th bit of R3
            and     R1,R1,R3, lsr #1        @ And it with the 1st bit
            and     R1,R1,R3, lsr #2        @ And it with the 2nd bit
            eor     R1,R1,R2, lsr #3        @ Eor (xor) with bit 3 of R2 to get the output bit
            bfi     R0,R1,#3,#1             @ Insert into R0 at position 3, the 1st bit from R1

            pop     {R1-R3}                 @ Pop off the stack the original values for R1-R3
            bx      LR                      @ Branch back to the calling code

@ End of assembly
.end
