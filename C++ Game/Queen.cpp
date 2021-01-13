/**
 * Contains the functions for a Queen piece.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Queen.h"

Queen::Queen(const double x, const double y) : Piece(x, y), LateralMovingPiece(x, y), DiagonalMovingPiece(x, y)
{
}

Queen::Queen(const Queen& other) : Queen(other.x_, other.y_)
{
}

Queen::Queen(Queen&& other) noexcept : Queen(other.x_, other.y_)
{
}

Queen::~Queen() = default;


bool Queen::is_valid_move(const double new_x, const double new_y) const
{
	return LateralMovingPiece::is_valid_move(new_x, new_y) || DiagonalMovingPiece::is_valid_move(new_x, new_y);
}

PieceType Queen::get_piece_type() const
{
	return PieceType::queen;
}
