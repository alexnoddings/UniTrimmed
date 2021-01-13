/**
 * Contains the functions for a piece which is capable of moving horizontally/vertically.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "LateralMovingPiece.h"

LateralMovingPiece::LateralMovingPiece(const double x, const double y) : Piece(x, y)
{
}

LateralMovingPiece::LateralMovingPiece(const LateralMovingPiece& other) : LateralMovingPiece(other.x_, other.y_)
{
}

LateralMovingPiece::LateralMovingPiece(LateralMovingPiece&& other) noexcept : LateralMovingPiece(other.x_, other.y_)
{
}

LateralMovingPiece::~LateralMovingPiece() = default;

bool LateralMovingPiece::is_valid_move(const double new_x, const double new_y) const
{
	// either the x hasn't changed and the y has, or vice versa
	return (new_x == get_x() && new_y != get_y())
		|| (new_x != get_x() && new_y == get_y());
}
