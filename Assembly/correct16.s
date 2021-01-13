@ Global definitions for entry points
.global correct16                          @ Make correct16 visible to other assembly code


@ Subroutine to generate a corrected 16 bit sequence, given 21 bits
@ Register R0 has the first 16 bits set to their correct values, and the rest zeroed
@ Registers R1 onwards have their contents preserved
@ Bits and blocks are labeled based on the provided specification diagram
correct16:      
            push    {R1-R3}                @ Push R1-R3 onto the stack to save its value

            @ The following 5 blocks have the same functionality as each other, with minor alterations to which
            @ Bits they are working on. The general purpose is to XOR different parts of R0 into R1 (shifts are
            @ done to get the bit we care about into position 0), and then insert bit 0 back into R0 at the correct position.
            @ The code is more or less recycled from the generate16 exercise, except this time the parity bit is also XOR'd

            @ Generate our own parity bit 0, based on the original parity bit and the bits that is meant to check
            mov     R1,#0                   @ Set R1 to 0 for even parity, 1 for odd
            @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
            eor     R1,R1,R0,lsr #0
            eor     R1,R1,R0,lsr #2
            eor     R1,R1,R0,lsr #4
            eor     R1,R1,R0,lsr #6
            eor     R1,R1,R0,lsr #8
            eor     R1,R1,R0,lsr #10
            eor     R1,R1,R0,lsr #12
            eor     R1,R1,R0,lsr #14
            eor     R1,R1,R0,lsr #16
            eor     R1,R1,R0,lsr #18
            eor     R1,R1,R0,lsr #20
            bfi     R2,R1,#0,#1             @ Bit field insert into R2 at position 0 the 1st bit of R1 (the only one useful)

            @ Generate our own parity bit 1, based on the original parity bit and the bits that is meant to check
            mov     R1,#0                   @ Set R1 to 0 for even parity, 1 for odd
            @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
            eor     R1,R1,R0,lsr #1
            eor     R1,R1,R0,lsr #2
            eor     R1,R1,R0,lsr #5
            eor     R1,R1,R0,lsr #6
            eor     R1,R1,R0,lsr #9
            eor     R1,R1,R0,lsr #10
            eor     R1,R1,R0,lsr #13
            eor     R1,R1,R0,lsr #14
            eor     R1,R1,R0,lsr #17
            eor     R1,R1,R0,lsr #18
            bfi     R2,R1,#1,#1             @ Bit field insert into R2 at position 1 the 1st bit of R1 (the only one useful)

            @ Generate our own parity bit 2, based on the original parity bit and the bits that is meant to check
            mov     R1,#0                   @ Set R1 to 0 for even parity, 1 for odd
            @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
            eor     R1,R1,R0,lsr #3
            eor     R1,R1,R0,lsr #4
            eor     R1,R1,R0,lsr #5
            eor     R1,R1,R0,lsr #6
            eor     R1,R1,R0,lsr #11
            eor     R1,R1,R0,lsr #12
            eor     R1,R1,R0,lsr #13
            eor     R1,R1,R0,lsr #14
            eor     R1,R1,R0,lsr #19
            eor     R1,R1,R0,lsr #20
            bfi     R2,R1,#2,#1             @ Bit field insert into R2 at position 2 the 1st bit of R1 (the only one useful)

            @ Generate our own parity bit 3, based on the original parity bit and the bits that is meant to check
            mov     R1,#0                   @ Set R1 to 0 for even parity, 1 for odd
            @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
            eor     R1,R1,R0,lsr #7
            eor     R1,R1,R0,lsr #8
            eor     R1,R1,R0,lsr #9
            eor     R1,R1,R0,lsr #10
            eor     R1,R1,R0,lsr #11
            eor     R1,R1,R0,lsr #12
            eor     R1,R1,R0,lsr #13
            eor     R1,R1,R0,lsr #14
            bfi     R2,R1,#3,#1             @ Bit field insert into R2 at position 3 the 1st bit of R1 (the only one useful)

            @ Generate our own parity bit 4, based on the original parity bit and the bits that is meant to check
            mov     R1,#0                   @ Set R1 to 0 for even parity, 1 for odd
            @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
            eor     R1,R1,R0,lsr #15
            eor     R1,R1,R0,lsr #16
            eor     R1,R1,R0,lsr #17
            eor     R1,R1,R0,lsr #18
            eor     R1,R1,R0,lsr #19
            eor     R1,R1,R0,lsr #20
            bfi     R2,R1,#4,#1             @ Bit field insert into R2 at position 4 the 1st bit of R1 (the only one useful)

            @ Shift the values back into 16-bit position into R3
            mov     R3,#0                   @ Reset R3
            lsr     R0,#2                   @ Get rid of the first 2 parity bits
            bfi     R3,R0,#0,#1             @ Bit field insert into R3 at position 0 the 1st bit from R0
            lsr     R0,#2                   @ Get rid of the used bit and next parity bit
            bfi     R3,R0,#1,#3             @ Bit field insert into R3 at position 1 the 1st 3 bits from R0
            lsr     R0,#4                   @ Get rid of the used bits and next parity bit
            bfi     R3,R0,#4,#7             @ Bit field insert into R3 at position 4 the 1st 7 bits from R0
            lsr     R0,#8                   @ Get rid of the used bits and next parity bit
            bfi     R3,R0,#11,#5            @ Bit field insert into R3 at position 11 the 1st 5 bits from R0


            mov     R0,#0                   @ Reset R0


            @ The following 16 blocks have the same functionality as each other, with minor alterations to which
            @ Bits they are working on and whether they AND or NAND them. The general purpose is to AND and NAND different
            @ parts of R2 into R1 (logical shifts are done to get the bit we care about into position 0), and then insert bit
            @ 0 back into R0 at the correct position. The comments above each section show whether bits are ANDed or NANDed,
            @ with a ! preceding a check bit denoting it needs NANDing. The bits moved in initially change sometimes based on
            @ the first bit that needs ANDing.  bic == nand

            @ D0 = c0 c1 !c2 !c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            and     R1,R1,R2,lsr #1         @ AND  C1
            bic     R1,R1,R2,lsr #2         @ NAND C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #0         @ XOR  D0
            bfi     R0,R1,#0,#1             @ Insert generated bit into D0

            @ D1 = c0 !c1 c2 !c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            bic     R1,R1,R2,lsr #1         @ NAND C1
            and     R1,R1,R2,lsr #2         @ AND  C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #1         @ XOR  D1
            bfi     R0,R1,#1,#1             @ Insert generated bit into D0

            @ D2 = !c0 c1 c2 !c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #1            @ Get  C1
            bic     R1,R1,R2,lsr #0         @ NAND C0
            and     R1,R1,R2,lsr #2         @ AND  C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #2         @ XOR  D2
            bfi     R0,R1,#2,#1             @ Insert generated bit into D0

            @ D3 = c0 c1 c2 !c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            and     R1,R1,R2,lsr #1         @ AND C1
            and     R1,R1,R2,lsr #2         @ AND  C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #3         @ XOR  D3
            bfi     R0,R1,#3,#1             @ Insert generated bit into D0

            @ D4 = c0 !c1 !c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            bic     R1,R1,R2,lsr #1         @ NAND C1
            bic     R1,R1,R2,lsr #2         @ NAND  C2
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #4         @ XOR  D4
            bfi     R0,R1,#4,#1             @ Insert generated bit into D0

            @ D5 = !c0 c1 !c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #1            @ Get  C1
            bic     R1,R1,R2,lsr #0         @ NAND C0
            bic     R1,R1,R2,lsr #2         @ NAND  C2
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #5         @ XOR  D5
            bfi     R0,R1,#5,#1             @ Insert generated bit into D0

            @ D6 = c0 c1 !c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C1
            and     R1,R1,R2,lsr #1         @ AND C0
            bic     R1,R1,R2,lsr #2         @ NAND  C2
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #6         @ XOR  D6
            bfi     R0,R1,#6,#1             @ Insert generated bit into D0

            @ D7 = !c0 !c1 c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #2            @ Get  C2
            bic     R1,R1,R2,lsr #1         @ NAND C1
            bic     R1,R1,R2,lsr #0         @ NAND  C0
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #7         @ XOR  D7
            bfi     R0,R1,#7,#1             @ Insert generated bit into D0

            @ D8 = c0 !c1 c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            bic     R1,R1,R2,lsr #1         @ NAND C1
            and     R1,R1,R2,lsr #2         @ AND  C2
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #8         @ XOR  D8
            bfi     R0,R1,#8,#1             @ Insert generated bit into D0

            @ D9 = !c0 c1 c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #1            @ Get  C1
            bic     R1,R1,R2,lsr #0         @ NAND C0
            and     R1,R1,R2,lsr #2         @ AND  C2
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #9         @ XOR  D9
            bfi     R0,R1,#9,#1             @ Insert generated bit into D0

            @ D10 = c0 c1 c2 c3 !c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            and     R1,R1,R2,lsr #1         @ AND C1
            and     R1,R1,R2,lsr #2         @ AND C2
            and     R1,R1,R2,lsr #3         @ AND C3
            bic     R1,R1,R2,lsr #4         @ NAND C4
            eor     R1,R1,R3,lsr #10        @ XOR  D10
            bfi     R0,R1,#10,#1            @ Insert generated bit into D0

            @ D11 = c0 !c1 !c2 !c3 c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            bic     R1,R1,R2,lsr #1         @ NAND C1
            bic     R1,R1,R2,lsr #2         @ NAND C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            and     R1,R1,R2,lsr #4         @ AND C4
            eor     R1,R1,R3,lsr #11        @ XOR  D11
            bfi     R0,R1,#11,#1            @ Insert generated bit into D0

            @ D12 = !c0 c1 !c2 !c3 c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #1            @ Get  C1
            bic     R1,R1,R2,lsr #0         @ NAND C0
            bic     R1,R1,R2,lsr #2         @ NAND C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            and     R1,R1,R2,lsr #4         @ AND C4
            eor     R1,R1,R3,lsr #12        @ XOR  D12
            bfi     R0,R1,#12,#1            @ Insert generated bit into D0

            @ D13 = c0 c1 !c2 !c3 c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            and     R1,R1,R2,lsr #1         @ AND C1
            bic     R1,R1,R2,lsr #2         @ NAND C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            and     R1,R1,R2,lsr #4         @ AND C4
            eor     R1,R1,R3,lsr #13        @ XOR  D13
            bfi     R0,R1,#13,#1            @ Insert generated bit into D0

            @ D14 = !c0 !c1 c2 !c3 c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #2            @ Get  C2
            bic     R1,R1,R2,lsr #1         @ NAND C1
            bic     R1,R1,R2,lsr #0         @ NAND C0
            bic     R1,R1,R2,lsr #3         @ NAND C3
            and     R1,R1,R2,lsr #4         @ AND C4
            eor     R1,R1,R3,lsr #14        @ XOR  D14
            bfi     R0,R1,#14,#1            @ Insert generated bit into D0

            @ D15 = c0 !c1 c2 !c3 c4
            mov     R1,#0                   @ Reset R1
            mov     R1,R2,lsr #0            @ Get  C0
            bic     R1,R1,R2,lsr #1         @ NAND C1
            and     R1,R1,R2,lsr #2         @ AND C2
            bic     R1,R1,R2,lsr #3         @ NAND C3
            and     R1,R1,R2,lsr #4         @ AND C4
            eor     R1,R1,R3,lsr #15        @ XOR  D15
            bfi     R0,R1,#15,#1            @ Insert generated bit into D0


            pop     {R1-R3}                 @ Pop off the stack the original values for R1-R3
            bx      LR                      @ Branch back to the calling code

@ End of assembly
.end
