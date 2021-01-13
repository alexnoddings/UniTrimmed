@ Global definitions for entry points
.global generate4                             @ Make generate4 visible to other assembly code


@ Subroutine to generate 3 check bits for the first 4 bits in R0
@ Register R0 has the first 4 bits preserved, the next 3 changed, and the rest are ignored
@ Registers R1 onwards have their contents preserved
@ Bits are labeled based on the provided coursework specification diagram.
generate4:    push    {R1}                    @ Push R1 onto the stack to save its value

              @ The following 3 blocks have the same functionality as each other, with minor alterations to which
              @ Bits they are working on. The general purpose is to XOR different parts of R0 into R1 (shifting to
              @ focus on bit 0 in R1), then to save the generated bit back onto the end of R0

              @ TX4 bit = (D0 ^ D2 ^ D3)
              mov     R1,#0                   @ Set R1 to be 0 for even parity
              eor     R1,R1,R0,lsr #0         @ XOR the current parity bit with bit 0 (gotten from logical shifting the number right)
              eor     R1,R1,R0,lsr #2         @ XOR the current parity bit with bit 2 (gotten from logical shifting the number right)
              eor     R1,R1,R0,lsr #3         @ XOR the current parity bit with bit 3 (gotten from logical shifting the number right)
              and     R1,#1                   @ And the number with 0b1 to mask the rightmost bit. Only shifting is done with the XORs, not masking, so the number in R1 may have extra bits
              bfi     R0,R1,#4,#1             @ Bit field insert into the 4th position of R0 the 1st bit of R1

              @ TX5 bit = (D1 ^ D2 ^ D3)
              mov     R1,#0                   @ Set R1 to be 0 for even parity
              eor     R1,R1,R0,lsr #1         @ XOR the current parity bit with bit 1 (gotten from logical shifting the number right)
              eor     R1,R1,R0,lsr #2         @ XOR the current parity bit with bit 2 (gotten from logical shifting the number right)
              eor     R1,R1,R0,lsr #3         @ XOR the current parity bit with bit 3 (gotten from logical shifting the number right)
              and     R1,#1                   @ And the number with 0b1 to mask the rightmost bit. Only shifting is done with the XORs, not masking, so the number in R1 may have extra bits
              bfi     R0,R1,#5,#1             @ Bit field insert into the 5th position of R0 the 1st bit of R1

              @ TX6 bit = (D0 ^ D1 ^ D3)
              mov     R1,#0                   @ Set R1 to be 0 for even parity
              eor     R1,R1,R0,lsr #0         @ XOR the current parity bit with bit 0 (gotten from logical shifting the number right)
              eor     R1,R1,R0,lsr #1         @ XOR the current parity bit with bit 1 (gotten from logical shifting the number right)
              eor     R1,R1,R0,lsr #3         @ XOR the current parity bit with bit 3 (gotten from logical shifting the number right)
              and     R1,#1                   @ And the number with 0b1 to mask the rightmost bit. Only shifting is done with the XORs, not masking, so the number in R1 may have extra bits
              bfi     R0,R1,#6,#1             @ Bit field insert into the 6th position of R0 the 1st bit of R1

              pop     {R1}                    @ Pop off the stack the original value for R1
              bx      LR                      @ Branch back to the calling code

@ End of assembly
.end
