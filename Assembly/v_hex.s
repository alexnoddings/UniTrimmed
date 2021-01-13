@ Global definitions for entry points
.global v_hex                       @ Make v_hex visible to other assembly code


v_hex:
            push  {R0-R11}          @ Save reg contents
            mov   R3,R0             @ R3 holds a copy of the input
            mov   R4,#0b1111        @ 4 bits mask

            @ Set registers up to display 1 char at a time
            ldr   R1,=pre           @ load the prefix string
            mov   R2,#2             @ Display the prefix (2 chars)
            mov   R0,#1             @ Stdout code
            mov   R7,#4             @ Linux svc code for string write
            svc   0                 @ print the prefix
            ldr   R5,=dig           @ Pointer to the digits string
            mov   R2,#1             @ No of chars to display
            mov   R8,#0             @ chars been printed flag
            mov   R6,#28            @ number of bits per hex digit

            @ Loop over 4 bit groups
            nxthex:
            and   R1,R4,R3,LSR R6   @ get next digit addr to display
            orrs  R8,R1             @ or R8 with R1 for the zero displayed flag and set flags
            addne R1,R5             @ if flags not equal (non-zero) set position to print at
            svcne 0                 @ if flags not equal call svc 0 to print
            subs  R6,#4             @ decrement bits by 4, set flags
            bge   nxthex            @ loop again if bits to print is still positive

            cmp   R8,#0             @ check if anything has been printed
            ldreq R1,=dig           @ if not load the digits address
            svceq 0                 @ call svc 0 to print first digit (0)

            pop   {R0-R11}          @ pop back from stack to maintain original contents
            bx    LR                @ branch back

@ Data
.data
dig:        .ascii "0123456789ABCDEF"
pre:        .ascii "0x"

@ End of assembly
.end
