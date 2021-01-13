/**
 * Contains the functions for a square shape.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Square.h"

Square::Square(const double top_left_x, const double top_left_y, const double width)
	: Shape(top_left_x, top_left_y, width)
{
}

Square::Square(const Point top_left, const double width)
	: Square(top_left.get_x(), top_left.get_y(), width)
{
}

Square::Square(const Square& other) : Square(other.top_left_x_, other.top_left_y_, other.width_)
{
}

Square::Square(Square&& other) noexcept : Square(other.top_left_x_, other.top_left_y_, other.width_)
{
}

Square::~Square() = default;
