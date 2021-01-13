theory dotsandboxes
  imports "../VDMToolkit"
begin

(*************************************************************************)
section \<open> VDM header \<close>
text \<open>\begin{vdmsl}[breaklines=true]
  module dotsandboxes
  exports all
  definitions
\end{vdmsl}\<close>

(*************************************************************************)
section \<open> Miscellaneous Helpers \<close>
text \<open> Miscellaneous functions that are used in but are not part of the model. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> take[@T] \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Takes from n elements from ts.
-- List comprehension would be an alternative but isn't nice to do in Isabelle.
take[@T]: seq of @T * nat -> seq of @T
take(ts, n) ==
	cases n:
		0 -> [],
		N -> take[@T](ts, N-1) ^ [ts(N)]
	end
-- Cannot take more elements than there are.
pre n <= len ts
-- The ammount of results will be the number of elements asked for.
post len RESULT = n
-- The index of the element to take will decrement by 1 every call.
measure n;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
fun take :: "'a VDMSeq \<Rightarrow> \<nat> \<Rightarrow> 'a VDMSeq" where
  take0 : "take ts 0 = []"
| takeN : "take ts (Suc n) = (take ts n) @ [ts ! n]"

subsubsection \<open> Precondition \<close>
definition pre_take :: "'a VDMSeq \<Rightarrow> \<nat> \<Rightarrow> \<bool>" where
  "pre_take ts n \<equiv> 
    n < length ts"

subsubsection \<open> Postcondition \<close>
definition post_take :: "'a VDMSeq \<Rightarrow> \<nat> \<Rightarrow> 'a VDMSeq \<Rightarrow> \<bool>" where
  "post_take ts n RESULT \<equiv> 
    length RESULT = n"

subsubsection \<open> Measure \<close>
definition measure_take :: "'a VDMSeq \<Rightarrow> \<nat> \<Rightarrow> \<nat>" where
  "measure_take ts n \<equiv> n"

lemma l_take_n_len:
  "pre_take ts n \<Longrightarrow>
    length (take ts n) = n"
  unfolding pre_take_def
  apply (induct ts)
   apply auto
  apply (induct n)
   apply auto
  by (metis (full_types) Suc_length_conv Suc_lessI less_SucI)

subsubsection \<open> Satisfiability Proof Obligation \<close>
text \<open> Proof is done using \<nat> as the function is completely generic, so proving it for any type will prove it for all \<close>
definition PO_take_sat_obl :: "\<bool>" where
  "PO_take_sat_obl \<equiv> 
    \<forall> (ts::(\<nat> VDMSeq)) n .
      pre_take ts n \<longrightarrow> 
        (let RESULT = (take ts n) in
          post_take ts n RESULT)"

theorem PO_take_sat_obl
  by (simp add: PO_take_sat_obl_def l_take_n_len post_take_def)

(*-----------------------------------------------------------------------*)
subsection \<open> are_no_duplicates \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If a sequence contains no duplicate elements.
--	This is calculated by checking the cardinality of the set of ts and the length of ts.
--	As sets cannot have duplicates, then if the cardinality of a set of ts is the same as the length of ts
-- 	then there cannot be any duplicates in ts.
-- Note: this was modified from the initial version as it was easier to prove in Isabelle
are_no_duplicates[@T]: seq of @T -> bool
are_no_duplicates(ts) ==
	card (elems ts) = len ts
-- No precondition. An empty sequence will have no duplicates and would not signal an issue.
pre true
-- No postcondition.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition are_no_duplicates :: "'a VDMSeq \<Rightarrow> \<bool>" where
  "are_no_duplicates ts \<equiv>
    card (set ts) = length ts"

subsubsection \<open> Precondition \<close>
definition pre_are_no_duplicates :: "'a VDMSeq \<Rightarrow> \<bool>" where
  "pre_are_no_duplicates lines \<equiv> True"

subsubsection \<open> Postcondition \<close>
definition post_are_no_duplicates :: "'a VDMSeq \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_are_no_duplicates lines RESULT \<equiv> True"

subsubsection \<open> Lemmas \<close>
lemma l_are_no_duplicates_empty_true:
  "are_no_duplicates [] = True"
  by (simp add: are_no_duplicates_def)

lemma l_are_no_duplicates_append_no_duplicates:
  "\<forall> t ts . 
    are_no_duplicates (ts) 
    \<and> t \<notin> elems (ts) \<longrightarrow> 
      are_no_duplicates (ts @ [t])"
  unfolding are_no_duplicates_def
  by (simp add: elems_def)

subsubsection \<open> Satisfiability Proof Obligation \<close>
text \<open> Proof is done using \<nat> as the function is completely generic, so proving it for any type will prove it for all \<close>
definition PO_are_no_duplicates_sat_obl :: "\<bool>" where
  "PO_are_no_duplicates_sat_obl \<equiv> 
    \<forall> (ts::(\<nat> VDMSeq)) . 
      pre_are_no_duplicates ts \<longrightarrow> 
        (let RESULT = (are_no_duplicates ts) in
          post_are_no_duplicates ts RESULT)"

theorem PO_are_no_duplicates_sat_obl
  by (simp add: PO_are_no_duplicates_sat_obl_def post_are_no_duplicates_def)

(*************************************************************************)
section \<open> Player \<close>
text \<open> Used to denominate which player has won/played a certain move. \<close>
(*-----------------------------------------------------------------------*)

subsection \<open> Player \<close>
text \<open>\begin{vdmsl}[breaklines=true]
Player = <Player1> | <Player2>;
\end{vdmsl}\<close>
datatype Player = Player1 | Player2

(*-----------------------------------------------------------------------*)
subsection \<open> other_player \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Swaps one player for the other.
other_player: Player -> Player
other_player(player) ==
	if player = <Player1> then <Player2>
												else <Player1>
-- No precondition.
pre true
-- The result cannot be the same as the first.
post RESULT <> player;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition other_player :: "Player \<Rightarrow> Player" where
  "other_player player \<equiv> (
    if player = Player1 then 
      Player2 
    else 
      Player1)"

subsubsection \<open> Precondition \<close>
definition pre_other_player :: "Player \<Rightarrow> \<bool>" where
  "pre_other_player player \<equiv> True"

subsubsection \<open> Postcondition \<close>
definition post_other_player :: "Player \<Rightarrow> Player \<Rightarrow> \<bool>" where
  "post_other_player player RESULT \<equiv> 
    player \<noteq> RESULT"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_other_player_sat_obl :: "\<bool>" where
  "PO_other_player_sat_obl \<equiv> 
    \<forall> p . 
      pre_other_player p \<longrightarrow> 
        (let RESULT = (other_player p) in
          post_other_player p RESULT)"

theorem PO_other_player_sat_obl
  by (simp add: PO_other_player_sat_obl_def other_player_def post_other_player_def)

(*-----------------------------------------------------------------------*)
subsection \<open> FIRST_PLAYER \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Who plays first
FIRST_PLAYER = <Player1>;
\end{vdmsl}\<close>
abbreviation
  FIRST_PLAYER :: Player where "FIRST_PLAYER \<equiv> Player1"

(*-----------------------------------------------------------------------*)
subsection \<open> SECOND_PLAYER \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Who plays second.
SECOND_PLAYER = other_player(FIRST_PLAYER);
\end{vdmsl}\<close>
abbreviation
  SECOND_PLAYER :: Player where "SECOND_PLAYER \<equiv> other_player(FIRST_PLAYER)"

(*************************************************************************)
section \<open> Grid Size \<close>
text \<open> Represents the width or height of the grid. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> GridSize \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Grids must be at least 2x2, otherwise a box can't be formed.
GridSize = nat1
inv gridSize == gridSize >= 2;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
type_synonym GridSize = "\<nat>"

subsubsection \<open> Invariant \<close>
definition inv_gridSize :: "GridSize \<Rightarrow> \<bool>" where
  "inv_gridSize gridSize \<equiv> gridSize \<ge> 2"

(*-----------------------------------------------------------------------*)
subsection \<open> GRID_WIDTH \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- How wide the grid will be.
GRID_WIDTH: GridSize = 3;
\end{vdmsl}\<close>
abbreviation
  GRID_WIDTH :: GridSize where "GRID_WIDTH \<equiv> 3"

subsubsection \<open> Lemmas \<close>
lemma l_inv_GRID_WIDTH:
  "inv_gridSize GRID_WIDTH"
  by (simp add: inv_gridSize_def)

(*-----------------------------------------------------------------------*)
subsection \<open> GRID_HEIGHT \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- How high the grid will be.
GRID_HEIGHT: GridSize = 3;
\end{vdmsl}\<close>
abbreviation
  GRID_HEIGHT :: GridSize where "GRID_HEIGHT \<equiv> 3"

subsubsection \<open> Lemmas \<close>
lemma l_inv_GRID_HEIGHT:
  "inv_gridSize GRID_HEIGHT"
  by (simp add: inv_gridSize_def)

(*************************************************************************)
section \<open> DotX, DotY \<close>
text \<open> Represents the position of a dot in the grid. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> DOT_X_MAX \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- The maximum value a dot's x position can take. GRID_WIDTH - 1 as dots are 0-indexed.
DOT_X_MAX: nat = GRID_WIDTH - 1;
\end{vdmsl}\<close>
abbreviation
  DOT_X_MAX :: \<nat> where "DOT_X_MAX \<equiv> GRID_WIDTH - 1"

(*-----------------------------------------------------------------------*)
subsection \<open> DOT_Y_MAX \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- The maximum value a dot's y position can take. GRID_HEIGHT - 1 as dots are 0-indexed.
DOT_Y_MAX: nat = GRID_HEIGHT - 1;
\end{vdmsl}\<close>
abbreviation
  DOT_Y_MAX :: \<nat> where "DOT_Y_MAX \<equiv> GRID_HEIGHT - 1"

(*-----------------------------------------------------------------------*)
subsection \<open> DotX \<close>
text \<open>\begin{vdmsl}[breaklines=true]
DotX = nat
inv dotX == dotX <= DOT_X_MAX;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
type_synonym DotX = "\<nat>"

subsubsection \<open> Invariant \<close>
definition inv_DotX :: "DotX \<Rightarrow> \<bool>" where
  "inv_DotX dotX \<equiv> dotX \<le> DOT_X_MAX"

(*-----------------------------------------------------------------------*)
subsection \<open> DotY \<close>
text \<open>\begin{vdmsl}[breaklines=true]
DotY = nat
inv dotY == dotY <= DOT_Y_MAX;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
type_synonym DotY = "\<nat>"

subsubsection \<open> Invariant \<close>
definition inv_DotY :: "DotY \<Rightarrow> \<bool>" where
  "inv_DotY dotY \<equiv> dotY \<le> DOT_Y_MAX"

(*************************************************************************)
section \<open> BoxX, BoxY \<close>
text \<open> Represents the position of a box in the grid. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> BOX_X_MAX \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- The maximum value a box could be found at on the X axis. GRID_WIDTH - 2 as box positions are based on the top left dot, and are 0-indexed.
BOX_X_MAX: nat = GRID_WIDTH - 2;
\end{vdmsl}\<close>
abbreviation
  BOX_X_MAX :: \<nat> where "BOX_X_MAX \<equiv> GRID_WIDTH - 2"

(*-----------------------------------------------------------------------*)
subsection \<open> BOX_Y_MAX \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- The maximum value a box could be found at on the Y axis. GRID_HEIGHT - 2 as box positions are based on the top left dot, and are 0-indexed.
BOX_Y_MAX: nat = GRID_HEIGHT - 2;
\end{vdmsl}\<close>
abbreviation
  BOX_Y_MAX :: \<nat> where "BOX_Y_MAX \<equiv> GRID_HEIGHT - 2"

(*-----------------------------------------------------------------------*)
subsection \<open> BoxX \<close>
text \<open>\begin{vdmsl}[breaklines=true]
BoxX = nat
inv boxX == boxX <= BOX_X_MAX;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
type_synonym BoxX = "\<nat>"

subsubsection \<open> Invariant \<close>
definition inv_BoxX :: "BoxX \<Rightarrow> \<bool>" where
  "inv_BoxX boxX \<equiv> boxX \<le> BOX_X_MAX"

(*-----------------------------------------------------------------------*)
subsection \<open> BoxY \<close>
text \<open>\begin{vdmsl}[breaklines=true]
BoxY = nat
inv boxY == boxY <= BOX_Y_MAX;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
type_synonym BoxY = "\<nat>"

subsubsection \<open> Invariant \<close>
definition inv_BoxY :: "BoxY \<Rightarrow> \<bool>" where
  "inv_BoxY boxY \<equiv> boxY \<le> BOX_Y_MAX"

(*************************************************************************)
section \<open> Dot \<close>
text \<open> Represents a dot on the grid. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> Dot \<close>
text \<open>\begin{vdmsl}[breaklines=true]
Dot ::
	x: DotX
	y: DotY
-- Invariants for x and y are based in DotX and DotY.
inv mk_Dot(-, -) == true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
record Dot =
  x :: DotX
  y :: DotY

subsubsection \<open> Invariant \<close>
definition inv_Dot_flat :: "DotX \<Rightarrow> DotY \<Rightarrow> \<bool>" where
  "inv_Dot_flat dotX dotY \<equiv> 
    inv_DotX dotX 
    \<and> inv_DotY dotY"
definition inv_Dot :: "Dot \<Rightarrow> \<bool>" where
  "inv_Dot dot \<equiv> 
    inv_Dot_flat (x dot) (y dot)"

(*-----------------------------------------------------------------------*)
subsection \<open> are_dots_equal \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If two dots are equal.
are_dots_equal: Dot * Dot -> bool
are_dots_equal(dot1, dot2) ==
	(dot1.x = dot2.x)
	 and
	(dot1.y = dot2.y)
-- No precondition.
pre true
post if RESULT then dot1 = dot2 else dot1 <> dot2;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition are_dots_equal :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool>" where
  "are_dots_equal dot1 dot2 \<equiv> 
    x dot1 = x dot2
    \<and> y dot1 = y dot2"

subsubsection \<open> Precondition \<close>
definition pre_are_dots_equal :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool>" where
  "pre_are_dots_equal dot1 dot2 \<equiv> 
    inv_Dot dot1 
    \<and> inv_Dot dot2"

subsubsection \<open> Postcondition \<close>
definition post_are_dots_equal :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_are_dots_equal dot1 dot2 RESULT \<equiv> 
    inv_Dot dot1 
    \<and> inv_Dot dot2"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_are_dots_equal_sat_obl :: "\<bool>" where
  "PO_are_dots_equal_sat_obl \<equiv> 
    \<forall> d1 d2 . 
      pre_are_dots_equal d1 d2 \<longrightarrow> 
        (let RESULT = (are_dots_equal d1 d2) in
          post_are_dots_equal d1 d2 RESULT)"

theorem PO_are_dots_equal_sat_obl
  by (simp add: PO_are_dots_equal_sat_obl_def post_are_dots_equal_def pre_are_dots_equal_def)

(*-----------------------------------------------------------------------*)
subsection \<open> are_dots_adjacent \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If two dots are adjacent.
are_dots_adjacent: Dot * Dot -> bool
are_dots_adjacent(fromd, tod) ==
	-- fromd is 1 to the left of tod.
	(fromd.x = tod.x - 1 and fromd.y = tod.y)
	or
	-- fromd is 1 above toDot.
	(fromd.x = tod.x and fromd.y = tod.y - 1)
-- No precondition as the two dots are guarenteed to be valid dots (invariants from mk_Dot) and it doesn't matter if they are equal.
pre true
-- No postcondition.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition are_dots_adjacent :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool>" where
  "are_dots_adjacent fromd tod \<equiv> 
    \<not> are_dots_equal fromd tod
    \<and> ((x fromd = x tod - 1 \<and> y fromd = y tod) 
        \<or> (x fromd = x tod \<and> y fromd = y tod - 1))"

subsubsection \<open> Precondition \<close>
definition pre_are_dots_adjacent :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool>" where
  "pre_are_dots_adjacent fromd tod \<equiv> 
    inv_Dot fromd 
    \<and> inv_Dot tod
    \<and> pre_are_dots_equal fromd tod
    \<and> post_are_dots_equal fromd tod (are_dots_equal fromd tod)"

subsubsection \<open> Postcondition \<close>
definition post_are_dots_adjacent :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_are_dots_adjacent fromd tod RESULT \<equiv> 
    inv_Dot fromd 
    \<and> inv_Dot tod"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_are_dots_adjacent_sat_obl :: "\<bool>" where
  "PO_are_dots_adjacent_sat_obl \<equiv> 
    \<forall> d1 d2 . 
      pre_are_dots_adjacent d1 d2 \<longrightarrow> 
        (let RESULT = (are_dots_adjacent d1 d2) in
          post_are_dots_adjacent d1 d2 RESULT)"

theorem PO_are_dots_adjacent_sat_obl
  by (simp add: PO_are_dots_adjacent_sat_obl_def post_are_dots_adjacent_def pre_are_dots_adjacent_def)

(*************************************************************************)
section \<open> Line \<close>
text \<open> Represents two connected dots on the grid. Dots must go to the right or down, not left or up. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> Line \<close>
text \<open>\begin{vdmsl}[breaklines=true]
Line ::
	fromDot: Dot
	toDot: Dot
-- Dots must be adjacent for it to be a valid line
inv mk_Line(fd, td) ==
	are_dots_adjacent(fd, td);
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
record Line =
  fromDot :: Dot
  toDot :: Dot

subsubsection \<open> Invariant \<close>
definition inv_Line_flat :: "Dot \<Rightarrow> Dot \<Rightarrow> \<bool>" where
  "inv_Line_flat fromd tod \<equiv>   
    inv_Dot fromd 
    \<and> inv_Dot tod
    \<and> are_dots_adjacent fromd tod"
definition inv_Line :: "Line \<Rightarrow> \<bool>" where
  "inv_Line line \<equiv>  
    inv_Line_flat (fromDot line) (toDot line)"

(*-----------------------------------------------------------------------*)
subsection \<open> are_lines_equal \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If two lines have the same from and to dots
are_lines_equal: Line * Line -> bool
are_lines_equal(line1, line2) ==
	are_dots_equal(line1.fromDot, line2.fromDot)
	and
	are_dots_equal(line1.toDot, line2.toDot)
-- No precondition.
pre true
post if RESULT then line1 = line2 else line1 <> line2;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition are_lines_equal :: "Line \<Rightarrow> Line \<Rightarrow> \<bool>" where
  "are_lines_equal line1 line2 \<equiv>
    are_dots_equal (fromDot line1) (fromDot line2) 
    \<and> are_dots_equal (toDot line1) (toDot line2)"

subsubsection \<open> Precondition \<close>
definition pre_are_lines_equal :: "Line \<Rightarrow> Line \<Rightarrow> \<bool>" where
  "pre_are_lines_equal line1 line2 \<equiv>  
    inv_Line line1 
    \<and> inv_Line line2
    \<and> pre_are_dots_equal (fromDot line1) (fromDot line2) 
    \<and> pre_are_dots_equal (toDot line1) (toDot line2)
    \<and> post_are_dots_equal (fromDot line1) (fromDot line2) (are_dots_equal (fromDot line1) (fromDot line2))
    \<and> post_are_dots_equal (toDot line1) (toDot line2) (are_dots_equal (toDot line1) (toDot line2))"

subsubsection \<open> Postcondition \<close>
definition post_are_lines_equal :: "Line \<Rightarrow> Line \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_are_lines_equal line1 line2 RESULT \<equiv> 
    inv_Line line1 
    \<and> inv_Line line2
    \<and> (if RESULT then 
        line1 = line2
       else
        line1 \<noteq> line2)"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_are_lines_equal_sat_obl :: "\<bool>" where
  "PO_are_lines_equal_sat_obl \<equiv> 
    \<forall> l1 l2 . 
      pre_are_lines_equal l1 l2 \<longrightarrow> 
        (let RESULT = (are_lines_equal l1 l2) in
          post_are_lines_equal l1 l2 RESULT)"

theorem PO_are_lines_equal_sat_obl
  unfolding PO_are_lines_equal_sat_obl_def
  unfolding post_are_lines_equal_def are_lines_equal_def pre_are_lines_equal_def
  unfolding inv_Line_def inv_Line_flat_def
  unfolding pre_are_dots_equal_def are_dots_equal_def post_are_dots_equal_def
  by auto

(*************************************************************************)
section \<open> MadeMoves \<close>
text \<open> Represents a sequence of lines that have been drawn. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> MadeMoves \<close>
text \<open>\begin{vdmsl}[breaklines=true]
MadeMoves = seq of Line
-- The same line cannot be drawn twice.
inv madeMoves == are_no_duplicates[Line](madeMoves);
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
type_synonym MadeMoves = "Line VDMSeq"

subsubsection \<open> Invariant \<close>
definition inv_MadeMoves :: "MadeMoves \<Rightarrow> \<bool>" where
  "inv_MadeMoves lines \<equiv>  
    inv_SeqElems inv_Line lines 
    \<and> are_no_duplicates lines"

(*-----------------------------------------------------------------------*)
subsection \<open> box_exists_at \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If a box exists at a point. The point used is to the top left of the box.
box_exists_at: MadeMoves * BoxX * BoxY -> bool
box_exists_at(lines, boxX, boxY) ==
	-- top, right, bottom, left
	(mk_Line(mk_Dot(boxX  , boxY  ), mk_Dot(boxX+1, boxY  )) in set elems lines) and
	(mk_Line(mk_Dot(boxX+1, boxY  ), mk_Dot(boxX+1, boxY+1)) in set elems lines) and
	(mk_Line(mk_Dot(boxX  , boxY+1), mk_Dot(boxX+1, boxY+1)) in set elems lines) and
	(mk_Line(mk_Dot(boxX  , boxY  ), mk_Dot(boxX  , boxY+1)) in set elems lines)
-- No precondition. Invariants for boxX and boxY are handled in their type.
-- A box will not be in an empty MadeMoves and would not signal an issue.
pre true
-- No postcondition.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition box_exists_at :: "MadeMoves \<Rightarrow> BoxX \<Rightarrow> BoxY \<Rightarrow> \<bool>" where
  "box_exists_at lines boxX boxY \<equiv>    (\<lparr>fromDot=\<lparr>x=boxX  , y=boxY  \<rparr>, toDot=\<lparr>x=boxX+1, y=boxY  \<rparr>\<rparr> \<in> set lines)
                                    \<and> (\<lparr>fromDot=\<lparr>x=boxX+1, y=boxY  \<rparr>, toDot=\<lparr>x=boxX+1, y=boxY+1\<rparr>\<rparr> \<in> set lines)
                                    \<and> (\<lparr>fromDot=\<lparr>x=boxX  , y=boxY+1\<rparr>, toDot=\<lparr>x=boxX+1, y=boxY+1\<rparr>\<rparr> \<in> set lines)
                                    \<and> (\<lparr>fromDot=\<lparr>x=boxX  , y=boxY  \<rparr>, toDot=\<lparr>x=boxX  , y=boxY+1\<rparr>\<rparr> \<in> set lines)"

subsubsection \<open> Precondition \<close>
definition pre_box_exists_at :: "MadeMoves \<Rightarrow> BoxX \<Rightarrow> BoxY \<Rightarrow> \<bool>" where
  "pre_box_exists_at lines boxX boxY \<equiv>  
    inv_MadeMoves lines 
    \<and> inv_BoxX boxX 
    \<and> inv_BoxY boxY
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX  , y=boxY  \<rparr> , toDot=\<lparr>x=boxX+1, y=boxY  \<rparr>\<rparr>
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX+1, y=boxY  \<rparr> , toDot=\<lparr>x=boxX+1, y=boxY+1\<rparr>\<rparr>
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX  , y=boxY+1\<rparr> , toDot=\<lparr>x=boxX+1, y=boxY+1\<rparr>\<rparr>
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX  , y=boxY  \<rparr> , toDot=\<lparr>x=boxX  , y=boxY+1\<rparr>\<rparr>"

subsubsection \<open> Postcondition \<close>
definition post_box_exists_at :: "MadeMoves \<Rightarrow> BoxX \<Rightarrow> BoxY \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_box_exists_at lines boxX boxY RESULT \<equiv>  
    inv_MadeMoves lines 
    \<and> inv_BoxX boxX 
    \<and> inv_BoxY boxY
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX  , y=boxY  \<rparr> , toDot=\<lparr>x=boxX+1, y=boxY  \<rparr>\<rparr>
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX+1, y=boxY  \<rparr> , toDot=\<lparr>x=boxX+1, y=boxY+1\<rparr>\<rparr>
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX  , y=boxY+1\<rparr> , toDot=\<lparr>x=boxX+1, y=boxY+1\<rparr>\<rparr>
    \<and> inv_Line \<lparr>fromDot=\<lparr>x=boxX  , y=boxY  \<rparr> , toDot=\<lparr>x=boxX  , y=boxY+1\<rparr>\<rparr>"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_box_exists_at_sat_obl :: "\<bool>" where
  "PO_box_exists_at_sat_obl \<equiv> 
    \<forall> madeMoves boxX boxY . 
      pre_box_exists_at madeMoves boxX boxY \<longrightarrow> 
        (let RESULT = (box_exists_at madeMoves boxX boxY) in
          post_box_exists_at madeMoves boxX boxY RESULT)"

theorem PO_box_exists_at_sat_obl
  unfolding PO_box_exists_at_sat_obl_def
  unfolding post_box_exists_at_def box_exists_at_def pre_box_exists_at_def
  by simp

(*-----------------------------------------------------------------------*)
subsection \<open> box_exists_at_nat \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Simple helper used when counting boxes. Returns 1 if a box exists, else 0.
box_exists_at_nat: MadeMoves * BoxX * BoxY -> nat
box_exists_at_nat(lines, boxX, boxY) ==
	if box_exists_at(lines, boxX, boxY) then 1 else 0
-- See precondition on box_exists_at.
pre true
-- Value must be 0 or 1 as a box exists or does not, there cannot be multiple at a given spot.
post RESULT = 0 or RESULT = 1;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition box_exists_at_nat :: "MadeMoves \<Rightarrow> BoxX \<Rightarrow> BoxY \<Rightarrow> \<nat>" where
  "box_exists_at_nat lines boxX boxY \<equiv>  
    if box_exists_at lines boxX boxY then 
      1
    else
      0"

subsubsection \<open> Precondition \<close>
definition pre_box_exists_at_nat :: "MadeMoves \<Rightarrow> BoxX \<Rightarrow> BoxY \<Rightarrow> \<bool>" where
  "pre_box_exists_at_nat lines boxX boxY \<equiv>
    pre_box_exists_at lines boxX boxY"

subsubsection \<open> Postcondition \<close>
definition post_box_exists_at_nat :: "MadeMoves \<Rightarrow> BoxX \<Rightarrow> BoxY \<Rightarrow> \<nat> \<Rightarrow> \<bool>" where
  "post_box_exists_at_nat lines boxX boxY RESULT \<equiv> 
    post_box_exists_at lines boxX boxY (box_exists_at lines boxX boxY)"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_box_exists_at_nat_sat_obl :: "\<bool>" where
  "PO_box_exists_at_nat_sat_obl \<equiv> 
    \<forall> madeMoves boxX boxY . 
      pre_box_exists_at_nat madeMoves boxX boxY \<longrightarrow> 
        (let RESULT = (box_exists_at_nat madeMoves boxX boxY) in
          post_box_exists_at_nat madeMoves boxX boxY RESULT)"

theorem PO_box_exists_at_nat_sat_obl
  unfolding PO_box_exists_at_nat_sat_obl_def
  unfolding post_box_exists_at_nat_def box_exists_at_nat_def pre_box_exists_at_nat_def
  unfolding post_box_exists_at_def box_exists_at_def pre_box_exists_at_def
  by simp

(*-----------------------------------------------------------------------*)
subsection \<open> box_count_r \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Recursively calculates how many boxes are in a MadeMoves. 
-- Pos represents how far in the grid it is, counting down by moving from the right to the left, then moving up a line. 
box_count_r: MadeMoves * nat -> nat
box_count_r(lines, pos) ==
	cases pos:
		-- Hit the start of the grid
		0 -> box_exists_at_nat(lines, 0, 0),
		-- x = n mod DOT_X_MAX, y = n div DOT_X_MAX
		n -> box_exists_at_nat(lines, n mod DOT_X_MAX, n div DOT_X_MAX) + box_count_r(lines, n - 1)
	end
-- Pos must be less than the max 1D box pos.
pre pos <= (GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1
-- No postcondition.
post true
-- The pos will decrement by 1 every call.
measure pos;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
fun box_count_r :: "MadeMoves \<Rightarrow> \<nat> \<Rightarrow> \<nat>" where
  box_count_r0 : "box_count_r lines 0 = box_exists_at_nat lines 0 0"
| box_count_rN : "box_count_r lines (Suc n) = box_exists_at_nat lines (n mod DOT_X_MAX) (n div DOT_X_MAX) + box_count_r lines n"

subsubsection \<open> Precondition \<close>
definition pre_box_count_r :: "MadeMoves \<Rightarrow> \<nat> \<Rightarrow> \<bool>" where
  "pre_box_count_r lines n \<equiv>  
    inv_MadeMoves lines 
    \<and> n \<le> (GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1"

subsubsection \<open> Postcondition \<close>
definition post_box_count_r :: "MadeMoves \<Rightarrow> \<nat> \<Rightarrow> \<nat> \<Rightarrow> \<bool>" where
  "post_box_count_r lines n RESULT \<equiv>  
    inv_MadeMoves lines"

subsubsection \<open> Measure \<close>
definition measure_box_count_r :: "MadeMoves \<Rightarrow> \<nat> \<Rightarrow> \<nat>" where
  "measure_box_count_r lines n \<equiv> n"

subsubsection \<open> Lemmas \<close>
lemma l_box_count_r_sat_obl:
  "\<forall> madeMoves n .
      pre_box_count_r madeMoves n \<longrightarrow> 
        post_box_count_r madeMoves n (box_count_r madeMoves n)"
  unfolding post_box_count_r_def pre_box_count_r_def
  unfolding inv_MadeMoves_def
  by simp

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_box_count_r_sat_obl :: "\<bool>" where
  "PO_box_count_r_sat_obl \<equiv> 
    \<forall> madeMoves n .
      pre_box_count_r madeMoves n \<longrightarrow> 
        (let RESULT = (box_count_r madeMoves n) in
          post_box_count_r madeMoves n RESULT)"

theorem PO_box_count_r_sat_obl
  by (simp add: PO_box_count_r_sat_obl_def l_box_count_r_sat_obl)

(*-----------------------------------------------------------------------*)
subsection \<open> box_count \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- A helper for calling box_count_r that fills in the starting position as the bottom right of the grid.
box_count: MadeMoves -> nat
box_count(lines) == box_count_r(lines, (GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1)
-- See precondition for box_count_r.
pre true
-- See postcondition for box_count_r.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition box_count :: "MadeMoves \<Rightarrow> \<nat>" where
  "box_count lines \<equiv>  
    box_count_r lines (nat ((GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1))"

subsubsection \<open> Precondition \<close>
definition pre_box_count :: "MadeMoves \<Rightarrow> \<bool>" where
  "pre_box_count lines \<equiv>  
    inv_MadeMoves lines
    \<and> pre_box_count_r lines (nat ((GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1))"

subsubsection \<open> Postcondition \<close>
definition post_box_count :: "MadeMoves \<Rightarrow> \<nat> \<Rightarrow> \<bool>" where
  "post_box_count lines RESULT \<equiv>  
    inv_MadeMoves lines 
    \<and> pre_box_count_r lines (nat ((GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1))
    \<and> post_box_count_r 
        lines 
        (nat ((GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1)) 
        (box_count_r lines (nat ((GRID_WIDTH - 1) * (GRID_HEIGHT - 1) - 1)))"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_box_count_sat_obl :: "\<bool>" where
  "PO_box_count_sat_obl \<equiv> 
    \<forall> lines . 
      pre_box_count lines \<longrightarrow>
        (let RESULT = (box_count lines) in
          post_box_count lines RESULT)"

theorem PO_box_count_sat_obl
  unfolding PO_box_count_sat_obl_def
  unfolding post_box_count_def pre_box_count_def
  by (simp add: l_box_count_r_sat_obl)

(*-----------------------------------------------------------------------*)
subsection \<open> who_played_last_move \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Who played the last move is calculated by moving forward from the first two moves. 
who_played_last_move: MadeMoves -> Player
who_played_last_move(lines) ==
	cases len lines:
		--   len = 1 means first player has played.
		1 -> FIRST_PLAYER,
		--   Cannot get an extra move from 1 line, so 2nd move must have been by the 2nd player.
		2 -> SECOND_PLAYER,
		--	 Cannot get an extra move from 2 lines, so 3rd move must have been made by 1st player.
		3 -> FIRST_PLAYER,
			-- If the box count 2 turns ago is less than the box count 1 turn ago, then the last turn made was a free turn (i.e. the turn did not swap)
		n -> if box_count(take[Line](lines, n - 2)) < box_count(take[Line](lines, n - 1)) then
					 who_played_last_move(take[Line](lines, n - 1))
			-- If the box count 3 turns ago was 2 less than the box count 2 turns ago, then this turn was also a free turn (double free turns from a double-box-making move)	 
				 else if box_count(take[Line](lines, n - 3)) = box_count(take[Line](lines, n - 2)) - 2 then
				 	 who_played_last_move(take[Line](lines, n - 1))
				 else
					 other_player(who_played_last_move(take[Line](lines, n - 1)))
	end
-- Nobody played the last move if no moves were played.
pre len lines >= 1
-- No postcondition.
post true
-- The length of lines will decrement by 1 every call.
measure len lines;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
fun who_played_last_move :: "MadeMoves \<Rightarrow> Player" where
  who_played_last_moveLen0 : "who_played_last_move [] = Player1"
| who_played_last_moveLen1 : "who_played_last_move [l] = Player1"
| who_played_last_moveLen2 : "who_played_last_move [l1, l2] = Player2"
| who_played_last_moveLen3 : "who_played_last_move [l1, l2, l3] = Player1"
| who_played_last_moveLenN : "who_played_last_move (l # ls) = (
                                if (box_count (take ls (length ls - 1))) < (box_count ls) then
                                  who_played_last_move ls
                                else if (box_count (take ls (length ls - 2)) < box_count (take ls (length ls - 1))) then
                                  who_played_last_move ls
                                else
                                  other_player (who_played_last_move ls)
)"

subsubsection \<open> Precondition \<close>
definition pre_who_played_last_move :: "MadeMoves \<Rightarrow> \<bool>" where
  "pre_who_played_last_move lines \<equiv>  
    inv_MadeMoves lines
    \<and> pre_box_count lines"

subsubsection \<open> Postcondition \<close>
definition post_who_played_last_move :: "MadeMoves \<Rightarrow> Player \<Rightarrow> \<bool>" where
  "post_who_played_last_move lines RESULT \<equiv>  
    inv_MadeMoves lines
    \<and> pre_box_count lines
    \<and> post_box_count lines (box_count lines)"

subsubsection \<open> Measure \<close>
definition measure_who_played_last_move :: "MadeMoves \<Rightarrow> \<nat>" where
  "measure_who_played_last_move lines \<equiv> length lines"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_who_played_last_move_sat_obl :: "\<bool>" where
  "PO_who_played_last_move_sat_obl \<equiv> 
    \<forall> madeMoves . 
      pre_who_played_last_move madeMoves \<longrightarrow>
        (let RESULT = (who_played_last_move madeMoves) in
          post_who_played_last_move madeMoves RESULT)"

theorem PO_who_played_last_move_sat_obl
  unfolding PO_who_played_last_move_sat_obl_def
  unfolding post_who_played_last_move_def pre_who_played_last_move_def
  unfolding post_box_count_def post_box_count_r_def pre_box_count_def pre_box_count_r_def
  by simp

(*-----------------------------------------------------------------------*)
subsection \<open> box_count_for \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Recursively calculates how many boxes a player has made.
box_count_for: MadeMoves * Player -> nat
box_count_for(lines, player) ==
	cases len lines:
		-- Can't have a box without lines
		0 -> 0,
		- -> if who_played_last_move(lines) <> player then
					-- ignore the last move as it was not from the specified player
					box_count_for(take[Line](lines, len lines - 1), player)
				 else
					-- bc(l) - bc(l-1) will be 0 if no box was made (i.e. just bcf(l-1))
					-- if one or more boxes were made, then bc(l) - bc(l-1) will equal that number
					(box_count(lines) - box_count(take[Line](lines, len lines - 1)))
					-- then add the number of boxes made in previous moves
					+ box_count_for(take[Line](lines, len lines - 1), player)
	end
-- No precondition. An empty MadeMoves is the base case.
pre true
-- No postcondition.
post true
-- The length of lines will decrement by 1 every call.
measure len lines;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
fun box_count_for :: "MadeMoves \<Rightarrow> Player \<Rightarrow> \<nat>" where
  box_count_forLen0 : "box_count_for [] p = 0"
| box_count_forLenN : "box_count_for (l # ls) p = (
                        if who_played_last_move (l # ls) \<noteq> p then 
                          box_count_for ls p
                        else
                          box_count_for ls p + (box_count (l # ls) - box_count ls)
)"

subsubsection \<open> Precondition \<close>
definition pre_box_count_for :: "MadeMoves \<Rightarrow> Player \<Rightarrow> \<bool>" where
  "pre_box_count_for lines player \<equiv>  
    inv_MadeMoves lines
    \<and> pre_box_count lines"

subsubsection \<open> Postcondition \<close>
definition post_box_count_for :: "MadeMoves \<Rightarrow> Player \<Rightarrow> \<nat> \<Rightarrow> \<bool>" where
  "post_box_count_for lines player RESULT \<equiv>  
    inv_MadeMoves lines 
    \<and> pre_box_count lines
    \<and> post_box_count lines (box_count lines)"

subsubsection \<open> Measure \<close>
definition measure_box_count_for :: "MadeMoves \<Rightarrow> Player \<Rightarrow> \<nat>" where
  "measure_box_count_for lines player \<equiv> length lines"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_box_count_for_sat_obl :: "\<bool>" where
  "PO_box_count_for_sat_obl \<equiv> 
    \<forall> madeMoves player .
      pre_box_count_for madeMoves player \<longrightarrow>
        (let RESULT = (box_count_for madeMoves player) in
          post_box_count_for madeMoves player RESULT)"

theorem PO_box_count_for_sat_obl
  unfolding PO_box_count_for_sat_obl_def
  unfolding post_box_count_for_def pre_box_count_for_def
  unfolding post_box_count_def post_box_count_r_def pre_box_count_def pre_box_count_r_def
  by simp

(*************************************************************************)
section \<open> Game State \<close>
text \<open> Represents the state of the game. \<close>

(*-----------------------------------------------------------------------*)
subsection \<open> DotsNBoxes \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- The state is incredibly simple (only one seq of Line) as the game's state can be calculated based off of this.
-- If it is empty, then the first player is to go. Otherwise, the next player to go can be calculated based off of the previously made moves.
-- This would be done similar to who_played_last_move by checking if the last move created a box, as this would lead to the same player going again.
-- Saving the next player to move here would be duplication of data.
state DotsNBoxes of
	madeMoves: MadeMoves
-- No invariant. Invariant for madeMoves is on the type.
inv mk_DotsNBoxes(-) == 
	true
-- Initialised to have no moves played.
init dotsNBoxes == dotsNBoxes = mk_DotsNBoxes([])
end
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
record DotsNBoxesState =
  madeMoves :: MadeMoves

subsubsection \<open> Invariant \<close>
definition inv_DotsNBoxesState_flat :: "MadeMoves \<Rightarrow> \<bool>" where
  "inv_DotsNBoxesState_flat lines \<equiv>  
    inv_MadeMoves lines"
definition inv_DotsNBoxesState :: "DotsNBoxesState \<Rightarrow> \<bool>" where
  "inv_DotsNBoxesState dotsNBoxes \<equiv>  
    inv_DotsNBoxesState_flat (madeMoves dotsNBoxes)"

subsubsection \<open> Initialisation \<close>
definition init_DotsNBoxesState :: "DotsNBoxesState" where
  "init_DotsNBoxesState \<equiv>  
    \<lparr> madeMoves = [] \<rparr>"
definition post_init_DotsNBoxesState :: "\<bool>" where
  "post_init_DotsNBoxesState \<equiv>  
    inv_DotsNBoxesState init_DotsNBoxesState"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_init_DotsNBoxesState_sat_obl :: "\<bool>" where
  "PO_init_DotsNBoxesState_sat_obl \<equiv> 
    post_init_DotsNBoxesState"

theorem PO_init_DotsNBoxesState_sat_obl
  unfolding PO_init_DotsNBoxesState_sat_obl_def
  unfolding post_init_DotsNBoxesState_def
  unfolding init_DotsNBoxesState_def inv_DotsNBoxesState_def inv_DotsNBoxesState_flat_def
  unfolding inv_MadeMoves_def inv_SeqElems_def inv_Line_def inv_Line_flat_def
  apply simp
  using are_no_duplicates_def l_are_no_duplicates_empty_true by auto

(*-----------------------------------------------------------------------*)
subsection \<open> game_are_all_moves_exhausted \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If all moves have been played.
game_are_all_moves_exhausted: MadeMoves -> bool
game_are_all_moves_exhausted(lines) ==
	len lines = (GRID_WIDTH - 1) * GRID_HEIGHT * 2
-- No precondition. No made moves just equals false, and would not be an error.
pre true
-- No postcondition.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition game_are_all_moves_exhausted :: "MadeMoves \<Rightarrow> \<bool>" where
  "game_are_all_moves_exhausted lines \<equiv>  
    len lines = (GRID_WIDTH - 1) * GRID_HEIGHT * 2"

subsubsection \<open> Precondition \<close>
definition pre_game_are_all_moves_exhausted :: "MadeMoves \<Rightarrow> \<bool>" where
  "pre_game_are_all_moves_exhausted lines \<equiv>  
    inv_MadeMoves lines"

subsubsection \<open> Postcondition \<close>
definition post_game_are_all_moves_exhausted :: "MadeMoves \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_game_are_all_moves_exhausted lines RESULT \<equiv>  
    inv_MadeMoves lines"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_game_are_all_moves_exhausted_sat_obl :: "\<bool>" where
  "PO_game_are_all_moves_exhausted_sat_obl \<equiv> 
    \<forall> madeMoves .
      pre_game_are_all_moves_exhausted madeMoves \<longrightarrow>
        (let RESULT = (game_are_all_moves_exhausted madeMoves) in
          post_game_are_all_moves_exhausted madeMoves RESULT)"

theorem PO_game_are_all_moves_exhausted_sat_obl
  unfolding PO_game_are_all_moves_exhausted_sat_obl_def
  by (simp add: post_game_are_all_moves_exhausted_def pre_game_are_all_moves_exhausted_def)

(*-----------------------------------------------------------------------*)
subsection \<open> game_is_stalemate \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- If the game has ended and both players have the same score.
game_is_stalemate: MadeMoves -> bool
game_is_stalemate(lines) ==
	box_count_for(lines, <Player1>) = box_count_for(lines, <Player2>)
-- Cannot be a stalemate if there are moves left to be played.
pre game_are_all_moves_exhausted(lines)
-- No postcondition.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition game_is_stalemate :: "MadeMoves \<Rightarrow> \<bool>" where
  "game_is_stalemate lines \<equiv>  
    box_count_for lines Player1 = box_count_for lines Player2"

subsubsection \<open> Precondition \<close>
definition pre_game_is_stalemate :: "MadeMoves \<Rightarrow> \<bool>" where
  "pre_game_is_stalemate lines \<equiv>  
    inv_MadeMoves lines 
    \<and> game_are_all_moves_exhausted lines
    \<and> pre_box_count_for lines Player1
    \<and> pre_box_count_for lines Player2"

subsubsection \<open> Postcondition \<close>
definition post_game_is_stalemate :: "MadeMoves \<Rightarrow> \<bool> \<Rightarrow> \<bool>" where
  "post_game_is_stalemate lines RESULT \<equiv>  
    inv_MadeMoves lines
    \<and> pre_box_count_for lines Player1
    \<and> pre_box_count_for lines Player2
    \<and> post_box_count_for lines Player1 (box_count_for lines Player1)
    \<and> post_box_count_for lines Player2 (box_count_for lines Player2)"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_game_is_stalemate_sat_obl :: "\<bool>" where
  "PO_game_is_stalemate_sat_obl \<equiv> 
    \<forall> madeMoves .
      pre_game_is_stalemate madeMoves \<longrightarrow>
        (let RESULT = (game_is_stalemate madeMoves) in
          post_game_is_stalemate madeMoves RESULT)"

theorem PO_game_is_stalemate_sat_obl
  unfolding PO_game_is_stalemate_sat_obl_def
  unfolding post_game_is_stalemate_def pre_game_is_stalemate_def
  by (simp add: post_box_count_def post_box_count_for_def post_box_count_r_def pre_box_count_def pre_box_count_for_def)

(*-----------------------------------------------------------------------*)
subsection \<open> game_winner \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Who has won the game.
game_winner: MadeMoves -> Player
game_winner(lines) ==
	if box_count_for(lines, <Player1>) > box_count_for(lines, <Player2>) then
		<Player1>
	else
		<Player2>
-- Cannot be a winner if it is a stalemate.
pre not game_is_stalemate(lines)
-- No postcondition.
post true;
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition game_winner :: "MadeMoves \<Rightarrow> Player" where
  "game_winner lines \<equiv>  
    if box_count_for lines Player1 > box_count_for lines Player2 then 
      Player1
    else
      Player2"

subsubsection \<open> Precondition \<close>
definition pre_game_winner :: "MadeMoves \<Rightarrow> \<bool>" where
  "pre_game_winner lines \<equiv>  
    inv_MadeMoves lines 
    \<and> \<not>(game_is_stalemate lines)
    \<and> pre_box_count_for lines Player1
    \<and> pre_box_count_for lines Player2"

subsubsection \<open> Postcondition \<close>
definition post_game_winner :: "MadeMoves \<Rightarrow> Player \<Rightarrow> \<bool>" where
  "post_game_winner lines RESULT \<equiv>  
    inv_MadeMoves lines
    \<and> pre_box_count_for lines Player1
    \<and> pre_box_count_for lines Player2
    \<and> post_box_count_for lines Player1 (box_count_for lines Player1)
    \<and> post_box_count_for lines Player2 (box_count_for lines Player2)"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_game_winner_sat_obl :: "\<bool>" where
  "PO_game_winner_sat_obl \<equiv> 
    \<forall> madeMoves .
      pre_game_winner madeMoves \<longrightarrow> 
        (let RESULT = (game_winner madeMoves) in
          post_game_winner madeMoves RESULT)"

theorem PO_game_winner_sat_obl
  unfolding PO_game_winner_sat_obl_def
  unfolding post_game_winner_def game_winner_def pre_game_winner_def
  by (simp add: post_box_count_def post_box_count_for_def post_box_count_r_def pre_box_count_def pre_box_count_for_def)

(*-----------------------------------------------------------------------*)
subsection \<open> play_move \<close>
text \<open>\begin{vdmsl}[breaklines=true]
-- Adds a line to the played lines.
play_move(line: Line) ==
	madeMoves := madeMoves ^ [line]
-- Writes to madeMoves.
ext wr madeMoves
-- The line may not already be drawn.
pre not line_exists(madeMoves, line)
-- The line will be added to the made moves.
post madeMoves = madeMoves~ ^ [line];
\end{vdmsl}\<close>

subsubsection \<open> Definition \<close>
definition play_move :: "Line \<Rightarrow> DotsNBoxesState \<Rightarrow> DotsNBoxesState" where
  "play_move line beforeState \<equiv> 
    \<lparr> madeMoves = madeMoves beforeState @ [line] \<rparr>"

subsubsection \<open> Precondition \<close>
definition pre_play_move :: "Line \<Rightarrow> DotsNBoxesState \<Rightarrow> \<bool>" where
  "pre_play_move line beforeState \<equiv>  
    inv_Line line 
    \<and> inv_DotsNBoxesState beforeState 
    \<and> line \<notin> elems (madeMoves beforeState)"

subsubsection \<open> Postcondition \<close>
definition post_play_move :: "Line \<Rightarrow> DotsNBoxesState \<Rightarrow> DotsNBoxesState \<Rightarrow> \<bool>" where
  "post_play_move line beforeState afterState \<equiv>
    inv_DotsNBoxesState afterState 
    \<and> (madeMoves afterState) = (madeMoves beforeState) @ [line]"

subsubsection \<open> Satisfiability Proof Obligation \<close>
definition PO_play_move_sat_obl :: "\<bool>" where
  "PO_play_move_sat_obl \<equiv> 
    \<forall> line state .
      pre_play_move line state \<longrightarrow>
        (let RESULT = (play_move line state) in
          post_play_move line state RESULT)"

theorem PO_play_move_sat_obl
  unfolding PO_play_move_sat_obl_def
  unfolding post_play_move_def play_move_def pre_play_move_def
  unfolding inv_DotsNBoxesState_def inv_DotsNBoxesState_flat_def
  by (simp add: inv_MadeMoves_def l_are_no_duplicates_append_no_duplicates)

