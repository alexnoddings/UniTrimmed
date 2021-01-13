/**
 * Contains the definition of a Queen piece.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"
#include "LateralMovingPiece.h"
#include "DiagonalMovingPiece.h"

class Queen : public virtual Piece, public virtual LateralMovingPiece, public virtual DiagonalMovingPiece
{
public:
	explicit Queen(const double x, const double y);
	Queen(const Queen& other);
	Queen(Queen&& other) noexcept;
	virtual ~Queen();

	bool is_valid_move(const double new_x, const double new_y) const override;
	PieceType get_piece_type() const override;
};

