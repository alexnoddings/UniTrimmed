/**
 * Contains the functions for a Bishop piece.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#include "Bishop.h"

Bishop::Bishop(const double x, const double y) : Piece(x, y), DiagonalMovingPiece(x, y)
{
}

Bishop::Bishop(const Bishop& other) : Bishop(other.x_, other.y_)
{
}

Bishop::Bishop(Bishop&& other) noexcept : Bishop(other.x_, other.y_)
{
}

Bishop::~Bishop() = default;

bool Bishop::is_valid_move(const double new_x, const double new_y) const
{
	return DiagonalMovingPiece::is_valid_move(new_x, new_y);
}

PieceType Bishop::get_piece_type() const
{
	return PieceType::bishop;
}
