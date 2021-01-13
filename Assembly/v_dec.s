@ Global definitions for entry points
.global v_dec                   @ Make v_dec visible to other assembly code

v_dec:  
        push    {R0-R9}         @ save register contents to stack
        mov     R3,R0           @ move a copy of input into r3
        mov     R0,#1           @ stdout code
        mov     R2,#1           @ chars to print
        mov     R7,#4           @ svc command for str write
        mov     R8,#0           @ -ve number flag
        mov     R9,#0           @ no of digits left before printing a ,

        cmp     R3,#0           @ compare num to 0
        movlt   R8,#1           @ if less than, set the -ve flag to be 1
        ldrlt   R1,=bopen       @ if less than, load the open bracket char
        svclt   0               @ if less than, call svc 0 to print
        rsblt   R3,R3,#0        @ make the value +ve

        cmp     R3,#10          @ if R3 is 10, only 1 column is needed
        blt     fnlcol          @ branch to the last column print

        ldr     R6,=pow10+8     @ point to 10^2
nxtpow: ldr     R5,[R6],#4      @ load next highest pow
        add     R9,#1           @ add 1 to comma pos counter
        cmp     R9,#3           @ if it is 3, reset back to 0
        moveq   R9,#0   
        cmp     R3,R5           @ compare if the highest val has been reached
        bge     nxtpow          @ if value is greater, continue finding next power
        sub     R6,#8           @ gone 2 ints to far, go back

nxtdig: ldr     R1,=dig-1       @ point to first byte of the digits
        ldr     R5,[R6],#-4     @ load the next pow of 10

mod10:  add     R1,#1           @ point r1 to the next digit to print
        subs    R3,R5           @ count down to the next digit
        bge     mod10           @ keep going
        addlt   R3,R5           @ counted too far
        svc     0               @ write digit to display
        cmp     R9,#0           @ check if comma counter is at 0
        moveq   R9,#2           @ reset back to 2 if it is
        ldreq   R1,=comma       @ load the comma if it is
        svceq   0               @ svc 0 to print the char if it is
        subne   R9,#1           @ otherwise subtract 1 from R9
        cmp     R5,#10          @ test if at the 10s yet
        bgt     nxtdig          @ if 1s, output rightmost and return

fnlcol: ldr     R1,=dig         @ point to digits
        add     R1,R3           @ get the offset
        svc     0               @ print

        cmp     R8,#1           @ if -ve flag set
        ldreq   R1,=bclose      @ load bracket close char
        svceq   0               @ print it

        pop     {R0-R9}         @ pop values back from stack
        bx      LR              @ branch back

pow10:  .word   1               @ 10^0 through 10^9
        .word   10
        .word   100
        .word   1000
        .word   10000
        .word   100000
        .word   1000000
        .word   10000000
        .word   100000000
        .word   1000000000
        .word   0x7FFFFFFF      @ largest int possible with 31 bits
dig:    .ascii  "0123456789"    @ ascii string of digits
bopen:  .ascii  "("             @ open bracket char
bclose: .ascii  ")"             @ close bracket char
comma:  .ascii  ","             @ comma char

@ End of assembly
.end
