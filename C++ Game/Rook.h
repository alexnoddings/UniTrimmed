/**
 * Contains the definition of a Rook piece.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"
#include "LateralMovingPiece.h"

class Rook : public virtual Piece, public virtual LateralMovingPiece
{
public:
	explicit Rook(const double x, const double y);
	Rook(const Rook& other);
	Rook(Rook&& other) noexcept;
	virtual ~Rook();

	bool is_valid_move(const double new_x, const double new_y) const override;
	PieceType get_piece_type() const override;
};

