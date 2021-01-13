/**
 * Contains the definition of a Bishop piece.
 *
 * @author Alexander Noddings <b7029792@ncl.ac.uk>
 */

#pragma once
#include "Piece.h"
#include "DiagonalMovingPiece.h"

class Bishop : public virtual Piece, public virtual DiagonalMovingPiece
{
public:
	explicit Bishop(const double x, const double y);
	Bishop(const Bishop& other);
	Bishop(Bishop&& other) noexcept;
	virtual ~Bishop();

	bool is_valid_move(const double new_x, const double new_y) const override;
	PieceType get_piece_type() const override;
};

