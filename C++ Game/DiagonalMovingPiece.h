/**
 * Contains the definition of a piece which is capable of moving diagonally.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"

class DiagonalMovingPiece: public virtual Piece
{
public:
	explicit DiagonalMovingPiece(const double x, const double y);
	DiagonalMovingPiece(const DiagonalMovingPiece& other);
	DiagonalMovingPiece(DiagonalMovingPiece&& other) noexcept;
	virtual ~DiagonalMovingPiece();

	bool is_valid_move(const double new_x, const double new_y) const override;
};

