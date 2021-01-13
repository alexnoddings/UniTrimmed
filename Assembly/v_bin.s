@ Global definitions for entry points
.global v_bin                       @ Make v_bin visible to other assembly code


@ Subroutine to print the binary contents of a 32-bit register in ascii
@ All register contents are preserved by the subroutine
@	Arguments:
@	R0: number to display
@	R2: number of bits to display
@ 	LR: return address
v_bin:
            push	{R0-R8}			@ Push the contents of registers R0-R7 to the stack
            sub	R6,R2,#1		    @ Number of bits to display minus 1
            cmp	R6,#31			    @ Most bits that can be printed (bit numbers starting at 0, leftmost will be 31)
            movhi	R6,#0			@ If larger than 31, set it to display only 1 bit
            mov	R3,R0			    @ Make the copy of R0 into R3
            mov	R4,#1			    @ Bit mask
            ldr	R5,=digits		    @ Move the digits address
            ldr	R1,=prefix
            mov	R2,#2
            mov	R0,#1			    @ Code for stdout
            mov	R7,#4			    @ Linux svc for string write
            svc	0
            mov	R2,#1			    @ Only display 1 character
            mov 	R8,#0			@ Digit printed flag

nxtbit:
            and	R1,R4,R3,LSR R6		@ Get bit 0 or 1 to display
            orr	R8,R1
            add	R1,R5
            cmp	R8,#1
            svceq	0
            subs	R6,#1			@ Decrement the bits left to display by 1
            bge	nxtbit			    @ Loop back over until R6 is -1 (wraps back up to 0xffffffff)
            cmp	R8,#0
            ldreq	R1,=digits
            svceq	0

            pop 	{R0-R8}			@ Pop the register contents of R0-R7 back to the stack
            bx	LR			        @ Return back to the address in the link register

@ Data
.data
digits:		.ascii	"01"
prefix:		.ascii	"0b"

@ End of assembly
.end
