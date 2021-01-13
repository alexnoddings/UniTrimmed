/**
 * Contains the functions for a Rook piece.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Rook.h"

Rook::Rook(const double x, const double y) : Piece(x, y), LateralMovingPiece(x, y)
{
}

Rook::Rook(const Rook& other) : Rook(other.x_, other.y_)
{
}

Rook::Rook(Rook&& other) noexcept : Rook(other.x_, other.y_)
{
}

Rook::~Rook() = default;

bool Rook::is_valid_move(const double new_x, const double new_y) const
{
	return LateralMovingPiece::is_valid_move(new_x, new_y);
}

PieceType Rook::get_piece_type() const
{
	return PieceType::rook;
}
