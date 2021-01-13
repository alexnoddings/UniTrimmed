@ Global definitions for entry points
.global generate16                          @ Make generate16 visible to other assembly code


@ Subroutine to generate 3 check bits for the first 4 bits in R0
@ Register R0 has the first 4 bits preserved, the next 3 changed, and the rest are ignored
@ Registers R1 onwards have their contents preserved
@ Bits are labeled based on the provided coursework specification diagram.
generate16:     push    {R1}                @ Push R1 onto the stack to save its value

                @ Get R0 in the right output positions ready
                @ Copies a series of bits at once, then shifts R1 to drop those bits
                @ Repeats for each series of bits until it is into position
                mov     R1,R0               @ Copy R0 into R1 for shifting
                bfi     R0,R1,#2,#1         @ D0 -> TX2  (1st bit of R1 -> 2nd bit of R0)
                lsr     R1,#1               @ Shift R1 right by 1
                bfi     R0,R1,#4,#3         @ D1-D3 -> TX4-TX6  (1st 3 bits of R1 -> 4th bit of R0)
                lsr     R1,#3               @ Shift R1 right by 3
                bfi     R0,R1,#8,#7         @ D4-D10 -> TX8-TX15  (1st 7 bits of R1 -> 8th bit of R0)
                lsr     R1,#7               @ Shift R1 right by 7
                bfi     R0,R1,#16,#5        @ D11-D15 -> TX17-TX21  (1st 5 bits of R1 -> 17th bit of R0)


                @ The following 5 blocks have the same functionality as each other, with minor alterations to which
                @ Bits they are working on. The general purpose is to XOR different parts of R0 into R1 (shifts are
                @ done to get the bit we care about into position 0), and then insert bit 0 back into R0 at the correct position.

                @ Check bit 0 (TX0)
                mov     R1,#0               @ Set R1 to 0 for even parity, 1 for odd
                @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
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
                bfi     R0,R1,#0,#1         @ Bit field insert into R0 at position 0 the 1st bit of R1 (the only one useful)

                @ Check bit 1 (TX1)
                mov     R1,#0               @ Set R1 to 0 for even parity, 1 for odd
                @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
                eor     R1,R1,R0,lsr #2
                eor     R1,R1,R0,lsr #5
                eor     R1,R1,R0,lsr #6
                eor     R1,R1,R0,lsr #9
                eor     R1,R1,R0,lsr #10
                eor     R1,R1,R0,lsr #13
                eor     R1,R1,R0,lsr #14
                eor     R1,R1,R0,lsr #17
                eor     R1,R1,R0,lsr #18
                bfi     R0,R1,#1,#1         @ Bit field insert into R0 at position 1 the 1st bit of R1 (the only one useful)

                @ Check bit 2 (TX3)
                mov     R1,#0               @ Set R1 to 0 for even parity, 1 for odd
                @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
                eor     R1,R1,R0,lsr #4
                eor     R1,R1,R0,lsr #5
                eor     R1,R1,R0,lsr #6
                eor     R1,R1,R0,lsr #11
                eor     R1,R1,R0,lsr #12
                eor     R1,R1,R0,lsr #13
                eor     R1,R1,R0,lsr #14
                eor     R1,R1,R0,lsr #19
                eor     R1,R1,R0,lsr #20
                bfi     R0,R1,#3,#1         @ Bit field insert into R0 at position 3 the 1st bit of R1 (the only one useful)

                @ Check bit 3 (TX7)
                mov     R1,#0               @ Set R1 to 0 for even parity, 1 for odd
                @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
                eor     R1,R1,R0,lsr #8
                eor     R1,R1,R0,lsr #9
                eor     R1,R1,R0,lsr #10
                eor     R1,R1,R0,lsr #11
                eor     R1,R1,R0,lsr #12
                eor     R1,R1,R0,lsr #13
                eor     R1,R1,R0,lsr #14
                bfi     R0,R1,#7,#1         @ Bit field insert into R0 at position 7 the 1st bit of R1 (the only one useful)

                @ Check bit 4 (TX15)
                mov     R1,#0               @ Set R1 to 0 for even parity, 1 for odd
                @ XOR values from R0 into R1 one by one, shifting to get the right bit in pos 0
                eor     R1,R1,R0,lsr #16
                eor     R1,R1,R0,lsr #17
                eor     R1,R1,R0,lsr #18
                eor     R1,R1,R0,lsr #19
                eor     R1,R1,R0,lsr #20
                bfi     R0,R1,#15,#1         @ Bit field insert into R0 at position 15 the 1st bit of R1 (the only one useful)

                pop     {R1}              @ Pop off the stack the original value for R1
                bx      LR                @ Branch back to the calling code

@ End of assembly
.end
