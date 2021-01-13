/**
 * Functions for calculating intersection.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"
#include "Circle.h"
#include "Square.h"

// do intersect between shapes
bool do_intersect(const Circle circle, const Square square);
bool do_intersect(const Square square, const Circle circle);
bool do_intersect(const Square square1, const Square square2);
bool do_intersect(const Circle circle1, const Circle circle2);

// do intersect between pieces
bool do_intersect(const Circle circle, const Piece* piece2);
bool do_intersect(const Square square, const Piece* piece2);
bool do_intersect(const Piece* piece1, const Piece* piece2);

// will intersect between shapes
bool will_intersect(const Circle circle, const Point new_position, const Square square);
bool will_intersect(const Square square, const Point new_position, const Circle circle);
bool will_intersect(const Square square1, const Point new_position, const Square square2);
bool will_intersect(const Circle circle1, const Point new_position, const Circle circle2);

// will intersect between pieces
bool will_intersect(const Circle moving_circle, const Point new_position, const Piece* static_piece);
bool will_intersect(const Square moving_square, const Point new_position, const Piece* static_piece);
bool will_intersect(const Piece* moving_piece, const Point new_position, const Piece* static_piece);