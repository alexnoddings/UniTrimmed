/**
 * Contains the functions for a piece which is capable of moving diagonally.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "DiagonalMovingPiece.h"
#include <cmath>

#ifndef DBL_EPSILON
	#define DBL_EPSILON 1E-16
#endif

DiagonalMovingPiece::DiagonalMovingPiece(const double x, const double y) : Piece(x, y)
{
}

DiagonalMovingPiece::DiagonalMovingPiece(const DiagonalMovingPiece& other) : DiagonalMovingPiece(other.x_, other.y_)
{
}

DiagonalMovingPiece::DiagonalMovingPiece(DiagonalMovingPiece&& other) noexcept : DiagonalMovingPiece(other.x_, other.y_)
{
}

DiagonalMovingPiece::~DiagonalMovingPiece() = default;

bool DiagonalMovingPiece::is_valid_move(const double new_x, const double new_y) const
{
	// must have moved the same distance on the x and y axis (ignoring -'s)
	const double delta_x = abs(get_x() - new_x);
	const double delta_y = abs(get_y() - new_y);
	const double delta_difference = abs(delta_x - delta_y);

	return delta_difference < DBL_EPSILON;
}
